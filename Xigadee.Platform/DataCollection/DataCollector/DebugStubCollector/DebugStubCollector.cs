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
    public class DebugStubCollector: DataCollectorBase<DataCollectorStatistics>
    {
        public DebugStubCollector() : base()
        {
        }

        public override void Write(EventSourceEvent eventData)
        {
            EventsEventSource.Add(eventData);
        }

        public override void Write(MetricEvent eventData)
        {
            EventsMetric.Add(eventData);
        }

        public override void Write(LogEvent eventData)
        {
            EventsLog.Add(eventData);
        }

        public override void Write(PayloadEvent eventData)
        {
            EventsPayload.Add(eventData);

        }
        public override void Write(BoundaryEvent eventData)
        {
            EventsBoundary.Add(eventData);
        }

        public override void Write(MicroserviceStatistics eventData)
        {
            EventsStatistics.Add(eventData);
        }

        public ConcurrentBag<EventSourceEvent> EventsEventSource { get; set; } = new ConcurrentBag<EventSourceEvent>();

        public ConcurrentBag<BoundaryEvent> EventsBoundary { get; set; } = new ConcurrentBag<BoundaryEvent>();

        public ConcurrentBag<PayloadEvent> EventsPayload { get; set; } = new ConcurrentBag<PayloadEvent>();

        public ConcurrentBag<LogEvent> EventsLog { get; set; } = new ConcurrentBag<LogEvent>();

        public ConcurrentBag<MetricEvent> EventsMetric { get; set; } = new ConcurrentBag<MetricEvent>();

        public ConcurrentBag<MicroserviceStatistics> EventsStatistics { get; set; } = new ConcurrentBag<MicroserviceStatistics>();


        protected override void StartInternal()
        {

        }

        protected override void StopInternal()
        {

        }
    }
}
