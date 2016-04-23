#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class supports provides generic logging support for Azure Blob Storage.
    /// </summary>
    public abstract class AzureStorageLoggingBase<E> : ServiceBase<LoggingStatistics>, IServiceLogger, IRequireSharedServices
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected StorageServiceBase mStorage;

        protected Func<E, string> mIdMaker;
        protected Func<E, string> mDirectoryMaker;

        protected string mServiceName;

        protected readonly ResourceProfile mResourceProfile;
        protected IResourceConsumer mResourceConsumer;
        #endregion
        #region Constructor

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="credentials">The azure storage credentials.</param>
        /// <param name="directoryMaker"></param>
        /// <param name="defaultTimeout">The default timeout for async requests.</param>
        /// <param name="accessType">The azure access type. BlobContainerPublicAccessType.Off is the default.</param>
        /// <param name="options">The optional blob request options.</param>
        /// <param name="context">The optional operation context.</param>
        /// <param name="containerName"></param>
        /// <param name="serviceName"></param>
        /// <param name="idMaker"></param>
        /// <param name="resourceProfile"></param>
        protected AzureStorageLoggingBase(StorageCredentials credentials
            , string containerName
            , string serviceName
            , Func<E, string> idMaker = null
            , Func<E, string> directoryMaker = null
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , ResourceProfile resourceProfile = null)
        {
            mStorage = new StorageServiceBase(credentials, containerName, accessType, options, context, defaultTimeout: defaultTimeout);
            mIdMaker = idMaker ?? IdMaker;
            mDirectoryMaker = directoryMaker ?? DirectoryMaker;
            mServiceName = serviceName;
            mResourceProfile = resourceProfile;
        }
        #endregion
        public ILoggerExtended Logger { get; set; }

        #region SharedServices
        /// <summary>
        /// This is the shared service connector
        /// </summary>
        public ISharedService SharedServices
        {
            get; set;
        }
        #endregion

        protected abstract string IdMaker(E data);

        protected abstract string DirectoryMaker(E data);

        /// <summary>
        /// This method serializes the event source in to the specifed binary format.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns>Returns a byte array.</returns>
        protected virtual byte[] Serialize(E entity, out string contentType, out string contentEncoding)
        {
            contentType = "application/json; charset=utf-8";
            contentEncoding = null;
            var jObj = JObject.FromObject(entity);
            var body = jObj.ToString();
            return Encoding.UTF8.GetBytes(body);
        }

        protected async Task Output(string id, string directory, E entity)
        {
            int start = StatisticsInternal.ActiveIncrement();
            Guid traceId = ProfileStart(id, directory);
            var result = ResourceRequestResult.Unknown;
            try
            {
                string contentType, contentEncoding;
                var blob = Serialize(entity, out contentType, out contentEncoding);

                await mStorage.CreateOrUpdate(id, blob
                    , directory: directory
                    , contentType: contentType
                    , contentEncoding: contentEncoding
                    , createSnapshot: false);

                result = ResourceRequestResult.Success;
            }
            catch (StorageThrottlingException)
            {
                result = ResourceRequestResult.Exception;
                throw;
            }
            catch (Exception ex)
            {
                result = ResourceRequestResult.Exception;
                Logger.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(start);
                ProfileEnd(traceId, start, result);
            }
        }

        protected async Task Output(string id, string directory, byte[] blob, string contentType, string contentEncoding = null)
        {
            int start = StatisticsInternal.ActiveIncrement();
            Guid traceId = ProfileStart(id, directory);
            var result = ResourceRequestResult.Unknown;
            try
            {
                await mStorage.CreateOrUpdate(id, blob
                    , directory: directory
                    , contentType: contentType
                    , contentEncoding: contentEncoding
                    , createSnapshot: false);
                result = ResourceRequestResult.Success;
            }
            catch (StorageThrottlingException)
            {
                result = ResourceRequestResult.Exception;
                throw;
            }
            catch (Exception ex)
            {
                result = ResourceRequestResult.Exception;
                Logger.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, contentType), ex);
                StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                ProfileEnd(traceId, start, result);
                StatisticsInternal.ActiveDecrement(start);
            }
        }

        protected virtual Guid ProfileStart(string id, string directory)
        {
            if (mResourceConsumer == null)
                return Guid.NewGuid();

            return mResourceConsumer.Start(string.Format("{0}/{1}", directory, id), Guid.NewGuid());
        }

        protected virtual void ProfileEnd(Guid profileId, int start, ResourceRequestResult result)
        {
            if (mResourceConsumer == null)
                return;

            mResourceConsumer.End(profileId, start, result);
        }

        protected virtual void ProfileRetry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            if (mResourceConsumer == null)
                return;

            mResourceConsumer.Retry(profileId, retryStart, reason);
        }


        #region Start/Stop Internal
        protected override void StartInternal()
        {
            mStorage.Start();
            var resourceTracker = SharedServices.GetService<IResourceTracker>();
            if (resourceTracker != null && mResourceProfile != null)
                mResourceConsumer = resourceTracker.RegisterConsumer(GetType().Name, mResourceProfile);
        }
        protected override void StopInternal()
        {
            mStorage.Stop();
            mResourceConsumer = null;
        }
        #endregion
    }
}
