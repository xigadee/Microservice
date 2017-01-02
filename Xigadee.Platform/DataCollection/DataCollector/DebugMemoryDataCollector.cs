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
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.Boundary, (e) => EventsBoundary.Add((BoundaryEvent)e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => EventsDispatcher.Add((DispatcherEvent)e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => EventsEventSource.Add((EventSourceEvent)e));
            SupportAdd(DataCollectionSupport.Logger, (e) => EventsLog.Add((LogEvent)e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => EventsMicroservice.Add((MicroserviceStatistics)e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => EventsMetric.Add((TelemetryEvent)e));

            SupportAdd(DataCollectionSupport.Resource, (e) => EventsResource.Add((ResourceEvent)e));

            SupportAdd(DataCollectionSupport.Custom, (e) => EventsCustom.Add(e));

            SupportAdd(DataCollectionSupport.Security, (e) => EventsSecurity.Add((SecurityEvent)e));
        }

        public ConcurrentBag<EventSourceEvent> EventsEventSource { get; set; } = new ConcurrentBag<EventSourceEvent>();

        public ConcurrentBag<BoundaryEvent> EventsBoundary { get; set; } = new ConcurrentBag<BoundaryEvent>();

        public ConcurrentBag<DispatcherEvent> EventsDispatcher { get; set; } = new ConcurrentBag<DispatcherEvent>();

        public ConcurrentBag<LogEvent> EventsLog { get; set; } = new ConcurrentBag<LogEvent>();

        public ConcurrentBag<TelemetryEvent> EventsMetric { get; set; } = new ConcurrentBag<TelemetryEvent>();

        public ConcurrentBag<MicroserviceStatistics> EventsMicroservice { get; set; } = new ConcurrentBag<MicroserviceStatistics>();

        public ConcurrentBag<EventBase> EventsCustom { get; set; } = new ConcurrentBag<EventBase>();

        public ConcurrentBag<SecurityEvent> EventsSecurity { get; set; } = new ConcurrentBag<SecurityEvent>();

        public ConcurrentBag<ResourceEvent> EventsResource { get; set; } = new ConcurrentBag<ResourceEvent>();

    }
}
