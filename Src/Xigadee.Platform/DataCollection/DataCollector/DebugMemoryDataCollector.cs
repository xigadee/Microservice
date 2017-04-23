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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This is a test collector. It is primarily used for unit testing to ensure the correct logging has occurred.
    /// </summary>
    public class DebugMemoryDataCollector: DataCollectorBase
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="supportMap">The support map can be used to filter the types of events that you wish to filter. Leave this null to support all types.</param>
        public DebugMemoryDataCollector(DataCollectionSupport? supportMap = null):base(supportMap)
        {

        }
        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.Boundary, (e) => EventsBoundary.Add(e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => EventsDispatcher.Add(e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => EventsEventSource.Add(e));
            SupportAdd(DataCollectionSupport.Logger, (e) => EventsLog.Add(e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => EventsMicroservice.Add(e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => EventsMetric.Add(e));

            SupportAdd(DataCollectionSupport.Resource, (e) => EventsResource.Add(e));

            SupportAdd(DataCollectionSupport.Custom, (e) => EventsCustom.Add(e));

            SupportAdd(DataCollectionSupport.Security, (e) => EventsSecurity.Add(e));
        }

        public ConcurrentBag<EventHolder> EventsEventSource { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsBoundary { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsDispatcher { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsLog { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsMetric { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsMicroservice { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsCustom { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsSecurity { get; set; } = new ConcurrentBag<EventHolder>();

        public ConcurrentBag<EventHolder> EventsResource { get; set; } = new ConcurrentBag<EventHolder>();

    }
}
