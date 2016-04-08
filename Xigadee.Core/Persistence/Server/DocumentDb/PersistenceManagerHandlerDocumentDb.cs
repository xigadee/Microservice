#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    #region PersistenceManagerHandlerDocumentDb<K, E>
    /// <summary>
    /// This is the REST based documentDb persistence handler class.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class PersistenceManagerHandlerDocumentDb<K, E>: PersistenceManagerHandlerDocumentDb<K, E, PersistenceStatistics>
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
        public PersistenceManagerHandlerDocumentDb(DocumentDbConnection connection
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
    /// This is the REST based documentDb persistence handler class.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The statistics class.</typeparam>
    public class PersistenceManagerHandlerDocumentDb<K, E, S> : PersistenceManagerHandlerJsonBase<K, E, S, PersistenceCommandPolicy>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Declarations
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
        public PersistenceManagerHandlerDocumentDb(DocumentDbConnection connection
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
            mShardingPolicy = shardingPolicy??new ShardingPolicy<K>(mCollectionName, (k) => 0, 1, (i) => mCollectionName);
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override creates the document db databaseId and collection if they don't already exist.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();

            mHolders = new Dictionary<string, CollectionHolder>();
            foreach (var collection in mShardingPolicy.Collections)
            {
                mHolders.Add(collection, mConnection.ToCollectionHolder(mDatabaseName, collection, mPolicy.DefaultTimeout, true));
            }
        }
        #endregion
        #region Partition(K key)
        /// <summary>
        /// This method uses the sharding policy to determine the appropriate collection for the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns the documentDb collection.</returns>
        protected virtual Collection Partition(K key)
        {
            var collection = mShardingPolicy.Resolve(key);
            return mHolders[collection].Collection;
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
                return new PersistenceResponseHolder<E>() { StatusCode = result.StatusCode, Content = result.Content, IsSuccess = true, Entity = mTransform.EntityDeserializer(result.Content) };
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
            var jsonHolder = mTransform.JsonMaker(holder.rq.Entity);

            var result = await Partition(jsonHolder.Key).Create(jsonHolder.Json, holder.rq.Timeout);

            return PersistenceResponseFormat(result);
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
            var documentRq = await ResolveDocumentIdByKey(key, holder.rq.Timeout);

            if (!documentRq.IsSuccess)
                return PersistenceResponseFormat(documentRq);

            var result = await Partition(key).Read(documentRq.DocumentId, holder.rq.Timeout);

            return PersistenceResponseFormat(result);
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
            //409 Conflict
            JsonHolder<K> jsonHolder = mTransform.JsonMaker(holder.rq.Entity);
            JsonHolder<K> jsonHolderUpdate;

            var documentRq = await ResolveDocumentIdByKey(jsonHolder.Key, holder.rq.Timeout);
            if (!documentRq.IsSuccess)
                return PersistenceResponseFormat(documentRq);

            string eTag = documentRq.ETag;

            //We check this in case optimistic locking has been turned on, but old versions don't support this yet.
            if (mTransform.Version.SupportsOptimisticLocking && documentRq.Fields.ContainsKey(mTransform.Version.VersionJsonMetadata.Key))
            {
                var currentVersionId = documentRq.Fields[mTransform.Version.VersionJsonMetadata.Key];

                if (currentVersionId != mTransform.Version.EntityVersionAsString(holder.rq.Entity))
                    return new PersistenceResponseHolder<E>() { StatusCode = 409, IsSuccess = false, IsTimeout = false, VersionId = currentVersionId};

                //Set the new version id on the entity.
                mTransform.Version.EntityVersionUpdate(holder.rq.Entity);
                jsonHolderUpdate = mTransform.JsonMaker(holder.rq.Entity);
            }
            else
                jsonHolderUpdate = jsonHolder;

            var result = await Partition(jsonHolderUpdate.Key).Update(documentRq.DocumentId, jsonHolderUpdate.Json, holder.rq.Timeout, eTag: eTag);

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
            var documentId = await ResolveDocumentIdByKey(holder.rq.Key, holder.rq.Timeout);

            if (!documentId.IsSuccess)
                return PersistenceResponseFormat(documentId);

            string eTag = documentId.ETag;
            var result = await Partition(holder.rq.Key).Delete(documentId.DocumentId, holder.rq.Timeout, eTag: eTag);

            if (result.IsSuccess)
            {
                //Switch the content over so that we can return the contentId and versionId.
                result.Content = documentId.Content;
                result.ETag = documentId.ETag;
            }

            return PersistenceResponseFormat(result);
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
            var documentId = await ResolveDocumentIdByKey(holder.rq.Key, holder.rq.Timeout);

            if (!documentId.IsSuccess)
                return PersistenceResponseFormat(documentId);

            var result = await Partition(holder.rq.Key).Read(documentId.DocumentId, holder.rq.Timeout);

            return PersistenceResponseFormat(result);
        }
        #endregion

        #region ResolveDocumentIdByKey(K key)
        /// <summary>
        /// This method resolves an entity id against the internal DocumentDb _rid value.
        /// </summary>
        /// <param name="key">The key to resolve.</param>
        /// <param name="timeout"></param>
        /// <returns>Returns the _rid value.</returns>
        protected virtual async Task<ResponseHolder> ResolveDocumentIdByKey(K key, TimeSpan? timeout = null)
        {
            string id = mTransform.KeyStringMaker(key);

            var extractions = new List<KeyValuePair<string, string>>();
            extractions.Add(mTransform.JsonMetadata_EntityType);
            if (mTransform.Version.SupportsVersioning)
                extractions.Add(mTransform.Version.VersionJsonMetadata);

            var result = await Partition(key).ResolveDocumentId(id, timeout: timeout, extractionJPaths: extractions);

            return result;
        }
        #endregion

        protected override void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, 
            IResponseHolder holderResponse)
        {
            if (holderResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(holderResponse.Content))
                {
                    var entity = mTransform.EntityDeserializer(holderResponse.Content);
                    rq.Key = mTransform.KeyMaker(entity);
                    holderResponse.VersionId = mTransform.Version?.EntityVersionAsString(entity);
                }
                else
                {
                    rq.Key = mTransform.KeyDeserializer(holderResponse.Id);
                }
            }

            base.ProcessOutputKey(rq,rs, holderResponse);
        }

        #region TimeoutCorrect

        protected override async Task<bool> TimeoutCorrectCreateUpdate(PersistenceRequestHolder<K, E> holder)
        {
            if (holder.rq.Entity == null)
                return false;

            var jsonHolder = mTransform.JsonMaker(holder.rq.Entity);
            var request = new PersistenceRepositoryHolder<K, E> {Key = jsonHolder.Key, Timeout = holder.rq.Timeout};
            var response = new PersistenceRepositoryHolder<K, E>();

            if (!(await RetrieveEntity(holder, ProcessRead)))
                return false;

            holder.rs.Entity = response.Entity;
            holder.rs.Key = response.Key;
            holder.rs.KeyReference = response.KeyReference;
            holder.rs.ResponseCode = !mTransform.Version.SupportsVersioning || jsonHolder.Version.Equals(mTransform.Version.EntityVersionAsString(holder.rs.Entity))
                ? response.ResponseCode
                : holder.rs.ResponseCode;

            return holder.rs.IsSuccess;
        }

        protected override async Task<bool> TimeoutCorrectDelete(PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {

            var alternateHolder = new PersistenceRequestHolder<K, E>(holder.profileId, holder.prq, holder.prs);
            alternateHolder.rq = new PersistenceRepositoryHolder<K, E> { Key = holder.rq.Key, KeyReference = holder.rq.KeyReference, Timeout = holder.rq.Timeout };
            alternateHolder.rs = new PersistenceRepositoryHolder<K, E>();

            bool byref = alternateHolder.rq.KeyReference != null && !string.IsNullOrEmpty(alternateHolder.rq.KeyReference.Item2);

            bool result;
            if (byref)
                result = await RetrieveEntity(alternateHolder, ProcessReadByRef);
            else
                result = await RetrieveEntity(alternateHolder, ProcessRead);

            if (result)
                return false;

            // We should have a 404 response code here. If not send back the one we got otherwise send back 200 OK
            holder.rs.Key = alternateHolder.rs.Key;
            holder.rs.KeyReference = alternateHolder.rs.KeyReference;
            holder.rs.ResponseCode = alternateHolder.rs.ResponseCode != 404 ? alternateHolder.rs.ResponseCode : 200;

            return holder.rs.IsSuccess;
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

            int maximumRetries = mPolicy.PersistenceRetryPolicy.GetMaximumRetries(holder.prq);

            while (numberOfRetries < maximumRetries && !holder.prq.Cancel.IsCancellationRequested)
            {
                await readAction(holder);

                if (holder.rs.Entity != null || holder.rs.ResponseCode == 404)
                    return holder.rs.Entity != null;

                await Task.Delay(mPolicy.PersistenceRetryPolicy.GetDelayBetweenRetries(holder.prq, numberOfRetries));

                numberOfRetries++;
            }

            Logger.LogMessage(LoggingLevel.Error
                , string.Format(
                    "Unable to retrieve entity after {0} retries, message cancelled({1}) on channel({2}) for request: {3} - response: {4}"
                    , numberOfRetries
                    , holder.prq.Cancel.IsCancellationRequested
                    , holder.prq.Message != null ? holder.prq.Message.ChannelPriority.ToString() : "null"
                    , holder.rq
                    , holder.rs)
                , "DocDb"
                );

            return false;
        }
    }
    #endregion
}
