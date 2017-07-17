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

namespace Xigadee
{
    /// <summary>
    /// This is the default messaging statistics.
    /// </summary>
    public class MessagingServiceStatistics: MessagingStatistics
    {
        #region Declarations
        private StatsContainer mStatsEnqueueTime;
        private StatsContainer mStatsException;
        private StatsContainer mStatsRateLimitHits;
        #endregion

        /// <summary>
        /// The default constructor.
        /// </summary>
        public MessagingServiceStatistics()
        {
            mStatsEnqueueTime = new StatsContainer();
            mStatsException = new StatsContainer();
            mStatsRateLimitHits = new StatsContainer();
        }

        /// <summary>
        /// This is the id of the client.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name override so that it gets serialized at the top of the JSON data.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// This boolean property indicates whether the service is active.
        /// </summary>
        public bool IsActive { get; set; }

        #region RecordQueueTime(DateTime? enqueueTime)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual void QueueTimeLog(DateTime? enqueueTime)
        {
            if (enqueueTime.HasValue)
            {
                var extent = DateTime.UtcNow - enqueueTime.Value;
                mStatsEnqueueTime.ActiveIncrement();
                mStatsEnqueueTime.ActiveDecrement((int)extent.TotalMilliseconds);
            }
        }
        #endregion

        #region QueueLength
        /// <summary>
        /// This is the length of the queue.
        /// </summary>
        public long? QueueLength { get; set; }
        #endregion

        #region RateLimits
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public string RateLimits
        {
            get
            {
                return mStatsRateLimitHits.ToString();
            }
        }
        #endregion
        #region RateLimitBatches
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public string[] RateLimitBatches
        {
            get
            {
                return mStatsRateLimitHits.Batches;
            }
        }
        #endregion
        #region QueueTimeMessage
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public string QueueTimeMessage
        {
            get
            {
                return mStatsEnqueueTime.ToString();
            }
        }
        #endregion
        #region QueueTimeBatches
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public virtual string[] QueueTimeBatches
        {
            get
            {
                return mStatsEnqueueTime.Batches;
            }
        }

        /// <summary>
        /// This is the slowest batch.
        /// </summary>
        public string QueueTimeSlow { get { return mStatsEnqueueTime.BatchSlow; } }
        /// <summary>
        /// This is the fastest batch.
        /// </summary>
        public string QueueTimeFast { get { return mStatsEnqueueTime.BatchFast; } }
        #endregion

        #region ExceptionHits
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public string ExceptionHits
        {
            get
            {
                return mStatsException.ToString();
            }
        }
        #endregion
        #region ExceptionHitsBatches
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public virtual string[] ExceptionHitsBatches
        {
            get
            {
                return mStatsException.Batches;
            }
        }

        #endregion

        #region Filters
        /// <summary>
        /// This is the list of filters if specified.
        /// </summary>
        public List<string> Filters { get; set; }
        #endregion

        #region ActivePayloads
        /// <summary>
        /// This is the list of current requests signalling a throttle request.
        /// </summary>
        public List<string> ActivePayloads { get; set; }
        public int ActivePayloadCount { get; set; }

        /// <summary>
        /// This is current rate limit hit count for the client
        /// </summary>
        public bool RateLimitSupported { get; set; }

        public int? RateLimitSum { get; set; }

        /// <summary>
        /// This is the rate limit hit count divided by the number of active payloads.
        /// </summary>
        public double? RateLimitSignalRatio { get; set; }

        public double? RateLimitAdjustmentPercentage { get; set; }
        #endregion

        public void ThrottleHitIncrement()
        {
            mStatsRateLimitHits.ActiveIncrement();
            mStatsRateLimitHits.ActiveDecrement(0);
        }

        public void ExceptionHitIncrement()
        {
            mStatsException.ActiveIncrement();
            mStatsException.ActiveDecrement(0);
        }
    }
}
