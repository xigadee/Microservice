#region using
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This persistence handler uses Azure Blob storage as its underlying storage mechanism.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageHandlerDocumentDbSdk<K, E, S, P>: PersistenceManagerHandlerJsonBase<K, E, S, P>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : PersistenceCommandPolicy, new()
    {
        #region Declarations

        DocumentClient mClient;
        /// <summary>
        /// This is the documentDb collection mapper;
        /// </summary>
        protected Dictionary<string, CollectionHolder> mHolders;
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
        public PersistenceMessageHandlerDocumentDbSdk(DocumentDbConnection connection
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
            : base( entityName: entityName
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

            mClient = connection.ToDocumentClient();
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override creates the document db databaseId and collection if they don't already exist.
        /// </summary>
        protected override void StartInternal()
        {
            mHolders = new Dictionary<string, CollectionHolder>();
            foreach (var collection in mShardingPolicy.Collections)
            {
                mHolders.Add(collection, mConnection.ToCollectionHolder(mDatabaseName, collection, mPolicy.DefaultTimeout, true));
            }

            base.StartInternal();
        }
        #endregion


        #region PersistenceResponseFormat(ResponseHolder result)
        /// <summary>
        /// This method sets the response holder based on the results holder.
        /// </summary>
        /// <param name="result">The documentDb result holder.</param>
        /// <returns>Returns the persistence holder.</returns>
        private PersistenceResponseHolder<E> PersistenceResponseFormat(ResponseHolder result)
        {
            if (result.IsSuccess)
                return new PersistenceResponseHolder<E>() { StatusCode = result.StatusCode, Content = result.Content, IsSuccess = true, Entity = mTransform.PersistenceEntitySerializer.Deserializer(result.Content) };
            else
                return new PersistenceResponseHolder<E>() { StatusCode = result.IsTimeout ? 504 : result.StatusCode, IsSuccess = false, IsTimeout = result.IsTimeout };
        }
        #endregion

        #region InternalCreate
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="key"></param>
        /// <param name="holder"></param>
        protected async override Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);

            var response = await mClient.CreateGeneric(jsonHolder.Json, mDatabaseName, mShardingPolicy.Resolve(key));

            return PersistenceResponseFormat(response);
        }
        #endregion
        #region InternalRead
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="key"></param>
        /// <param name="holder"></param>
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            var response = await mClient.ReadGeneric(mDatabaseName, mShardingPolicy.Resolve(key), mTransform.KeyStringMaker(key));

            return PersistenceResponseFormat(response);
        }
        #endregion
        #region InternalUpdate
        /// <summary>
        /// This method performs a documentDb update.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="holder">The request holder.</param>
        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            string eTag = null;
            string collection = mShardingPolicy.Resolve(key);
            string stringKey = mTransform.KeyStringMaker(key);

            var jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);

            //We check this in case optimistic locking has been turned on, but old versions don't support this yet.
            if (mTransform.Version.SupportsOptimisticLocking) 
            {
                //OK, we need to read from the to validate the version and confirm the eTag.
                var resultRead = await mClient.ReadGeneric(mDatabaseName, collection, stringKey);
                if (resultRead.IsSuccess)
                {
                    E entityCurrent = mTransform.PersistenceEntitySerializer.Deserializer(resultRead.Content);

                    if (mTransform.Version.EntityVersionAsString(entityCurrent) != jsonHolder.Version)
                    {
                        return new PersistenceResponseHolder<E>() { StatusCode = 409, IsSuccess = false, IsTimeout = false, VersionId = jsonHolder.Version };
                    }

                    eTag = resultRead.ETag;

                    ////&& holder.Rq.Fields.ContainsKey(mTransform.Version.VersionJsonMetadata.Key))
                    //var currentVersionId = documentRq.Fields[mTransform.Version.VersionJsonMetadata.Key];

                    //if (currentVersionId != mTransform.Version.EntityVersionAsString(holder.Rq.Entity))
                    //    return new PersistenceResponseHolder<E>() { StatusCode = 409, IsSuccess = false, IsTimeout = false, VersionId = currentVersionId };

                    //Set the new version id on the entity.
                    mTransform.Version.EntityVersionUpdate(holder.Rq.Entity);
                    jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);
                }
            }

            var result = await mClient.UpdateGeneric(jsonHolder.Json, mDatabaseName, collection, stringKey, eTag);

            if (result.IsSuccess && mTransform.Version.SupportsArchiving)
            {
                //mCollection.Create(documentRq.DocumentId, jsonHolder.Json, rq.Timeout).Result;
            }

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalDelete
        /// <summary>
        /// Delete request.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder> InternalDelete(K key,
            PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var response = await mClient.DeleteGeneric(mDatabaseName, mShardingPolicy.Resolve(key), mTransform.KeyStringMaker(key));

            return PersistenceResponseFormat(response);
        }
        #endregion
        #region InternalVersion
        /// <summary>
        /// Version.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var response = await mClient.ReadGeneric(mDatabaseName, mShardingPolicy.Resolve(key), mTransform.KeyStringMaker(key));

            return PersistenceResponseFormat(response);
        }
        #endregion


        protected override void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            IResponseHolder holderResponse)
        {
            if (holderResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(holderResponse.Content))
                {
                    var entity = mTransform.PersistenceEntitySerializer.Deserializer(holderResponse.Content);
                    rq.Key = mTransform.KeyMaker(entity);
                    holderResponse.VersionId = mTransform.Version?.EntityVersionAsString(entity);
                }
                else
                {
                    rq.Key = mTransform.KeyDeserializer(holderResponse.Id);
                }
            }

            base.ProcessOutputKey(rq, rs, holderResponse);
        }

        #region TimeoutCorrect

        protected override async Task<bool> TimeoutCorrectCreateUpdate(PersistenceRequestHolder<K, E> holder)
        {
            if (holder.Rq.Entity == null)
                return false;

            var jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);
            var request = new PersistenceRepositoryHolder<K, E> { Key = jsonHolder.Key, Timeout = holder.Rq.Timeout };
            var response = new PersistenceRepositoryHolder<K, E>();

            if (!(await RetrieveEntity(holder, ProcessRead)))
                return false;

            holder.Rs.Entity = response.Entity;
            holder.Rs.Key = response.Key;
            holder.Rs.KeyReference = response.KeyReference;
            holder.Rs.ResponseCode = !mTransform.Version.SupportsVersioning || jsonHolder.Version.Equals(mTransform.Version.EntityVersionAsString(holder.Rs.Entity))
                ? response.ResponseCode
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

            int maximumRetries = mPolicy.PersistenceRetryPolicy.GetMaximumRetries(holder.Prq);

            while (numberOfRetries < maximumRetries && !holder.Prq.Cancel.IsCancellationRequested)
            {
                await readAction(holder);

                if (holder.Rs.Entity != null || holder.Rs.ResponseCode == 404)
                    return holder.Rs.Entity != null;

                await Task.Delay(mPolicy.PersistenceRetryPolicy.GetDelayBetweenRetries(holder.Prq, numberOfRetries));

                numberOfRetries++;
            }

            Logger.LogMessage(LoggingLevel.Error
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

