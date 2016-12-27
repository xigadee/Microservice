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
    public partial class AzureStorageDataCollector: DataCollectorBase<DataCollectorStatistics, AzureStorageDataCollectorPolicy>
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected readonly ResourceProfile mResourceProfile;
        protected readonly IResourceConsumer mResourceConsumer;
        protected readonly IEncryptionHandler mEncryption;

        protected CloudStorageAccount mStorageAccount;

        protected readonly BlobContainerPublicAccessType mAccessType;
        protected readonly BlobRequestOptions mOptions;
        protected readonly OperationContext mContext;
        protected StorageCredentials mCredentails;
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

            mEncryption = encryption;

            mResourceProfile = resourceProfile;
        }
        #endregion

        #region Start/Stop...
        protected override void StartInternal()
        {
            base.StartInternal();

            mStorageAccount = new CloudStorageAccount(mCredentails, true);

            //Create the blob client
            StartClientBlob();
            //Create the table client
            StartClientTable();
            //Create the queue client
            StartClientQueue();
            //Create the queue client
            StartClientFile();
        }

        protected override void StopInternal()
        {
            StopClientFile();
            StopClientQueue();
            StopClientTable();
            StopClientBlob();

            mStorageAccount = null;

            base.StopInternal();
        }
        #endregion

        #region Start/Stop Blob...
        protected virtual void StartClientBlob()
        {
            mHoldersBlob = mPolicy.Options.Where((o) => o.SupportsBlob())
                .ToDictionary((k) => k.Support, (k) => new AzureStorageConnectorBlob(mDefaultTimeout) { Support = k.Support, Options = k }); 

            //Do we have any supported clients?
            if (mHoldersBlob.Count == 0)
                return;

            var client = mStorageAccount.CreateCloudBlobClient();
            if (mOptions != null)
                client.DefaultRequestOptions = mOptions;

            foreach (var item in mHoldersBlob.Values)
            {
                //item.Container = client.GetContainerReference(item.RequestOptionsDefault);
                item.Container.CreateIfNotExists(mAccessType, mOptions, mContext);

            }

        }
        protected virtual void StopClientBlob()
        {
            mHoldersBlob.Clear();
        }
        #endregion
        #region Start/Stop Table...
        protected virtual void StartClientTable()
        {
            mHoldersTable = mPolicy.Options.Where((o) => o.SupportsTable())
                .ToDictionary((k) => k.Support, (k) => new AzureStorageConnectorTable() { Support = k.Support, Options = k });

            //Do we have any supported clients?
            if (mHoldersTable.Count == 0)
                return;

            var client = mStorageAccount.CreateCloudTableClient();

            foreach (var item in mHoldersTable.Values)
            {
                item.Client = client;
                item.Table = client.GetTableReference(item.Options.ConnectorTable.RootId);
                item.Table.CreateIfNotExists();
            }
        }
        protected virtual void StopClientTable()
        {
            mHoldersTable.Clear();
        }
        #endregion
        #region Start/Stop Queue...
        protected virtual void StartClientQueue()
        {
            mHoldersQueue = mPolicy.Options.Where((o) => o.SupportsQueue())
                .ToDictionary((k) => k.Support, (k) => new AzureStorageConnectorQueue() { Support = k.Support, Options = k });

            //Do we have any supported clients?
            if (mHoldersQueue.Count == 0)
                return;
            // Create the queue client.
            var client = mStorageAccount.CreateCloudQueueClient();

            foreach (var item in mHoldersQueue.Values)
            {
                item.Client = client;
                item.Queue = client.GetQueueReference(item.Options.QueueId);
                item.Queue.CreateIfNotExists();
            }

            //// Retrieve a reference to a queue.
            //CloudQueue queue = queueClient.GetQueueReference("myqueue");

            //// Create the queue if it doesn't already exist.
            //queue.CreateIfNotExists();

            //// Create a message and add it to the queue.
            //CloudQueueMessage message = new CloudQueueMessage("Hello, World");
            //queue.AddMessage(message);

            // Async enqueue the message
            //await queue.AddMessageAsync(cloudQueueMessage);
            //Console.WriteLine("Message added");
        }
        protected virtual void StopClientQueue()
        {
            mHoldersQueue.Clear();
        }
        #endregion
        #region Start/Stop File...
        protected virtual void StartClientFile()
        {
            mHoldersFile = mPolicy.Options.Where((o) => o.SupportsFile())
                .ToDictionary((k) => k.Support, (k) => new AzureStorageConnectorFile() { Support = k.Support, Options = k });

            //Do we have any supported clients?
            if (mHoldersFile.Count == 0)
                return;

            // Create the queue client.
            var client = mStorageAccount.CreateCloudFileClient();
            
            foreach (var item in mHoldersFile.Values)
            {
                item.Client = client;
                //item.Queue = client.getf(item.Options.QueueId);
                //item.Queue.CreateIfNotExists();
            }
        }
        protected virtual void StopClientFile()
        {
            mHoldersFile.Clear();
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

            AzureStorageContainerBlob cont = option.BlobConverter(OriginatorId, e);

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? (ProfileStart($"{cont.Directory}/{cont.Id}")) : default(Guid?);

            var result = ResourceRequestResult.Unknown;
            try
            {
 

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

            AzureStorageContainerTable cont = option.TableConverter?.Invoke(OriginatorId, e) ?? e.DefaultTableConverter(OriginatorId);

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"{cont.Id}") : default(Guid?);
            var result = ResourceRequestResult.Unknown;
            try
            {


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

            AzureStorageContainerQueue cont = option.QueueConverter?.Invoke(OriginatorId, e) ?? e.QueueConverterDefault(OriginatorId);

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"{cont.Id}") : default(Guid?);
            var result = ResourceRequestResult.Unknown;
            try
            {


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

            AzureStorageContainerQueue cont = option.QueueConverter?.Invoke(OriginatorId, e) ?? e.QueueConverterDefault(OriginatorId);

            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"{cont.Id}") : default(Guid?);
            var result = ResourceRequestResult.Unknown;
            try
            {
                //Currently do nothing

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
