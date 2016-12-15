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
    public partial class AzureStorageDataCollector: DataCollectorHolder
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

        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.BoundaryLogger, (e) => WriteBoundaryEvent((BoundaryEvent)e));
            //SupportAdd(DataCollectionSupport.Dispatcher, (e) => EventsDispatcher.Add((DispatcherEvent)e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => WriteEventSource((EventSourceEvent)e));
            SupportAdd(DataCollectionSupport.Logger, (e) => WriteLog((LogEvent)e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => WriteStatistics((MicroserviceStatistics)e));
            //SupportAdd(DataCollectionSupport.Telemetry, (e) => EventsMetric.Add((TelemetryEvent)e));
        }

        protected virtual void WriteLog(LogEvent log)
        {

        }


        protected string DirectoryMaker(LogEvent logEvent)
        {
            string level = Enum.GetName(typeof(LoggingLevel), logEvent.Level);

            return string.Format("{0}/{1}/{2:yyyy-MM-dd}/{2:HH}", mServiceName, level, DateTime.UtcNow);
        }

        protected string IdMaker(LogEvent logEvent)
        {
            if (logEvent is ILogStoreName)
                return ((ILogStoreName)logEvent).StorageId;

            // If there is a category specified and it contains valid digits or characters then make it part of the log name to make it easier to filter log events
            if (!string.IsNullOrEmpty(logEvent.Category) && logEvent.Category.Any(char.IsLetterOrDigit))
                return string.Format("{0}_{1}_{2}", logEvent.GetType().Name, new string(logEvent.Category.Where(char.IsLetterOrDigit).ToArray()), Guid.NewGuid().ToString("N"));

            return string.Format("{0}_{1}", logEvent.GetType().Name, Guid.NewGuid().ToString("N"));
        }



        protected virtual void WriteEventSource(EventSourceEvent log)
        {

        }

        protected virtual void WriteBoundaryEvent(BoundaryEvent log)
        {

        }

        protected virtual void WriteStatistics(MicroserviceStatistics log)
        {

        }



    }
}
