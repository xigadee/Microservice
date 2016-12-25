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
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethods
    {
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The log event.</param>
        /// <param name="sync">Specifies whether this method should be written syncronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, LogEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Logger, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The boundary event.</param>
        /// <param name="sync">Specifies whether this method should be written syncronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, BoundaryEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.BoundaryLogger, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The event source.</param>
        /// <param name="sync">Specifies whether this method should be written syncronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, EventSourceEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.EventSource, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The resource event.</param>
        /// <param name="sync">Specifies whether this method should be written syncronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, ResourceEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Resource, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The telemetry event.</param>
        /// <param name="sync">Specifies whether this method should be written syncronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, TelemetryEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Telemetry, sync);
        }
    }
}
