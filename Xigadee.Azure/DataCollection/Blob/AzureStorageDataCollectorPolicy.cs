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
using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    /// <summary>
    /// This class contains the specific policy options for the type of logging.
    /// </summary>
    public class AzureStorageDataCollectorPolicy:DataCollectorPolicy
    {
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions Log { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Logger
                , AzureStorageDataCollectorOptions.StorageBehaviour.Blob);
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions EventSource { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.EventSource
                , AzureStorageDataCollectorOptions.StorageBehaviour.Blob);
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions Statistics { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Statistics
                , AzureStorageDataCollectorOptions.StorageBehaviour.Blob);
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions Dispatcher { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Dispatcher
                , AzureStorageDataCollectorOptions.StorageBehaviour.Table);
        /// <summary>
        /// This is the Boundary Logging Table storage table.
        /// </summary>
        public AzureStorageDataCollectorOptions Boundary { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.BoundaryLogger
                , AzureStorageDataCollectorOptions.StorageBehaviour.Table);
        /// <summary>
        /// This is the telemetry Table storage  table.
        /// </summary>
        public AzureStorageDataCollectorOptions Telemetry { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Telemetry
                , AzureStorageDataCollectorOptions.StorageBehaviour.Table);
        /// <summary>
        /// This is the telemetry Table storage  table.
        /// </summary>
        public AzureStorageDataCollectorOptions Custom { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Custom);
    }




}
