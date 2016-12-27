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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to log collection data to the Windows Azure Blob and Table storage.
    /// </summary>
    public class AzureStorageDataCollector: DataCollectorBase<DataCollectorStatistics, AzureStorageDataCollectorPolicy>
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected readonly ResourceProfile mResourceProfile;
        protected readonly IResourceConsumer mResourceConsumer;
        protected readonly IEncryptionHandler mEncryptionHandler;

        protected StorageCredentials mCredentails;
        protected CloudStorageAccount mStorageAccount;

        protected readonly BlobContainerPublicAccessType mAccessType;
        protected readonly BlobRequestOptions mOptions;
        protected readonly OperationContext mContext;
        protected TimeSpan? mDefaultTimeout;

        protected Dictionary<DataCollectionSupport, AzureStorageConnectorBlob> mHoldersBlob;
        protected Dictionary<DataCollectionSupport, AzureStorageConnectorTable> mHoldersTable;
        protected Dictionary<DataCollectionSupport, AzureStorageConnectorQueue> mHoldersQueue;
        protected Dictionary<DataCollectionSupport, AzureStorageConnectorFile> mHoldersFile;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="context"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="encryption"></param>
        /// <param name="supportMap"></param>
        public AzureStorageDataCollector(StorageCredentials credentials
            , AzureStorageDataCollectorPolicy policy = null
            , OperationContext context = null
            , ResourceProfile resourceProfile = null
            , IEncryptionHandler encryption = null
            , DataCollectionSupport? supportMap = null):base(supportMap, policy)
        {
            if (credentials == null)
                throw new ArgumentNullException($"{nameof(AzureStorageDataCollector)}: credentials cannot be null.");

            mCredentails = credentials;

            mContext = context;

            mEncryptionHandler = encryption;

            mResourceProfile = resourceProfile;
        }
        #endregion

        #region Start/Stop...
        /// <summary>
        /// This method creates the storage connectors.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();

            mStorageAccount = new CloudStorageAccount(mCredentails, true);

            //Create the blob client
            mHoldersBlob = Start<AzureStorageConnectorBlob>((o) => o.SupportsBlob());
            //Create the table client
            mHoldersTable = Start<AzureStorageConnectorTable>((o) => o.SupportsTable()
            , (v) => v.Serializer = v.Options.ConnectorTable.Serializer);
            //Create the queue client
            mHoldersQueue = Start<AzureStorageConnectorQueue>((o) => o.SupportsQueue());
            //Create the queue client
            mHoldersFile = Start<AzureStorageConnectorFile>((o) => o.SupportsFile());
        }
        /// <summary>
        /// This method clears the storage connectors.
        /// </summary>
        protected override void StopInternal()
        {
            mHoldersFile.Clear();
            mHoldersQueue.Clear();
            mHoldersTable.Clear();
            mHoldersBlob.Clear();

            mStorageAccount = null;

            base.StopInternal();
        }
        /// <summary>
        /// This method creates a collection of connectors for each specific storage type.
        /// </summary>
        /// <typeparam name="R">The connector type.</typeparam>
        /// <param name="isValid">A function that filters the specific type.</param>
        /// <param name="customize">This method customizes the collector.</param>
        /// <returns>Returns a dictionary containing the support types and their associated connectors.</returns>
        protected virtual Dictionary<DataCollectionSupport,R> Start<R>(Func<AzureStorageDataCollectorOptions, bool> isValid
            , Action<R> customize = null) 
            where R:IAzureStorageConnectorBase,new()
        {
            var holders = mPolicy.Options.Where((o) => isValid(o))
                .ToDictionary((k) => k.Support, (k) => new R()
                {
                      Support = k.Support
                    , Options = k
                    , StorageAccount = mStorageAccount
                    , DefaultTimeout = mDefaultTimeout
                    , Context = mContext
                    , EncryptionHandler = mEncryptionHandler
                });

            if (customize != null)
                holders.Values
                    .ForEach((v) => customize(v));

            //Do we have any supported clients?
            holders.Values
                .ForEach((v) => v.Initialize());

            return holders;
        }
        #endregion

        #region SupportLoadDefault()
        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.Boundary, (e) => Write(mPolicy.Boundary,(BoundaryEvent)e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => Write(mPolicy.Dispatcher, (DispatcherEvent)e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => Write(mPolicy.EventSource, (EventSourceEvent)e));
            SupportAdd(DataCollectionSupport.Logger, (e) => Write(mPolicy.Log, (LogEvent)e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => Write(mPolicy.Statistics, (MicroserviceStatistics)e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => Write(mPolicy.Telemetry, (TelemetryEvent)e));
            SupportAdd(DataCollectionSupport.Resource, (e) => Write(mPolicy.Resource, (ResourceEvent)e));
            SupportAdd(DataCollectionSupport.Custom, (e) => Write(mPolicy.Custom, e));
        }
        #endregion

        #region Profiling ...
        private Guid ProfileStart(string id)
        {
            return mResourceConsumer?.Start(id, Guid.NewGuid()) ?? Guid.NewGuid();
        }

        private void ProfileEnd(Guid profileId, int start, ResourceRequestResult result)
        {
            mResourceConsumer?.End(profileId, start, result);
        }

        private void ProfileRetry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            mResourceConsumer?.Retry(profileId, retryStart, reason);
        }
        #endregion

        #region Write(AzureStorageDataCollectorOptions option, EventBase e)
        /// <summary>
        /// Output the data for the three option types.
        /// </summary>
        /// <param name="option">The storage options</param>
        /// <param name="e">The event object.</param>
        protected void Write(AzureStorageDataCollectorOptions option, EventBase e)
        {
            List<Task> mActions = new List<Task>();

            if (option.SupportsBlob())
                mActions.Add(WriteBlob(option.Support, e));
            if (option.SupportsTable())
                mActions.Add(WriteTable(option.Support, e));
            if (option.SupportsQueue())
                mActions.Add(WriteQueue(option.Support, e));
            if (option.SupportsFile())
                mActions.Add(WriteFile(option.Support, e));

            Task.WhenAll(mActions).Wait();
        }
        #endregion

        #region WriteBlob(AzureStorageDataCollectorOptions option, EventBase e)
        /// <summary>
        /// This method writes the EventBase entity to blob storage.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteBlob(DataCollectionSupport support, EventBase e)
        {
            var option = mHoldersBlob[support]?.Options;
            if (option == null)
                return;

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? (ProfileStart($"AzureBlob_{e.TraceId}")) : default(Guid?);

            var result = ResourceRequestResult.Unknown;
            try
            {
                await option.ConnectorBlob.Write(e, OriginatorId);

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
                //Collector?.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement(option.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(option.Support, start);
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
        #endregion
        #region WriteTable(AzureStorageDataCollectorOptions option, EventBase e)
        /// <summary>
        /// This method transforms the EventBase entity to a Table Storage entity and saves it to the table specified in the options.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteTable(DataCollectionSupport support, EventBase e)
        {
            var option = mHoldersTable[support]?.Options;
            if (option == null)
                return;

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"AzureTable_{e.TraceId}") : default(Guid?);
            var result = ResourceRequestResult.Unknown;
            try
            {
                await option.ConnectorTable.Write(e, OriginatorId);

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
                //Collector?.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement(option.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(option.Support, start);
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
        #endregion
        #region WriteQueue(AzureStorageDataCollectorOptions option, EventBase e)
        /// <summary>
        /// This method writes the EventBase entity to a blob queue.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteQueue(DataCollectionSupport support, EventBase e)
        {
            var option = mHoldersQueue[support]?.Options;
            if (option == null)
                return;

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"AzureQueue_{e.TraceId}") : default(Guid?);
            var result = ResourceRequestResult.Unknown;
            try
            {
                await option.ConnectorQueue.Write(e, OriginatorId);

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
                //Collector?.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement(option.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(option.Support, start);
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
        #endregion
        #region WriteFile(AzureStorageDataCollectorOptions option, EventBase e)
        /// <summary>
        /// This method writes the EventBase entity to a blob queue.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteFile(DataCollectionSupport support, EventBase e)
        {
            var option = mHoldersFile[support]?.Options;
            if (option == null)
                return;

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"AzureFile_{e.TraceId}") : default(Guid?);
            var result = ResourceRequestResult.Unknown;
            try
            {
                await option.ConnectorFile.Write(e, OriginatorId);

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
                //Collector?.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement(option.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(option.Support, start);
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
        #endregion
    }
}
