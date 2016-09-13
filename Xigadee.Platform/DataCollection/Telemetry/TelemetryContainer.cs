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

#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This collection contains the Telemetry modules.
    /// </summary>
    public class TelemetryContainer : CollectionContainerBase<ITelemetry>
    {      
        #region Declarations
        /// <summary>
        /// This holds a collection of counters used to identify performance bottlenecks.
        /// </summary>
        ConcurrentDictionary<string, TelemetryCounterHolder> mStatusTracker;
        #endregion

        #region Constructor
        public TelemetryContainer(IEnumerable<ITelemetry> telemetry)
            : base(telemetry)
        {
            mStatusTracker = new ConcurrentDictionary<string, TelemetryCounterHolder>();
        } 
        #endregion

        public void Log(string key, int delta, bool isSuccess)
        {
            var holder = mStatusTracker.GetOrAdd(key, (k) => new TelemetryCounterHolder(k));

            holder.Log(delta, isSuccess, !isSuccess);
        }

        #region FlushTelemetry()
        /// <summary>
        /// This method flushes log information out to the telemetries and resets the status tracker
        /// </summary>
        public async Task FlushTelemetry()
        {
            try
            {
                var existingStatusTracker = Interlocked.Exchange(ref mStatusTracker, new ConcurrentDictionary<string, TelemetryCounterHolder>());
                var messageCounters = existingStatusTracker.Values.ToList();
                Items.ForEach(t => messageCounters.ForEach(mc => LogTelemetry(t, mc)));            
                existingStatusTracker.Clear();
            }
            catch (Exception ex)
            {

            }

        }
        #endregion

        private static void LogTelemetry(ITelemetry telemetry, TelemetryCounterHolder messageCounterHolder)
        {
            //telemetry.TrackMetric(string.Format("{0}-AverageDelta", messageCounterHolder.Key), messageCounterHolder.AverageDelta.TotalMilliseconds);
            //telemetry.TrackMetric(string.Format("{0}-SuccessfulMessages", messageCounterHolder.Key), messageCounterHolder.SuccessfulMessages);
            //telemetry.TrackMetric(string.Format("{0}-FailedMessages", messageCounterHolder.Key), messageCounterHolder.FailedMessages);            
        }
    }
}
