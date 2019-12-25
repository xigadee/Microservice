using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the repository for Blob storage.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositoryBlobStorage<K, E> : RepositoryBase<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        protected CloudStorageAccount mStorageAccount;
        protected CloudBlobClient mStorageClient;
        protected CloudBlobContainer mEntityContainer;
        protected readonly string mContainerName;
        protected readonly BlobContainerPublicAccessType mAccessType;
        protected readonly BlobRequestOptions mOptions;
        protected readonly OperationContext mContext;
        protected StorageCredentials mCredentails;
        protected TimeSpan? mDefaultTimeout;
        protected readonly IServiceHandlerEncryption mEncryption;

        public const string cnMetaDeleted = "Deleted";
        public const string cnMetaContentId = "ContentId";
        public const string cnMetaVersionId = "VersionId";
        public const string cnDefaultContentType = "application/octet-stream";
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBlobStorage{K, E}"/> class.
        /// </summary>
        /// <param name="credentials">The azure credentials</param>
        /// <param name="containerName">The container name to store the entities.</param>
        /// <param name="accessType">The azure storage access type.</param>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="referenceMaker">The optional entity reference maker.</param>
        /// <param name="propertiesMaker">The optional entity properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyManager">The key manager.</param>
        /// <param name="options">The blob request options.</param>
        /// <param name="context">The operation context.</param>
        /// <param name="defaultTimeout">The default timeout for the operations.</param>
        /// <param name="encryption">Encryption to be used when storing the blob</param>
        /// <exception cref="ArgumentNullException">The sqlConnection cannot be null.</exception>
        public RepositoryBlobStorage(StorageCredentials credentials
            , string containerName
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , Func<E, K> keyMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            , BlobRequestOptions options = null
            , OperationContext context = null
            , TimeSpan? defaultTimeout = null
            , IServiceHandlerEncryption encryption = null
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
            if (credentials == null)
                throw new ArgumentNullException("StorageServiceBase: Storage credentials cannot be null.");

            if (containerName == null)
                throw new ArgumentNullException("StorageServiceBase: Storage containerName cannot be null.");

            mCredentails = credentials;
            mContainerName = ValidateAzureContainerName(containerName);
            mAccessType = accessType;
            mOptions = options ?? BlobRequestOptionsDefault;
            mContext = context;
            mDefaultTimeout = defaultTimeout;
            mEncryption = encryption;

            mStorageAccount = new CloudStorageAccount(mCredentails, true);
            mStorageClient = mStorageAccount.CreateCloudBlobClient();
            if (mOptions != null)
                mStorageClient.DefaultRequestOptions = mOptions;

            mEntityContainer = mStorageClient.GetContainerReference(mContainerName);
            mEntityContainer.CreateIfNotExistsAsync(mAccessType, mOptions, mContext).Wait();
        }
        #endregion

        #region BlobRequestOptionsDefault
        /// <summary>
        /// This method returns the default blob request options.
        /// </summary>
        protected virtual BlobRequestOptions BlobRequestOptionsDefault
        {
            get
            {
                return new BlobRequestOptions()
                {
                    RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(200), 5)
                    ,
                    ServerTimeout = mDefaultTimeout
                    //, ParallelOperationThreadCount = 64 
                };
            }
        }
        #endregion
        #region ValidateAzureContainerName(string container)
        /// <summary>
        /// This method validates that the container name passed is valid.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>Returns a lower case version of the string passed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">This exception is thrown when the string does not pass the azure container criteria.</exception>
        public static string ValidateAzureContainerName(string container)
        {
            var output = container.ToLowerInvariant();

            if (output.Length < 3 || output.Length > 63)
                throw new ArgumentOutOfRangeException("container", "Azure container names can only be between 3 and 63 characters long.");
            if (output.StartsWith("-"))
                throw new ArgumentOutOfRangeException("container", "Azure container names cannot start with a '-' character");
            if (output.Contains("--"))
                throw new ArgumentOutOfRangeException("container", "Azure container names cannot contain multiple consecutive '-' characters");
            if (output.ToCharArray().Count((c) => !(Char.IsLetterOrDigit(c) || c == '-')) > 0)
                throw new ArgumentOutOfRangeException("container", "Azure container names can only contain '0-9' 'A-Z' 'a-z' and the '-' character.");

            return output;
        }
        #endregion

        #region MetadataGet(CloudBlockBlob blob, StorageResponseHolder response)
        /// <summary>
        /// This method gets the metadata for the entity.
        /// </summary>
        /// <param name="blob">The blob.</param>
        /// <param name="holder">The response to set the necessary parameters.</param>
        protected virtual void MetadataGet(CloudBlockBlob blob, StorageHolderBase holder)
        {
            holder.Fields = blob.Metadata.ToDictionary((k) => k.Key, (k) => k.Value);

            if (holder.Fields.ContainsKey(cnMetaContentId))
                holder.Id = holder.Fields[cnMetaContentId];

            if (holder.Fields.ContainsKey(cnMetaVersionId))
                holder.VersionId = holder.Fields[cnMetaVersionId];

            if (blob.Properties.LastModified.HasValue)
                holder.LastUpdated = blob.Properties.LastModified.Value.UtcDateTime;

            if (holder.Fields.ContainsKey(cnMetaDeleted))
                holder.IsDeleted = holder.Fields[cnMetaDeleted] == "true";

            holder.ETag = blob.Properties.ETag;
            holder.ContentType = blob.Properties.ContentType;
            holder.ContentEncoding = blob.Properties.ContentEncoding;
        }
        #endregion
        #region MetadataSet...
        /// <summary>
        /// This method sets the appropriate metadata for an entity.
        /// </summary>
        /// <param name="blob">The blob.</param>
        /// <param name="holder">The storage request holder.</param>
        /// <param name="deleted">A property indicating whether the entity has been deleted.</param>
        protected virtual void MetadataSet(CloudBlockBlob blob
            , StorageHolderBase holder
            , bool deleted = false)
        {
            blob.Metadata.Clear();

            if (holder.Fields != null)
                holder.Fields
                    .Where((k) => !(new[] { cnMetaContentId, cnMetaDeleted }).Contains(k.Key))
                    .ForEach((k) => MetadataSetItemOrCreate(blob, k.Key, k.Value));

            MetadataSetItemOrCreate(blob, cnMetaContentId, holder.Id);

            if (holder.VersionId != null)
                MetadataSetItemOrCreate(blob, cnMetaVersionId, holder.VersionId);

            blob.Properties.ContentType = holder.ContentType ?? cnDefaultContentType;

            blob.Properties.ContentEncoding = holder.ContentEncoding;

            if (deleted)
                MetadataSetItemOrCreate(blob, cnMetaDeleted, "true");
        }
        #endregion
        #region MetadataSetItemOrCreate(CloudBlockBlob blob, string key, string value)
        /// <summary>
        /// This method overwrites or creates a new metadata value for a blob.
        /// </summary>
        /// <param name="blob">The blob.</param>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        private void MetadataSetItemOrCreate(CloudBlockBlob blob, string key, string value)
        {
            if (blob.Metadata.ContainsKey(key))
                blob.Metadata[key] = value;
            else
                blob.Metadata.Add(key, value);
        }
        #endregion

        #region Statistics
        /// <summary>
        /// THis is the internal statistics counter.
        /// </summary>
        public MessagingStatistics Statistics { get; } = new MessagingStatistics();

        #endregion

        #region CallCloudBlockBlob...
        /// <summary>
        /// This is wrapper class that provides generic exception handling support
        /// and retrieves the standard metadata for each request.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="action">The async action task.</param>
        /// <returns>Returns a task with the response.</returns>
        protected async Task<StorageResponseHolder> CallCloudBlockBlob(StorageRequestHolder rq
            , Func<StorageRequestHolder, StorageResponseHolder, bool, Task> action)
        {
            int start = Statistics.ActiveIncrement();
            var rs = new StorageResponseHolder();
            try
            {
                var refEntityDirectory = mEntityContainer.GetDirectoryReference(rq.Directory);
                rq.Blob = refEntityDirectory.GetBlockBlobReference(rq.SafeKey);

                bool exists = await rq.Blob.ExistsAsync();// TODO: Work out why we can't pass the cancellation token: rq.CancelSet);
                if (exists)
                {
                    MetadataGet(rq.Blob, rq);
                    exists ^= rq.IsDeleted;
                }

                await action(rq, rs, exists);
            }
            catch (StorageException sex)
            {
                rs.Ex = sex;
                rs.IsSuccess = false;
                rs.StatusCode = sex.RequestInformation.HttpStatusCode;
                rs.IsTimeout = rs.StatusCode == 500 || rs.StatusCode == 503;
            }
            catch (TaskCanceledException tcex)
            {
                rs.Ex = tcex;
                rs.IsTimeout = true;
                rs.IsSuccess = false;
                rs.StatusCode = 502;
            }
            catch (Exception ex)
            {
                rs.Ex = ex;
                rs.IsSuccess = false;
                rs.StatusCode = 500;
            }
            finally
            {
                if (!rs.IsSuccess)
                    Statistics.ErrorIncrement();
                Statistics.ActiveDecrement(start);
            }

            return rs;
        }
        #endregion


        public override Task<RepositoryHolder<HistoryRequest<K>, HistoryResponse<E>>> History(HistoryRequest<K> key, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        public override Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        public override Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, E>> CreateInternal(K key, E entity, RepositorySettings options, Action<RepositoryHolder<K, E>> holderAction)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRefInternal(string refKey, string refValue, RepositorySettings options = null, Action<RepositoryHolder<K, Tuple<K, string>>> holderAction = null)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, Tuple<K, string>>> DeleteInternal(K key, RepositorySettings options, Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, E>> ReadByRefInternal(string refKey, string refValue, RepositorySettings options, Action<RepositoryHolder<K, E>> holderAction)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, E>> ReadInternal(K key, RepositorySettings options, Action<RepositoryHolder<K, E>> holderAction)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, E>> UpdateInternal(K key, E entity, RepositorySettings options, Action<RepositoryHolder<K, E>> holderAction)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRefInternal(string refKey, string refValue, RepositorySettings options, Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            throw new NotImplementedException();
        }

        protected override Task<RepositoryHolder<K, Tuple<K, string>>> VersionInternal(K key, RepositorySettings options, Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            throw new NotImplementedException();
        }
    }
}
