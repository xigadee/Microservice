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
using System.IO;

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
                , AzureStorageBehaviour.Blob)
            { BlobConverter = LogLocalBlobConverter };
        /// <summary>
        /// This is the EventSource options.
        /// </summary>
        public AzureStorageDataCollectorOptions EventSource { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.EventSource
                , AzureStorageBehaviour.Blob)
            { BlobConverter = EventSourceLocalBlobConverter };
        /// <summary>
        /// This is the Statistics options. By default encryption is not set for statistics.
        /// </summary>
        public AzureStorageDataCollectorOptions Statistics { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Statistics
                , AzureStorageBehaviour.BlobAndTable)
            { Encryption = AzureStorageEncryption.None };
        /// <summary>
        /// This is the Dispatcher options.
        /// </summary>
        public AzureStorageDataCollectorOptions Dispatcher { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Dispatcher
                , AzureStorageBehaviour.Table);
        /// <summary>
        /// This is the Boundary Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Boundary { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.BoundaryLogger
                , AzureStorageBehaviour.Table);
        /// <summary>
        /// This is the Telemetry Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Telemetry { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Telemetry
                , AzureStorageBehaviour.Table);
        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Resource { get; set; }
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Telemetry
                , AzureStorageBehaviour.Table);
        /// <summary>
        /// This is the Custom options.
        /// </summary>
        public AzureStorageDataCollectorOptions Custom { get; set; } 
            = new AzureStorageDataCollectorOptions(DataCollectionSupport.Custom);

        public static AzureStorageContainerBlob LogLocalBlobConverter(MicroserviceId msId, EventBase ev)
        {
            var e = ev as LogEvent;

            string level = Enum.GetName(typeof(LoggingLevel), e.Level);
            string id = "";
            string folder = string.Format("{0}/{1}/{2:yyyy-MM-dd}/{2:HH}", msId.Name, level, DateTime.UtcNow);

            //if (e is ILogStoreName)
            //    return ((ILogStoreName)logEvent).StorageId;

            //// If there is a category specified and it contains valid digits or characters then make it part of the log name to make it easier to filter log events
            //if (!string.IsNullOrEmpty(logEvent.Category) && logEvent.Category.Any(char.IsLetterOrDigit))
            //    return string.Format("{0}_{1}_{2}", logEvent.GetType().Name, new string(logEvent.Category.Where(char.IsLetterOrDigit).ToArray()), Guid.NewGuid().ToString("N"));

            //return string.Format("{0}_{1}", logEvent.GetType().Name, Guid.NewGuid().ToString("N"));
            return new Xigadee.AzureStorageContainerBlob();
        }

        public static AzureStorageContainerBlob EventSourceLocalBlobConverter(MicroserviceId msId, EventBase ev)
        {
            var e = ev as EventSourceEntry;

            string id = string.Format("{0}.json", string.Join("_", e.Key.Split(Path.GetInvalidFileNameChars())));
            string folder = string.Format("{0}/{1:yyyy-MM-dd}/{2}", msId.Name, e.UTCTimeStamp, e.EntityType);

            return new AzureStorageContainerBlob();
        }
    }
}
