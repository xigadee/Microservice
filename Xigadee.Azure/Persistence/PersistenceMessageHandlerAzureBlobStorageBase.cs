#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This persistence handler uses Azure Blob storage as its underlying storage mechanism.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageHandlerAzureBlobStorageBase<K, E> : PersistenceManagerHandlerJsonBase<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected StorageServiceBase mStorage;

        protected Func<K, string> mIdMaker;

        protected string mDirectory;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="credentials">The azure storage credentials.</param>
        /// <param name="entityName">The options entity name. If this is not presented then the entity name will be used.</param>
        /// <param name="versionPolicy">The versioning policy.</param>
        /// <param name="defaultTimeout">The default timeout for async requests.</param>
        /// <param name="accessType">The azure access type. BlobContainerPublicAccessType.Off is the default.</param>
        /// <param name="options">The optional blob request options.</param>
        /// <param name="context">The optional operation context.</param>
        /// <param name="retryPolicy">Persistence retry policy</param>
        public PersistenceMessageHandlerAzureBlobStorageBase(StorageCredentials credentials
            , Func<E, K> keyMaker
            , Func<K, string> idMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null)
            : base(entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy:persistenceRetryPolicy, resourceProfile:resourceProfile, cacheManager: cacheManager)
        {
            mDirectory = entityName ?? typeof(E).Name;
            mStorage = new StorageServiceBase(credentials, "persistence", accessType, options, context, defaultTimeout: defaultTimeout);
            mKeyMaker = keyMaker;
            mIdMaker = idMaker;
        }
        #endregion

        protected override string KeyStringMaker(K key)
        {
            return mIdMaker(key);
        }

        #region StartInternal()
        /// <summary>
        /// This override starts the storage agent.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();
            mStorage.Start();
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override stops the storage agent.
        /// </summary>
        protected override void StopInternal()
        {
            mStorage.Stop();
            base.StopInternal();
        }
        #endregion

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
            var blob = Encoding.UTF8.GetBytes(jsonHolder.Json);

            var result = await mStorage.Create(jsonHolder.Id, blob
                , contentType: "application/json; charset=utf-8"
                , version: jsonHolder.Version, directory: mDirectory);

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
            var result = await mStorage.Read(mIdMaker(rq.Key), directory: mDirectory);

            ProcessOutputEntity(rq.Key, rs, result);
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
            var jsonHolder = JsonMaker(rq);
            var blob = Encoding.UTF8.GetBytes(jsonHolder.Json);

            var result = await mStorage.Update(jsonHolder.Id, blob
                , contentType: "application/json; charset=utf-8"
                , version: jsonHolder.Version, directory: mDirectory);

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
            var result = await mStorage.Delete(mIdMaker(rq.Key), directory: mDirectory);

            ProcessOutputKey(rq, rs, result);
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
            var result = await mStorage.Version(mIdMaker(rq.Key), directory: mDirectory);

            ProcessOutputKey(rq, rs, result);
        }
        #endregion

        //Unsupported
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
            rs.ResponseCode = 405;
            rs.ResponseMessage = "Read by reference not supported";
            return;
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
            rs.ResponseCode = 405;
            rs.ResponseMessage = "Read by reference not supported";
            return;
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
            rs.ResponseCode = 405;
            rs.ResponseMessage = "Read by reference not supported";
            return;
        }
        #endregion

        protected override void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rs, IResponseHolder holderResponse)
        {
            if (holderResponse.IsSuccess)
            {
                rs.ResponseCode = holderResponse.StatusCode;

                OutputEntitySet(rs, holderResponse.Content);
            }
            else
            {
                rs.IsTimeout = holderResponse.IsTimeout;
                rs.ResponseCode = holderResponse.Ex != null ? 500 : holderResponse.StatusCode;

                if (holderResponse.Ex != null)
                    Logger.LogException(
                        string.Format("Error in blob storage persistence {0}-{1}", typeof(E).Name, rs.Key), holderResponse.Ex);
                else
                    Logger.LogMessage(LoggingLevel.Warning, 
                        string.Format("Error in blob storage persistence {0}-{1}/{2}-{3}", typeof(E).Name, rs.Key, rs.ResponseCode, rs.ResponseMessage), "BlobStorage");
            }
        }

        protected override void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq,
            PersistenceRepositoryHolder<K, Tuple<K, string>> rs, IResponseHolder holderResponse)
        {
            rs.Key = rq.Key;

            if (holderResponse.IsSuccess)
            {
                rs.ResponseCode = holderResponse.StatusCode;
                string version;
                holderResponse.Fields.TryGetValue(StorageServiceBase.cnMetaVersionId, out version);
                rs.Settings.VersionId = version;
                rs.Entity = new Tuple<K, string>(rs.Key, version);
            }
            else
            {
                rs.IsTimeout = holderResponse.IsTimeout;
                rs.ResponseCode = 404;
            }
        }
    }
}
