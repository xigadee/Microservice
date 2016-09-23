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

namespace Xigadee
{
    /// <summary>
    /// This is a test collector. It is primarily used for unit testing to ensure the correct logging has occurred.
    /// </summary>
    public class MemoryStubCollector: DataCollectorBase
    {
        public MemoryStubCollector() : base(typeof(MemoryStubCollector).Name)
        {
        }

        public override void BoundaryLog(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
        }

        public override void BoundaryLogPoll(Guid id, int requested, int actual, string channelId)
        {
        }

        public override void DispatcherPayloadComplete(TransmissionPayload payload, int delta, bool isSuccess)
        {
        }

        public override void DispatcherPayloadException(TransmissionPayload payload, Exception pex)
        {
        }

        public override void DispatcherPayloadIncoming(TransmissionPayload payload)
        {
        }

        public override void DispatcherPayloadUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {
        }

        public override async Task Log(LogEvent logEvent)
        {
        }

        public override void MicroserviceStatisticsIssued(MicroserviceStatistics statistics)
        {
        }

        public override void TrackMetric(string metricName, double value)
        {
        }

        public override async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
