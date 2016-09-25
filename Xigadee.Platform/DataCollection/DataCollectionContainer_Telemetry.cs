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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class DataCollectionContainer
    {
        #region Declarations
        /// <summary>
        /// This holds a collection of counters used to identify performance bottlenecks.
        /// </summary>
        ConcurrentDictionary<string, TelemetryCounterHolder> mStatusTracker;
        #endregion
        #region StartTelemetry()
        /// <summary>
        /// This method starts the telemetry.
        /// </summary>
        protected virtual void StartTelemetry()
        {
            mStatusTracker = new ConcurrentDictionary<string, TelemetryCounterHolder>();
            mTelemetry.ForEach((c) => ServiceStart(c));
        }

        protected virtual void StopTelemetry()
        {
            mTelemetry.ForEach((c) => ServiceStop(c));
        }
        #endregion
        #region Telemetry(string key, int delta, bool isSuccess)
        /// <summary>
        /// This method is called by the Dispatcher when a message has completed processing.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <param name="isSuccess"></param>
        public void Telemetry(string key, int delta, bool isSuccess)
        {
            var holder = mStatusTracker.GetOrAdd(key, (k) => new TelemetryCounterHolder(k));

            holder.Log(delta, isSuccess, !isSuccess);
        }
        #endregion
        #region TelemetryFlush()
        /// <summary>
        /// This method is called by a timer to flush the telemetry statistics to the underlying telemetry loggers.
        /// </summary>
        /// <returns></returns>
        public async Task TelemetryFlush()
        {
            try
            {
                var existingStatusTracker = Interlocked.Exchange(ref mStatusTracker, new ConcurrentDictionary<string, TelemetryCounterHolder>());
                var messageCounters = existingStatusTracker.Values.ToList();
                //mTelemetry.ForEach(t => messageCounters.ForEach(mc => mc.Log(t, mc)));
                existingStatusTracker.Clear();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion


    }
}
