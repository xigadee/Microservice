#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{

    public abstract class PersistenceManagerHandlerDocumentDbBase<K, E, S, P>: PersistenceManagerHandlerJsonBase<K, E, S, P>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : DocumentDbPersistenceCommandPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This is the connection to documentDb.
        /// </summary>
        protected readonly DocumentDbConnection mConnection;
        /// <summary>
        /// This is the default documentDb database name.
        /// </summary>
        protected readonly string mDatabaseName;
        /// <summary>
        /// This is the base documentDb collection naem
        /// </summary>
        protected readonly string mCollectionName;
        /// <summary>
        /// This sharding policy is used to create the sharded collection.
        /// </summary>
        protected ShardingPolicy<K> mShardingPolicy;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="connection">The documentDb connection.</param>
        /// <param name="database">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="jsonMaker">This function can be used to override the default JSON creation functions.</param>
        /// <param name="databaseCollection">The is the collection name. If the collection does it exist it will be created. This will be used by the sharding policy to create multiple collections.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionMaker">This function should be set to enforce optimistic locking.</param>
        /// <param name="defaultTimeout">This is the default timeout period to be used when connecting to documentDb.</param>
        /// <param name="shardingPolicy">This is sharding policy used to choose the appropriate collection from the key presented.</param>
        /// <param name="retryPolicy"></param>
        public PersistenceManagerHandlerDocumentDbBase(
              DocumentDbConnection connection
            , string database
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string databaseCollection = null
            , ShardingPolicy<K> shardingPolicy = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<K, string> keySerializer = null
            )
            : base(entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , keyMaker: keyMaker
                  , referenceMaker: referenceMaker
                  , keySerializer: keySerializer
                  , keyDeserializer: keyDeserializer
                  )
        {
            mConnection = connection;
            mDatabaseName = database;
            mCollectionName = databaseCollection ?? typeof(E).Name;
            mShardingPolicy = shardingPolicy ?? new ShardingPolicy<K>(mCollectionName, (k) => 0, 1, (i) => mCollectionName);
        }
        #endregion

        /// <summary>
        /// This override create the database and collection.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();

            if (Policy.AutoCreateDatabase)
                CreateDatabase();

            if (Policy.AutoCreateCollections)
                CreateCollections();
        }

        /// <summary>
        /// This method should create the DocumentDb database.
        /// </summary>
        protected abstract void CreateDatabase();

        /// <summary>
        /// This method should create the associated collections for the persistence agent.
        /// </summary>
        protected abstract void CreateCollections();


        #region TimeoutCorrect

        protected override async Task<bool> TimeoutCorrectCreateUpdate(PersistenceRequestHolder<K, E> holder)
        {
            if (holder.Rq.Entity == null || mTransform == null)
                return false;

            var jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);
            var alternateHolder = new PersistenceRequestHolder<K, E>(holder.ProfileId, holder.Prq, holder.Prs)
            {
                Rq = new PersistenceRepositoryHolder<K, E> { Key = jsonHolder.Key, Timeout = holder.Rq.Timeout },
                Rs = new PersistenceRepositoryHolder<K, E>()
            };

            if (!(await RetrieveEntity(alternateHolder, ProcessRead)))
                return false;

            holder.Rs.Entity = alternateHolder.Rs.Entity;
            holder.Rs.Key = alternateHolder.Rs.Key;
            holder.Rs.KeyReference = alternateHolder.Rs.KeyReference;
            holder.Rs.ResponseCode = mTransform.Version.SupportsVersioning && jsonHolder.Version.Equals(mTransform.Version.EntityVersionAsString(holder.Rs.Entity))
                ? alternateHolder.Rs.ResponseCode
                : holder.Rs.ResponseCode;

            return holder.Rs.IsSuccess;
        }

        protected override async Task<bool> TimeoutCorrectDelete(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {

            var alternateHolder = new PersistenceRequestHolder<K, E>(holder.ProfileId, holder.Prq, holder.Prs);
            alternateHolder.Rq = new PersistenceRepositoryHolder<K, E> { Key = holder.Rq.Key, KeyReference = holder.Rq.KeyReference, Timeout = holder.Rq.Timeout };
            alternateHolder.Rs = new PersistenceRepositoryHolder<K, E>();

            bool byref = alternateHolder.Rq.KeyReference != null && !string.IsNullOrEmpty(alternateHolder.Rq.KeyReference.Item2);

            bool result;
            if (byref)
                result = await RetrieveEntity(alternateHolder, ProcessReadByRef);
            else
                result = await RetrieveEntity(alternateHolder, ProcessRead);

            if (result)
                return false;

            // We should have a 404 response code here. If not send back the one we got otherwise send back 200 OK
            holder.Rs.Key = alternateHolder.Rs.Key;
            holder.Rs.KeyReference = alternateHolder.Rs.KeyReference;
            holder.Rs.ResponseCode = alternateHolder.Rs.ResponseCode != 404 ? alternateHolder.Rs.ResponseCode : 200;

            return holder.Rs.IsSuccess;
        }
        #endregion
        #region RetrieveEntity
        /// <summary>
        /// Retrieves an entity using the supplied read action (read or read by ref) using the persistence retry policy
        /// </summary>
        /// <param name="rq">Read request</param>
        /// <param name="rs">Read response</param>
        /// <param name="m">Transmission request message</param>
        /// <param name="l">Tranmission response messages</param>
        /// <param name="readAction">Read action</param>
        /// <returns></returns>
        protected virtual async Task<bool> RetrieveEntity(PersistenceRequestHolder<K, E> holder, Func<PersistenceRequestHolder<K, E>, Task> readAction)
        {
            int numberOfRetries = 0;

            int maximumRetries = Policy.PersistenceRetryPolicy.GetMaximumRetries(holder.Prq);

            while (numberOfRetries < maximumRetries && !holder.Prq.Cancel.IsCancellationRequested)
            {
                await readAction(holder);

                if (holder.Rs.Entity != null || holder.Rs.ResponseCode == 404)
                    return holder.Rs.Entity != null;

                await Task.Delay(Policy.PersistenceRetryPolicy.GetDelayBetweenRetries(holder.Prq, numberOfRetries));

                numberOfRetries++;
            }

            Collector?.LogMessage(LoggingLevel.Error
                , string.Format(
                    "Unable to retrieve entity after {0} retries, message cancelled({1}) on channel({2}) for request: {3} - response: {4}"
                    , numberOfRetries
                    , holder.Prq.Cancel.IsCancellationRequested
                    , holder.Prq.Message != null ? holder.Prq.Message.ChannelPriority.ToString() : "null"
                    , holder.Rq
                    , holder.Rs)
                , "DocDb"
                );

            return false;
        }
        #endregion
    }
}
