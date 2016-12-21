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
            = new AzureStorageDataCollectorOptions("Log", AzureStorageDataCollectorOptions.StorageBehaviour.Blob);
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions EventSource { get; set; } 
            = new AzureStorageDataCollectorOptions("EventSource", AzureStorageDataCollectorOptions.StorageBehaviour.Blob);
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions Statistics { get; set; } 
            = new AzureStorageDataCollectorOptions("Statistics", AzureStorageDataCollectorOptions.StorageBehaviour.Blob);
        /// <summary>
        /// This is the Dispatcher Table storage logging table.
        /// </summary>
        public AzureStorageDataCollectorOptions Dispatcher { get; set; } 
            = new AzureStorageDataCollectorOptions("Dispatcher", AzureStorageDataCollectorOptions.StorageBehaviour.Table);
        /// <summary>
        /// This is the Boundary Logging Table storage table.
        /// </summary>
        public AzureStorageDataCollectorOptions Boundary { get; set; } 
            = new AzureStorageDataCollectorOptions("Boundary", AzureStorageDataCollectorOptions.StorageBehaviour.Table);
        /// <summary>
        /// This is the telemetry Table storage  table.
        /// </summary>
        public AzureStorageDataCollectorOptions Telemetry { get; set; } 
            = new AzureStorageDataCollectorOptions("Telemetry", AzureStorageDataCollectorOptions.StorageBehaviour.Table);
        /// <summary>
        /// This is the telemetry Table storage  table.
        /// </summary>
        public AzureStorageDataCollectorOptions Custom { get; set; } 
            = new AzureStorageDataCollectorOptions("Custom");
    }

    /// <summary>
    /// This class contains the storage configuration options for the specific DataCollection type.
    /// </summary>
    public class AzureStorageDataCollectorOptions
    {
        public AzureStorageDataCollectorOptions(string tableId
            , StorageBehaviour behavior = StorageBehaviour.None
            )
        {
            TableId = tableId;
            Behaviour = behavior;
        }

        /// <summary>
        /// This is the Boundary Logging Table storage table.
        /// </summary>
        public string TableId { get; set; }

        public Func<EventBase, AzureStorageBlobLocation> BlobNamer { get; set; } = null;


        public Func<EventBase, byte[]> BlobSerializer { get; set; } = null;

        public Func<EventBase, ITableEntity> TableSerializer { get; set; } = null;
        /// <summary>
        /// The storage action
        /// </summary>
        public StorageBehaviour Behaviour { get; set; }

        /// <summary>
        /// The encryption type.
        /// </summary>
        public StorageEncryption Encryption { get; set; } = StorageEncryption.BlobWhenPresent;

        /// <summary>
        /// This enumeration determines the specific storage behavior for the data type.
        /// </summary>
        [Flags]
        public enum StorageBehaviour
        {
            None = 0,
            Blob = 1,
            Table = 2,
            BlobAndTable = 3
        }

        /// <summary>
        /// This settings determines the specific encryption settings for the data type.
        /// </summary>
        public enum StorageEncryption
        {
            None,
            BlobWhenPresent,
            BlobAlwaysWithException
        }
    }


}
