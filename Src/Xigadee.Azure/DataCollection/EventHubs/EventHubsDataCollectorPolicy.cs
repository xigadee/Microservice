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
    public class EventHubsDataCollectorPolicy : DataCollectorPolicy
    {
        /// <summary>
        /// This is the Log options.
        /// </summary>
        public EventHubDataCollectorOptions Log { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Logger

                );

        /// <summary>
        /// This is the EventSource options.
        /// </summary>
        public EventHubDataCollectorOptions EventSource { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.EventSource

                );
        /// <summary>
        /// This is the Statistics options. By default encryption is not set for statistics.
        /// </summary>
        public EventHubDataCollectorOptions Statistics { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Statistics

                )
            { EncryptionPolicy = AzureStorageEncryption.None };
        /// <summary>
        /// This is the Dispatcher options.
        /// </summary>
        public EventHubDataCollectorOptions Dispatcher { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Dispatcher

                );
        /// <summary>
        /// This is the Boundary Table storage options.
        /// </summary>
        public EventHubDataCollectorOptions Boundary { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Boundary

                );
        /// <summary>
        /// This is the Telemetry Table storage options.
        /// </summary>
        public EventHubDataCollectorOptions Telemetry { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Telemetry

                );
        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public EventHubDataCollectorOptions Resource { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Resource

                );

        /// <summary>
        /// This is the Resource Table storage options.
        /// </summary>
        public EventHubDataCollectorOptions Security { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Security

                );

        /// <summary>
        /// This is the Custom options.
        /// </summary>
        public EventHubDataCollectorOptions Custom { get; set; }
            = new EventHubDataCollectorOptions(DataCollectionSupport.Custom);

        /// <summary>
        /// This is an enumeration of all the options.
        /// </summary>
        public virtual IEnumerable<EventHubDataCollectorOptions> Options
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
