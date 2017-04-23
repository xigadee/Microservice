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

using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class contains the specific policy options for the type of logging.
    /// </summary>
    public class AzureStorageDataCollectorPolicy:DataCollectorPolicy
    {
        /// <summary>
        /// This is the Log options.
        /// </summary>
        public AzureStorageDataCollectorOptions Log { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Logger
                , AzureStorageBehaviour.BlobAndTable
                , serializerTable: AzureStorageHelper.ToTableLogEvent
                , binaryMakeId: AzureStorageHelper.LoggerMakeId
                , binaryMakeFolder: AzureStorageHelper.LoggerMakeFolder
                , isSupported: AzureStorageHelper.DefaultLogLevelSupport
                );

        /// <summary>
        /// This is the EventSource options.
        /// </summary>
        public AzureStorageDataCollectorOptions EventSource { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.EventSource
                , AzureStorageBehaviour.Blob
                , binaryMakeId: AzureStorageHelper.EventSourceMakeId
                , binaryMakeFolder: AzureStorageHelper.EventSourceMakeFolder
                );
        /// <summary>
        /// This is the Statistics options. By default encryption is not set for statistics.
        /// </summary>
        public AzureStorageDataCollectorOptions Statistics { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Statistics
                , AzureStorageBehaviour.Blob
                , makeId: AzureStorageHelper.StatisticsMakeId
                , binaryMakeId: AzureStorageHelper.StatisticsMakeId
                , binaryMakeFolder: AzureStorageHelper.StatisticsMakeFolder
                )
            { EncryptionPolicy = AzureStorageEncryption.None};
        /// <summary>
        /// This is the Dispatcher options.
        /// </summary>
        public AzureStorageDataCollectorOptions Dispatcher { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Dispatcher
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableDispatcherEvent
                );
        /// <summary>
        /// This is the Boundary Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Boundary { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Boundary
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableBoundaryEvent
                );
        /// <summary>
        /// This is the Telemetry Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Telemetry { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Telemetry
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableTelemetryEvent
                );
        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Resource { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Resource
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableGeneric
                );

        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Security { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Security
                , AzureStorageBehaviour.Table
                , AzureStorageHelper.ToTableGeneric
                );

        /// <summary>
        /// This is the Custom options.
        /// </summary>
        public AzureStorageDataCollectorOptions Custom { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Custom);

        /// <summary>
        /// This is an enumeration of all the options.
        /// </summary>
        public virtual IEnumerable<AzureStorageDataCollectorOptions> Options
        {
            get
            {
                yield return Log;
                yield return EventSource;
                yield return Statistics;
                yield return Dispatcher;
                yield return Boundary;
                yield return Telemetry;
                yield return Resource;
                yield return Custom;
                yield return Security;
            }
        }


    }
}
