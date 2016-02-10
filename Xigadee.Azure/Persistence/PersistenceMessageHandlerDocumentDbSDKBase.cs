#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This persistence handler uses Azure Blob storage as its underlying storage mechanism.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageHandlerDocumentDbSDKBase<K, E>: PersistenceManagerHandlerJsonBase<K, E>
        where K : IEquatable<K>
    {
        DocumentClient mClient;

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
        /// This is the base documentDb collection name.
        /// </summary>
        protected readonly string mCollectionName;

        Func<RepositoryHolder<K, E>, JsonHolder<K>> mJsonMaker;
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
        public PersistenceMessageHandlerDocumentDbSDKBase(DocumentDbConnection connection
            , string database
            , Func<E, K> keyMaker
            , Func<RepositoryHolder<K, E>, JsonHolder<K>> jsonMaker = null
            , string databaseCollection = null
            , string entityName = null
            , VersionPolicy<E> versionMaker = null
            , TimeSpan? defaultTimeout = null
            , ShardingPolicy<K> shardingPolicy = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null) 
            : base(entityName, versionMaker, defaultTimeout, persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile: resourceProfile)
        {
            mConnection = connection;
            mDatabaseName = database;
            mCollectionName = databaseCollection ?? typeof(E).Name;
            mJsonMaker = jsonMaker;
            mKeyMaker = keyMaker;
            mShardingPolicy = shardingPolicy ?? new ShardingPolicy<K>(mCollectionName, (k) => 0, 1, (i) => mCollectionName);
            mClient = null;
        }
        #endregion

        protected virtual async Task<ResponseHolder> ResolveDocumentIdByRef(Tuple<string, string> reference, TimeSpan? timeout = null)
        {
            throw new NotSupportedException("ResolveDocumentIdByRef");
        }

        protected virtual K ResolveKeyFromString(string id)
        {
            throw new NotSupportedException("ResolveKeyFromString");
        }

        #region JsonMaker(PersistenceRepositoryHolder<K, E> rq)
        /// <summary>
        /// This method intercepts the base JSON maker if the fuction has been set in the constructor.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <returns>Returns a JSON holder.</returns>
        protected override JsonHolder<K> JsonMaker(PersistenceRepositoryHolder<K, E> rq)
        {
            if (mJsonMaker == null)
                return base.JsonMaker(rq);

            return mJsonMaker(rq);
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
                mHolders.Add(collection, mConnection.ToCollectionHolder(mDatabaseName, collection, mDefaultTimeout, true));
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

        protected async virtual Task<ResponseHolder> CreateInternal(K key, string json, TimeSpan? timeout = null)
        {
            return await Partition(key).Create(json, timeout);
        }

        #region ProcessCreate
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessCreate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var jsonHolder = JsonMaker(rq);

            var result = await CreateInternal(jsonHolder.Key, jsonHolder.Json, rq.Timeout);

            ProcessOutputEntity(jsonHolder.Key, rs, result);
        }
        #endregion

        #region ProcessRead
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessRead(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var documentId = await ResolveDocumentIdByKey(rq.Key, rq.Timeout);
            await ProcessInternalRead(rq.Key, documentId, rq, rs, prq, prs);
        }
        #endregion
        #region ProcessReadByRef
        /// <summary>
        /// Read By Reference
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessReadByRef(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var documentId = await ResolveDocumentIdByRef(rq.KeyReference, rq.Timeout);
            K key = documentId.IsSuccess ? ResolveKeyFromString(documentId.Id) : default(K);
            await ProcessInternalRead(key, documentId, rq, rs, prq, prs);
        }
        #endregion

        #region ProcessUpdate
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            //409 Conflict
            JsonHolder<K> jsonHolder = JsonMaker(rq);
            JsonHolder<K> jsonHolderUpdate;

            var documentRq = await ResolveDocumentIdByKey(jsonHolder.Key, rq.Timeout);
            if (!documentRq.IsSuccess)
            {
                SetDocumentRetrievalFailure(documentRq, rs);
                return;
            }

            string eTag = documentRq.ETag;

            //We check this in case optimistic locking has been turned on, but old versions don't support this yet.
            if (mVersion.SupportsOptimisticLocking && documentRq.Fields.ContainsKey(mVersion.VersionJsonMetadata.Key))
            {
                var currentVersionId = documentRq.Fields[mVersion.VersionJsonMetadata.Key];
                if (currentVersionId != mVersion.EntityVersionAsString(rq.Entity))
                {
                    rs.ResponseCode = 409;
                    rs.ResponseMessage = "Conflict";
                    rs.Key = jsonHolder.Key;
                    rs.Settings.VersionId = currentVersionId;
                    return;
                }

                //Set the new version id on the entity.
                mVersion.EntityVersionUpdate(rq.Entity);
                jsonHolderUpdate = JsonMaker(rq);
            }
            else
                jsonHolderUpdate = jsonHolder;

            var result = await Partition(jsonHolderUpdate.Key).Update(documentRq.DocumentId, jsonHolderUpdate.Json, rq.Timeout, eTag: eTag);

            if (result.IsSuccess && mVersion.SupportsArchiving)
            {
                //mCollection.Create(documentRq.DocumentId, jsonHolder.Json, rq.Timeout).Result;
            }

            ProcessOutputEntity(jsonHolder.Key, rs, result);
        }
        #endregion

        #region ProcessDelete
        /// <summary>
        /// Delete request.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessDelete(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var documentId = await ResolveDocumentIdByKey(rq.Key, rq.Timeout);

            await ProcessInternalDelete(rq.Key, documentId, rq, rs, prq, prs);
        }
        #endregion
        #region ProcessDeleteByRef
        /// <summary>
        /// Delete by reference request.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessDeleteByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var documentId = await ResolveDocumentIdByRef(rq.KeyReference, rq.Timeout);
            K key = documentId.IsSuccess ? ResolveKeyFromString(documentId.Id) : default(K);
            await ProcessInternalDelete(key, documentId, rq, rs, prq, prs);
        }
        #endregion

        #region ProcessVersion
        /// <summary>
        /// Version.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessVersion(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var documentId = await ResolveDocumentIdByKey(rq.Key, rq.Timeout);
            await ProcessInternalVersion(rq.Key, documentId, rq, rs, prq, prs);
        }
        #endregion
        #region ProcessVersionByRef
        /// <summary>
        /// Version by reference.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task ProcessVersionByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var documentId = await ResolveDocumentIdByRef(rq.KeyReference, rq.Timeout);
            K key = documentId.IsSuccess ? ResolveKeyFromString(documentId.Id) : default(K);
            await ProcessInternalVersion(key, documentId, rq, rs, prq, prs);
        }
        #endregion

        #region ProcessInternalRead
        protected virtual async Task ProcessInternalRead(K key, ResponseHolder documentRq,
            PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            if (!documentRq.IsSuccess)
            {
                SetDocumentRetrievalFailure(documentRq, rs);
                return;
            }

            var result = await Partition(key).Read(documentRq.DocumentId, rq.Timeout);

            ProcessOutputEntity(key, rs, result);
        }
        #endregion
        #region ProcessInternalDelete
        protected virtual async Task ProcessInternalDelete(K key, ResponseHolder documentRq,
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            if (!documentRq.IsSuccess)
            {
                SetDocumentRetrievalFailure(documentRq, rs);
                return;
            }

            string eTag = documentRq.ETag;
            var result = await Partition(key).Delete(documentRq.DocumentId, rq.Timeout, eTag: eTag);

            if (result.IsSuccess)
            {
                //Switch the content over so that we can return the contentId and versionId.
                result.Content = documentRq.Content;
                result.ETag = documentRq.ETag;
            }

            ProcessOutputKey(rs, result);
        }
        #endregion
        #region ProcessInternalVersion
        protected virtual async Task ProcessInternalVersion(K key, ResponseHolder documentRq,
            PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            if (!documentRq.IsSuccess)
            {
                SetDocumentRetrievalFailure(documentRq, rs);
                return;
            }

            var result = await Partition(key).Read(documentRq.DocumentId, rq.Timeout);

            ProcessOutputKey(rs, result);
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
            string id = KeyStringMaker(key);

            var extractions = new List<KeyValuePair<string, string>>();
            extractions.Add(cnJsonMetadata_EntityType);
            if (mVersion.SupportsVersioning)
                extractions.Add(mVersion.VersionJsonMetadata);

            var result = await Partition(key).ResolveDocumentId(id, timeout: timeout, extractionJPaths: extractions);

            return result;
        }
        #endregion

        protected virtual void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rs, ResponseHolder holderResponse)
        {
            if (holderResponse.IsSuccess)
            {
                rs.ResponseCode = (int)holderResponse.Response.StatusCode;
                rs.Entity = EntityMaker(holderResponse.Content);
                rs.Key = KeyMaker(rs.Entity);
                rs.Settings.VersionId = mVersion.EntityVersionAsString(rs.Entity);
                rs.KeyReference = new Tuple<string, string>(rs.Key == null ? null : rs.Key.ToString(), rs.Settings.VersionId);
            }
            else
            {
                rs.IsTimeout = holderResponse.IsTimeout;
                rs.ResponseCode = holderResponse.Ex != null ? 500 : (int)holderResponse.Response.StatusCode;

                if (holderResponse.Ex != null && !rs.IsTimeout)
                    Logger.LogException(string.Format("Error in DocDb persistence {0}-{1}", typeof(E).Name, key), holderResponse.Ex);
                else
                    Logger.LogMessage(
                        rs.IsTimeout ? LoggingLevel.Warning : LoggingLevel.Info,
                        string.Format("Error in DocDb persistence {0}-{1}-{2}-{3}", typeof(E).Name, rs.ResponseCode, key,
                            holderResponse.Ex != null ? holderResponse.Ex.ToString() : rs.ResponseMessage), "DocDb");
            }
        }

        protected virtual void ProcessOutputKey(
            PersistenceRepositoryHolder<K, Tuple<K, string>> rs, ResponseHolder holderResponse)
        {
            if (holderResponse.IsSuccess)
            {
                rs.ResponseCode = (int)holderResponse.Response.StatusCode;
                var entity = EntityMaker(holderResponse.Content);
                rs.Key = KeyMaker(entity);
                string version;
                holderResponse.Fields.TryGetValue(mVersion.VersionJsonMetadata.Key, out version);
                rs.Settings.VersionId = version;
                rs.Entity = new Tuple<K, string>(rs.Key, version);
                rs.KeyReference = new Tuple<string, string>(rs.Key == null ? null : rs.Key.ToString(), version);
            }
            else
            {
                rs.IsTimeout = holderResponse.IsTimeout;
                rs.ResponseCode = 404;
            }
        }

        #region SetDocumentRetrievalFailure<KT,ET>(ResponseHolder documentRq, PersistenceRepositoryHolder<KT,ET> responseHolder)
        /// <summary>
        /// This method sets the correct response for a document retrieval failure.
        /// </summary>
        /// <typeparam name="KT"></typeparam>
        /// <typeparam name="ET"></typeparam>
        /// <param name="documentRq"></param>
        /// <param name="responseHolder"></param>
        protected void SetDocumentRetrievalFailure<KT, ET>(ResponseHolder documentRq, PersistenceRepositoryHolder<KT, ET> responseHolder)
        {
            if (documentRq.IsSuccess)
                return;

            responseHolder.ResponseCode = documentRq.IsTimeout ? 504 : 404;
            responseHolder.ResponseMessage = documentRq.IsTimeout ? "Gateway Timeout" : "Not Found";
            responseHolder.IsTimeout = documentRq.IsTimeout;
        }
        #endregion

        #region TimeoutCorrect

        protected override async Task<bool> TimeoutCorrectCreateUpdate(PersistenceRepositoryHolder<K, E> rq,
            PersistenceRepositoryHolder<K, E> rs, TransmissionPayload m, List<TransmissionPayload> l)
        {
            if (rq.Entity == null)
                return false;

            var jsonHolder = JsonMaker(rq);
            var request = new PersistenceRepositoryHolder<K, E> { Key = jsonHolder.Key, Timeout = rq.Timeout };
            var response = new PersistenceRepositoryHolder<K, E>();

            if (!(await RetrieveEntity(request, response, m, l, ProcessRead)))
                return false;

            rs.Entity = response.Entity;
            rs.Key = response.Key;
            rs.KeyReference = response.KeyReference;
            rs.ResponseCode = !mVersion.SupportsVersioning || jsonHolder.Version.Equals(mVersion.EntityVersionAsString(rs.Entity))
                ? response.ResponseCode
                : rs.ResponseCode;

            return rs.IsSuccess;
        }

        protected override async Task<bool> TimeoutCorrectDelete(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload m, List<TransmissionPayload> l)
        {
            var request = new PersistenceRepositoryHolder<K, E> { Key = rq.Key, KeyReference = rq.KeyReference, Timeout = rq.Timeout };
            var response = new PersistenceRepositoryHolder<K, E>();

            bool result;
            if (request.KeyReference != null && !string.IsNullOrEmpty(request.KeyReference.Item2))
                result = await RetrieveEntity(request, response, m, l, ProcessReadByRef);
            else
                result = await RetrieveEntity(request, response, m, l, ProcessRead);

            if (result)
                return false;

            // We should have a 404 response code here. If not send back the one we got otherwise send back 200 OK
            rs.Key = response.Key;
            rs.KeyReference = response.KeyReference;
            rs.ResponseCode = response.ResponseCode != 404 ? response.ResponseCode : 200;
            return rs.IsSuccess;
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
        protected virtual async Task<bool> RetrieveEntity(
            PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload m, List<TransmissionPayload> l,
            Func<PersistenceRepositoryHolder<K, E>, PersistenceRepositoryHolder<K, E>, TransmissionPayload, List<TransmissionPayload>, Task> readAction)
        {
            int numberOfRetries = 0;
            int maximumRetries = mPersistenceRetryPolicy.GetMaximumRetries(m);
            while (numberOfRetries < maximumRetries && !m.Cancel.IsCancellationRequested)
            {
                await readAction(rq, rs, m, l);

                if (rs.Entity != null || rs.ResponseCode == 404)
                    return rs.Entity != null;

                await Task.Delay(mPersistenceRetryPolicy.GetDelayBetweenRetries(m, numberOfRetries));
                numberOfRetries++;
            }

            Logger.LogMessage(LoggingLevel.Error,
                string.Format(
                    "Unable to retrieve entity after {0} retries, message cancelled({1}) on channel({2}) for request: {3} - response: {4}",
                    numberOfRetries, m.Cancel.IsCancellationRequested, m.Message != null ? m.Message.ChannelPriority.ToString() : "null", rq, rs), "DocDb");

            return false;
        }
    }
    #endregion
}

