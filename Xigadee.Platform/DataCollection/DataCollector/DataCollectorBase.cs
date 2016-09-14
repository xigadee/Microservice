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
    /// This abstract class is used to implement data collectors.
    /// </summary>
    public abstract class DataCollectorBase: IDataCollectorComponent
    {
        protected DataCollectorBase(string name, DataCollectionSupport support = DataCollectionSupport.All)
        {
            Name = name;
            Support = support;
        }

        /// <summary>
        /// This returns the type of supported data collection
        /// </summary>
        public DataCollectionSupport Support { get; }
        /// <summary>
        /// Returns true if the requested type is supported.
        /// </summary>
        /// <param name="support">The data collection type</param>
        /// <returns></returns>
        public bool IsSupported(DataCollectionSupport support)
        {
            return (Support & support) == support;
        }

        public string Name
        {
            get;
        }

        public string OriginatorId
        {
            get;set;
        }

        public abstract Guid BatchPoll(int requested, int actual, string channelId);

        public abstract Task Log(LogEvent logEvent);

        public abstract void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?));

        public abstract void TrackMetric(string metricName, double value);

        public abstract Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false);
    }
}
