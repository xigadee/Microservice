#region using
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class encapsulates entity storage functionality for Azure blob storage.
    /// </summary>
    public class StorageServiceBase: ServiceBase<MessagingStatistics>
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

        public const string cnMetaDeleted = "Deleted";
        public const string cnMetaContentId = "ContentId";
        public const string cnMetaVersionId = "VersionId";
        public const string cnDefaultContentType = "application/octet-stream";

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="credentials">The azure credentials</param>
        /// <param name="containerName">The container name to store the entities.</param>
        /// <param name="accessType">The azure storage access type.</param>
        /// <param name="options">The blob request options.</param>
        /// <param name="context">The operation context.</param>
        /// <param name="defaultTimeout">The default timeout for the operations.</param>
        public StorageServiceBase(StorageCredentials credentials
            , string containerName
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , TimeSpan? defaultTimeout = null)
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
                return new BlobRequestOptions() {
                    RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(200), 5)
                    , ServerTimeout = mDefaultTimeout
                    //, ParallelOperationThreadCount = 64 
            };
            }
        } 
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override creates the cloud container.
        /// </summary>
        protected override void StartInternal()
        {
            mStorageAccount = new CloudStorageAccount(mCredentails, true);
            mStorageClient = mStorageAccount.CreateCloudBlobClient();
            if (mOptions != null)
                mStorageClient.DefaultRequestOptions = mOptions;

            mEntityContainer = mStorageClient.GetContainerReference(mContainerName);
            mEntityContainer.CreateIfNotExists(mAccessType, mOptions, mContext);
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// 
        /// </summary>
        protected override void StopInternal()
        {

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
        /// <param name="response">The response to set the necessary parameters.</param>
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
        /// <param name="holder">The storage request holder.</param>
        /// <param name="deleted">A property indicating whether the entity has been deleted.</param>
        protected virtual void MetadataSet(CloudBlockBlob blob, StorageHolderBase holder,
            bool deleted = false)
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
            int start = mStatistics.ActiveIncrement();
            var rs = new StorageResponseHolder();
            try
            {
                var refEntityDirectory = mEntityContainer.GetDirectoryReference(rq.Directory);
                rq.Blob = refEntityDirectory.GetBlockBlobReference(rq.SafeKey);

                bool exists = await rq.Blob.ExistsAsync(rq.CancelSet);
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
                    mStatistics.ErrorIncrement();
                mStatistics.ActiveDecrement(start);
            }

            return rs;
        } 
        #endregion

        #region Create...
        public virtual async Task<StorageResponseHolder> Create(string key, byte[] body,
            string contentType = null, string contentEncoding = null,
            string version = null,
            string directory = null, IEnumerable<KeyValuePair<string, string>> metadata = null,
            CancellationToken? cancel = null)
        {
            var request = new StorageRequestHolder(key, cancel, directory);

            return await CallCloudBlockBlob(request, 
                async (rq, rs, exists) =>
                {
                    if (!exists)
                    {
                        //Check for a soft delete condition.
                        var access = rq.ETag != null ? AccessCondition.GenerateIfMatchCondition(rq.ETag) : AccessCondition.GenerateIfNotExistsCondition();

                        rq.ContentType = contentType;
                        rq.ContentEncoding = contentEncoding;
                        rq.VersionId = version;
                        if (metadata != null)
                            rq.Fields = metadata.ToDictionary((k) => k.Key, (k) => k.Value);
                        MetadataSet(rq.Blob, rq);

                        await rq.Blob.UploadFromByteArrayAsync(body, 0, body.Length,
                            access, mOptions, mContext, rq.CancelSet);

                        MetadataGet(rq.Blob, rs);
                        rs.Data = body;
                    }
                    else
                        rq.CopyTo(rs);

                    rs.IsSuccess = !exists;
                    rs.StatusCode = exists ? 409 : 201;
                });
        }
        #endregion
        #region Update...
        public virtual async Task<StorageResponseHolder> Update(string key, byte[] body,
            string contentType = null, string contentEncoding = null,
            string version = null, string oldVersion = null,
            string directory = null, IEnumerable<KeyValuePair<string, string>> metadata = null,
            CancellationToken? cancel = null, bool createSnapshot = false)
        {
            var request = new StorageRequestHolder(key, cancel, directory);
            return await CallCloudBlockBlob(request,
                async (rq, rs, exists) =>
                {
                    if (!exists)
                    {
                        rq.CopyTo(rs);
                        rs.StatusCode = 404;
                        rs.IsSuccess = false;
                        return;
                    }

                    if (oldVersion != null && oldVersion != rq.VersionId)
                    {
                        MetadataGet(rq.Blob, rs);
                        rs.StatusCode = 409;
                        rs.IsSuccess = false;
                        return;
                    }

                    if (createSnapshot)
                    {
                        await rq.Blob.CreateSnapshotAsync(rq.Fields,
                            AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);
                        MetadataGet(rq.Blob, rq);
                    }

                    rq.ContentType = contentType ?? rs.ContentType;
                    rq.ContentEncoding = contentEncoding ?? rs.ContentEncoding;
                    rq.VersionId = version;
                    MetadataSet(rq.Blob, rq);

                    await rq.Blob.UploadFromByteArrayAsync(body, 0, body.Length,
                        AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);

                    MetadataGet(rq.Blob, rs);
                    rs.Data = body;
                    rs.IsSuccess = true;
                    rs.StatusCode = 200;
                });
        }
        #endregion
        #region CreateOrUpdate...
        public virtual async Task<StorageResponseHolder> CreateOrUpdate(string key, byte[] body,
            string contentType = null, string contentEncoding = null,
            string version = null, string oldVersion = null,
            string directory = null, IEnumerable<KeyValuePair<string, string>> metadata = null,
            CancellationToken? cancel = null, bool createSnapshot = false)
        {
            var request = new StorageRequestHolder(key, cancel, directory);
            var response = await CallCloudBlockBlob(request,
                async (rq, rs, exists) =>
                {
                    if (oldVersion != null && oldVersion != rq.VersionId)
                    {
                        MetadataGet(rq.Blob, rs);
                        rs.StatusCode = 409;
                        rs.IsSuccess = false;
                        return;
                    }

                    if (exists && createSnapshot)
                    {
                        await rq.Blob.CreateSnapshotAsync(rq.Fields,
                            AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);
                        MetadataGet(rq.Blob, rq);
                    }

                    rq.ContentType = contentType ?? rs.ContentType;
                    rq.ContentEncoding = contentEncoding ?? rs.ContentEncoding;
                    rq.VersionId = version;
                    MetadataSet(rq.Blob, rq);

                    await rq.Blob.UploadFromByteArrayAsync(body, 0, body.Length,
                        AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);

                    MetadataGet(rq.Blob, rs);
                    rs.Data = body;

                    rs.IsSuccess = true;
                    rs.StatusCode = 200;
                });

            if (!response.IsSuccess && response.IsTimeout)
                throw new StorageThrottlingException(string.Format("Storage has been throttled for {0}-{1}", directory, key));

            return response;
        }
        #endregion
        #region Read...
        public virtual async Task<StorageResponseHolder> Read(string key, string directory = null, CancellationToken? cancel = null)
        {
            var request = new StorageRequestHolder(key, cancel, directory);
            return await CallCloudBlockBlob(request,
                async (rq, rs, exists) =>
                {
                    if (exists)
                    {
                        var sData = new MemoryStream();

                        await rq.Blob.DownloadToStreamAsync(sData, rq.CancelSet);

                        rs.Data = new byte[sData.Length];
                        sData.Position = 0;
                        sData.Read(rs.Data, 0, rs.Data.Length);
                        MetadataGet(rq.Blob, rs);
                    }
                    else
                        rq.CopyTo(rs);

                    rs.IsSuccess = exists;
                    rs.StatusCode = exists ? 200 : 404;
                });
        }
        #endregion
        #region Version...
        public virtual async Task<StorageResponseHolder> Version(string key, string directory = null, CancellationToken? cancel = null)
        {
            var request = new StorageRequestHolder(key, cancel, directory);
            return await CallCloudBlockBlob(request, 
                async (rq, rs, exists) =>
                {
                    if (exists)
                        MetadataGet(rq.Blob, rs);
                    else
                        rq.CopyTo(rs);
                    rs.IsSuccess = exists;
                    rs.StatusCode = exists ? 200 : 404;
                });
        }
        #endregion
        #region Delete...
        public virtual async Task<StorageResponseHolder> Delete(string key, string directory = null, string version = null
            , CancellationToken? cancel = null
            , bool createSnapshotBeforeDelete = true, bool hardDelete = false)
        {
            var request = new StorageRequestHolder(key, cancel, directory);
            return await CallCloudBlockBlob(request,
                async (rq, rs, exists) =>
                {
                    //Does the entity currently exist?
                    if (!exists)
                    {
                        rq.CopyTo(rs);
                        rs.IsSuccess = false;
                        rs.StatusCode = 404;
                        return;
                    }
                    //OK, do the version match?
                    if (version != null && rq.VersionId != version)
                    {
                        MetadataGet(rq.Blob, rs);
                        rs.IsSuccess = false;
                        rs.StatusCode = 409;
                        return;
                    }

                    if (hardDelete)
                    {
                        //OK, we hard delete the entity and remove any snapshots that may be there.
                        await rq.Blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);
                    }
                    else
                    {
                        if (createSnapshotBeforeDelete)
                        {
                            await rq.Blob.CreateSnapshotAsync(rq.Fields,
                                AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);
                        }
                        //Ok, we are going to soft delete the entity.
                        MetadataSet(rq.Blob, rq, deleted: true);
                        await rq.Blob.SetMetadataAsync(AccessCondition.GenerateIfMatchCondition(rq.ETag), mOptions, mContext, rq.CancelSet);
                    }

                    MetadataGet(rq.Blob, rs);
                    rs.IsSuccess = true;
                    rs.StatusCode = 200;
                });
        } 
        #endregion

    }
}
