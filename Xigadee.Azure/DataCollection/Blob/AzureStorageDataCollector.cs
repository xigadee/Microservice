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
        protected readonly StorageServiceBase mStorage;
        protected string mServiceName;
        protected readonly ResourceProfile mResourceProfile;
        protected readonly IResourceConsumer mResourceConsumer;
        protected readonly IEncryptionHandler mEncryption;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="containerName"></param>
        /// <param name="serviceName"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="accessType"></param>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="encryption"></param>
        /// <param name="supportMap"></param>
        public AzureStorageDataCollector(StorageCredentials credentials
            , string containerName
            , string serviceName
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , ResourceProfile resourceProfile = null
            , IEncryptionHandler encryption = null
            , DataCollectionSupport? supportMap = null):base(supportMap)
        {
            mStorage = new StorageServiceBase(credentials, containerName, accessType, options, context, defaultTimeout, encryption);

            mServiceName = serviceName;
            mResourceProfile = resourceProfile;
            mEncryption = encryption;
        }
        #endregion

        #region SupportLoadDefault()
        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.BoundaryLogger, (e) => WriteBoundaryEvent((BoundaryEvent)e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => WriteDispatcherEvent((DispatcherEvent)e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => WriteEventSource((EventSourceEvent)e));
            SupportAdd(DataCollectionSupport.Logger, (e) => WriteLogEvent((LogEvent)e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => WriteStatistics((MicroserviceStatistics)e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => WriteTelemetryEvent((TelemetryEvent)e));
            SupportAdd(DataCollectionSupport.Custom, (e) => WriteCustom(e));
        }
        #endregion

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

    }
}
