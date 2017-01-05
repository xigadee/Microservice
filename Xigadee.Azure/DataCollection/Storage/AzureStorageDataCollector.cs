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
            mHoldersBlob = Start<AzureStorageConnectorBlob>((o) => o.SupportsBlob()
            , (v) =>
            {
                v.Serializer = v.Options.SerializerBinary;
                v.MakeId = v.Options.BinaryMakeId;
                v.MakeFolder = v.Options.BinaryMakeFolder;
                ValidateEncryptionPolicy(v);
            });
            //Create the table client
            mHoldersTable = Start<AzureStorageConnectorTable>((o) => o.SupportsTable()
            , (v) =>
            {
                v.Serializer = v.Options.SerializerTable;
                v.MakeId = v.Options.MakeId;
            });
            //Create the queue client
            mHoldersQueue = Start<AzureStorageConnectorQueue>((o) => o.SupportsQueue()
            , (v) =>
            {
                v.Serializer = v.Options.SerializerBinary;
                v.MakeId = v.Options.BinaryMakeId;
                ValidateEncryptionPolicy(v);
            });
            //Create the queue client
            mHoldersFile = Start<AzureStorageConnectorFile>((o) => o.SupportsFile()
            , (v) =>
            {
                v.Serializer = v.Options.SerializerBinary;
                v.MakeId = v.Options.BinaryMakeId;
                ValidateEncryptionPolicy(v);
            });
        }

        /// <summary>
        /// This method validates the encryption policy.
        /// </summary>
        /// <param name="connector">The connector to validate.</param>
        protected virtual void ValidateEncryptionPolicy(IAzureStorageConnectorBase connector)
        {
            if (connector.EncryptionPolicy == AzureStorageEncryption.BlobAlwaysWithException
                && connector.EncryptionHandler == null)
                    throw new AzureStorageDataCollectorEncryptionPolicyException(connector.Support);
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
                    , EncryptionPolicy = k.EncryptionPolicy
                });

            try
            {
                if (customize != null)
                    holders.Values
                        .ForEach((v) => customize(v));

                //Do we have any supported clients?
                holders.Values
                    .ForEach((v) => v.Initialize());
            }
            catch (Exception ex)
            {
                throw;
            }

            return holders;
        }
        #endregion

        #region SupportLoadDefault()
        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.Boundary, (e) => WriteConnectors(DataCollectionSupport.Boundary,e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => WriteConnectors(DataCollectionSupport.Dispatcher, e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => WriteConnectors(DataCollectionSupport.EventSource, e));
            SupportAdd(DataCollectionSupport.Logger, (e) => WriteConnectors(DataCollectionSupport.Logger, e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => WriteConnectors(DataCollectionSupport.Statistics, e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => WriteConnectors(DataCollectionSupport.Telemetry, e));
            SupportAdd(DataCollectionSupport.Resource, (e) => WriteConnectors(DataCollectionSupport.Resource, e));
            SupportAdd(DataCollectionSupport.Custom, (e) => WriteConnectors(DataCollectionSupport.Custom, e));
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

        #region WriteConnectors(DataCollectionSupport support, EventBase e)
        /// <summary>
        /// Output the data for the three option types.
        /// </summary>
        /// <param name="option">The storage options</param>
        /// <param name="e">The event object.</param>
        protected void WriteConnectors(DataCollectionSupport support, EventBase e)
        {
            List<Task> mActions = new List<Task>();
            //Blob
            if (mHoldersBlob.ContainsKey(support) && mHoldersBlob[support].ShouldWrite(e))
                mActions.Add(WriteConnector(mHoldersBlob[support], e));
            //Table
            if (mHoldersTable.ContainsKey(support) && mHoldersTable[support].ShouldWrite(e))
                mActions.Add(WriteConnector(mHoldersTable[support], e));
            //Queue
            if (mHoldersQueue.ContainsKey(support) && mHoldersQueue[support].ShouldWrite(e))
                mActions.Add(WriteConnector(mHoldersQueue[support], e));
            //File
            if (mHoldersFile.ContainsKey(support) && mHoldersFile[support].ShouldWrite(e))
                mActions.Add(WriteConnector(mHoldersFile[support], e));

            Task.WhenAll(mActions).Wait();
        }
        #endregion

        /// <summary>
        /// This method writes the event data to the underlying storage.
        /// </summary>
        /// <param name="connector">The generic connector.</param>
        /// <param name="e">The event to write.</param>
        /// <returns>This is an async process.</returns>
        protected virtual async Task WriteConnector(IAzureStorageConnectorBase connector, EventBase e)
        {
            int start = StatisticsInternal.ActiveIncrement(connector.Support);

            Guid? traceId = connector.Options.ShouldProfile ? (ProfileStart($"Azure{connector.Support}_{e.TraceId}")) : default(Guid?);

            var result = ResourceRequestResult.Unknown;
            try
            {
                await connector.Write(e, OriginatorId);

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
                StatisticsInternal.ErrorIncrement(connector.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(connector.Support, start);
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
    }
}
