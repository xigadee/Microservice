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
    /// This is a test collector.
    /// </summary>
    public class MemoryCollector: DataCollectorBase
    {
        public MemoryCollector() : base(typeof(MemoryCollector).Name)
        {
        }

        public override Guid BatchPoll(int requested, int actual, string channelId)
        {
            throw new NotImplementedException();
        }

        public override Task Log(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }

        public override void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            throw new NotImplementedException();
        }

        public override void TrackMetric(string metricName, double value)
        {
            throw new NotImplementedException();
        }

        public override Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            throw new NotImplementedException();
        }
    }
}
