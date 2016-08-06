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
    public class PersistenceMessageHandlerAzureBlobStorageBase<K, E> : PersistenceManagerHandlerJsonBase<K, E, PersistenceStatistics, PersistenceCommandPolicy>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected StorageServiceBase mStorage;

        protected Func<K, string> mStorageIdMaker;

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
            , Func<string, K> keyDeserializer
            , Func<K, string> storageIdMaker = null
            , Func<K, string> keySerializer = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            )
            : base( keyMaker, keyDeserializer
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy:persistenceRetryPolicy
                  , resourceProfile:resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker:referenceMaker
                  , keySerializer: keySerializer
                  )
        {
            mDirectory = entityName ?? typeof(E).Name;
            mStorage = new StorageServiceBase(credentials, "persistence", accessType, options, context, defaultTimeout: defaultTimeout);
            mStorageIdMaker = storageIdMaker ?? mTransform.KeySerializer;
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override starts the storage agent.
        /// </summary>
        protected override void StartInternal()
        {
            mStorage.Start();
            base.StartInternal();
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override stops the storage agent.
        /// </summary>
        protected override void StopInternal()
        {
            base.StopInternal();
            mStorage.Stop();
        }
        #endregion

        private PersistenceResponseHolder<E> PersistenceResponseFormat(StorageResponseHolder result)
        {
            if (result.IsSuccess)
            {
                if (result.Content != null)
                    return new PersistenceResponseHolder<E>() { StatusCode = result.StatusCode, Content = result.Content, IsSuccess = true, Entity = mTransform.PersistenceEntitySerializer.Deserializer(result.Content), VersionId = result.VersionId };
                else
                    return new PersistenceResponseHolder<E>() { StatusCode = result.StatusCode, IsSuccess = true, VersionId = result.VersionId };
            }
            else
                return new PersistenceResponseHolder<E>() { StatusCode = result.IsTimeout ? 504 : result.StatusCode, IsSuccess = false, IsTimeout = result.IsTimeout };
        }

        #region InternalCreate
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);

            var result = await mStorage.Create(mStorageIdMaker(jsonHolder.Key)
                , jsonHolder.ToBlob()
                , contentType: "application/json; charset=utf-8"
                , version: jsonHolder.Version, directory: mDirectory);

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalRead
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            var result = await mStorage.Read(mStorageIdMaker(holder.Rq.Key), directory: mDirectory);

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalUpdate
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var jsonHolder = mTransform.JsonMaker(holder.Rq.Entity);

            var result = await mStorage.Update(mStorageIdMaker(jsonHolder.Key)
                , jsonHolder.ToBlob()
                , contentType: "application/json; charset=utf-8"
                , version: jsonHolder.Version, directory: mDirectory);

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
        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var result = await mStorage.Delete(mStorageIdMaker(holder.Rq.Key), directory: mDirectory);

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
            var result = await mStorage.Version(mStorageIdMaker(holder.Rq.Key), directory: mDirectory);

            return PersistenceResponseFormat(result);
        }

        #endregion
    }
}
