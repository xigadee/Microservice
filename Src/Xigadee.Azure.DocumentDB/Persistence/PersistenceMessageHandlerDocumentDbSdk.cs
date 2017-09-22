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
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
#endregion
namespace Xigadee
{
    #region PersistenceMessageHandlerDocumentDbSdk<K, E>
    /// <summary>
    /// This is the naitve documentDb persistence handler class.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageHandlerDocumentDbSdk<K, E>: PersistenceMessageHandlerDocumentDbSdk<K, E, PersistenceStatistics, DocumentDbPersistenceCommandPolicy>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="connection">The documentDb connection.</param>
        /// <param name="database">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
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
            : base(connection, database, keyMaker, keyDeserializer
                  , databaseCollection: databaseCollection
                  , shardingPolicy: shardingPolicy
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker: referenceMaker
                  , keySerializer: keySerializer
                  )
        {
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// This persistence handler uses Azure Blob storage as its underlying storage mechanism.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageHandlerDocumentDbSdk<K, E, S, P>: PersistenceManagerHandlerDocumentDbBase<K, E, S, P>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : DocumentDbPersistenceCommandPolicy, new()
    {
        #region Declarations
        /// <summary>
        /// This is the client used to communicate with the DocumentDb service.
        /// </summary>
        DocumentClient mClient;
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
            : base(connection, database, databaseCollection: databaseCollection, shardingPolicy: shardingPolicy
                  , entityName: entityName
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
            mClient = connection.ToDocumentClient();
        }
        #endregion

        #region CreateDatabase()
        /// <summary>
        /// This method will create the database on the server.
        /// </summary>
        protected override void CreateDatabase()
        {
            try
            {
                var db = mClient.CreateDatabaseAsync(new Microsoft.Azure.Documents.Database { Id = mDatabaseName }).Result;
            }
            catch (Exception ex)
            {
                var inEx = ex.InnerException as DocumentClientException;
                if (inEx == null || inEx.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    throw;
                }
            }
        }
        #endregion
        #region CreateCollections()
        /// <summary>
        /// This method creates the appropriate collections based on the sharding policy.
        /// </summary>
        protected override void CreateCollections()
        {
            var uri = UriFactory.CreateDatabaseUri(mDatabaseName);
            foreach (var collection in mShardingPolicy.Collections)
            {
                try
                {
                    var coll = mClient.CreateDocumentCollectionAsync(uri, new DocumentCollection { Id = collection }).Result;
                }
                catch (Exception ex)
                {
                    var inEx = ex.InnerException as DocumentClientException;
                    if (inEx == null || inEx.StatusCode != System.Net.HttpStatusCode.Conflict)
                    {
                        throw;
                    }
                }
            }
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
            string stringKey = mTransform.KeyStringMaker(key);
            var response = await mClient.ReadGeneric(mDatabaseName, mShardingPolicy.Resolve(key), stringKey);
            response.Id = mTransform.KeySerializer(key);

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

                if (!resultRead.IsSuccess)
                    return new PersistenceResponseHolder<E>() { StatusCode = resultRead.StatusCode, IsSuccess = false, IsTimeout = false};

                E entityCurrent = mTransform.PersistenceEntitySerializer.Deserializer(resultRead.Content);

                if (mTransform.Version.EntityVersionAsString(entityCurrent) != jsonHolder.Version)
                {
                    return new PersistenceResponseHolder<E>() { StatusCode = 409, IsSuccess = false, IsTimeout = false, VersionId = jsonHolder.Version };
                }

                eTag = resultRead.ETag;

                //Set the new version id on the entity.
                mTransform.Version.EntityVersionUpdate(holder.Rq.Entity);

                jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);
            }

            var result = await mClient.UpdateGeneric(jsonHolder.Json, mDatabaseName, collection, eTag);

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
            string stringKey = mTransform.KeyStringMaker(key);
            var response = await mClient.DeleteGeneric(mDatabaseName, mShardingPolicy.Resolve(key), stringKey);
            response.Id = mTransform.KeySerializer(key);

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
            string stringKey = mTransform.KeyStringMaker(key);
            var response = await mClient.ReadGeneric(mDatabaseName, mShardingPolicy.Resolve(key), stringKey);
            response.Id = mTransform.KeySerializer(key);       

            return PersistenceResponseFormat(response);
        }
        #endregion

        /// <summary>
        /// This is the specific search implementation for the DocumentDb database
        /// </summary>
        /// <param name="holder"></param>
        /// <returns></returns>
        protected override Task ProcessSearch(PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {          
            holder.Rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
            holder.Rs.ResponseMessage = "Search is not implemented.";
            return Task.FromResult(0);
        }

        #region PersistenceResponseFormat(ResponseHolder result)
        /// <summary>
        /// This method sets the response holder based on the results holder.
        /// </summary>
        /// <param name="result">The documentDb result holder.</param>
        /// <returns>Returns the persistence holder.</returns>
        private PersistenceResponseHolder<E> PersistenceResponseFormat(ResponseHolder result)
        {
            if (result.IsSuccess)
            {
                if (result.Content != null)
                    return new PersistenceResponseHolder<E>() { Id = result.Id, StatusCode = result.StatusCode, Content = result.Content, IsSuccess = true, Entity = mTransform.PersistenceEntitySerializer.Deserializer(result.Content), VersionId = result.VersionId };
                else
                    return new PersistenceResponseHolder<E>() { Id = result.Id, StatusCode = result.StatusCode, IsSuccess = true, VersionId = result.VersionId };
            }
            else
                return new PersistenceResponseHolder<E>() { Id = result.Id, StatusCode = result.IsTimeout ? 504 : result.StatusCode, IsSuccess = false, IsTimeout = result.IsTimeout };
        }
        #endregion

        #region ProcessOutputKey ...
        /// <summary>
        /// This override enables the key and version id to be returned from the JSON entity.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="holderResponse"></param>
        protected override void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq
            , PersistenceRepositoryHolder<K, Tuple<K, string>> rs
            , IResponseHolder holderResponse)
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
        #endregion        

    }
}

