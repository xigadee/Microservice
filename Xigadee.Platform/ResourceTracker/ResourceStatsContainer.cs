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
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This container holds the usage statistics for a particular resource.
    /// </summary>
    public class ResourceStatsContainer
    {
        #region Declarations
        StatsCounter mBatch;
        StatsCounter mCurrent;
        StatsCounter mBatchSlow;
        StatsCounter mBatchFast;
        StatsCounter[] mBatchHistory;
        private long mTotalErrors = 0;
        private long mTotalMessagesIn = 0;
        private long mBatchCount = 0;
        private int mBatchSize;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceStatsContainer(int batchHistoryCount = 10, int batchSize = 200)
        {
            if (batchHistoryCount < 0)
                throw new ArgumentOutOfRangeException("batchHistoryCount", "batchHistoryCount cannot be less that zero");
            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException("batchSize", "batchSize cannot zero or less");

            mBatchHistory = new StatsCounter[batchHistoryCount];
            mBatchSize = batchSize;
            mCurrent = new StatsCounter();
            mBatch = new StatsCounter(mBatchSize > 0 ? mBatchSize : 500);
        }
        #endregion

        #region CalculateDelta(int now, int start)
        /// <summary>
        /// This method calculates the delta and takes in to account that the
        /// tickcount recycles to negative every 49 days.
        /// </summary>
        /// <param name="now"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int CalculateDelta(int now, int start)
        {
            int delta;
            if (now >= start)
                delta = now - start;
            else
            {
                //Do this, otherwise you'll be in a world of pain every 49 days.
                long upLimit = ((long)(int.MaxValue)) + Math.Abs(int.MinValue - now);
                delta = (int)(upLimit - start);
            }

            return delta;
        }
        #endregion

        #region ErrorIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual void ErrorIncrement()
        {
            Interlocked.Increment(ref mTotalErrors);
        }
        #endregion
        #region ActiveIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual int ActiveIncrement()
        {
            int tickCount = Environment.TickCount;
            Interlocked.Increment(ref mTotalMessagesIn);
            return tickCount;
        }
        #endregion
        #region ActiveDecrement(long start)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(int start)
        {
            int delta = CalculateDelta(Environment.TickCount, start);

            mCurrent.Increment(delta);
            mBatch.Increment(delta);

            if (mBatch.IsBatchComplete)
            {
                var oldBatch = Interlocked.Exchange(ref mBatch, new StatsCounter(mBatchSize > 0 ? mBatchSize : 500));
                oldBatch.Stop();
                BatchArchive(oldBatch);
            }

            return delta;
        }
        #endregion

        #region BatchArchive(StatsCounter batch)
        /// <summary>
        /// This method archives the batch at a specified interval.
        /// </summary>
        /// <param name="batch"></param>
        protected virtual void BatchArchive(StatsCounter batch)
        {
            batch.Id = Interlocked.Increment(ref mBatchCount);
            mBatchHistory[batch.Id % mBatchHistory.LongLength] = batch;

            if (batch.Delta > 0 && (mBatchSlow == null || batch.Average > mBatchSlow.Average))
                mBatchSlow = batch;
            if (batch.Delta > 0 && (mBatchFast == null || batch.Average < mBatchFast.Average))
                mBatchFast = batch;
        }
        #endregion

        #region Batches
        /// <summary>
        /// This is a string representation of the batches.
        /// </summary>
        public string[] Batches
        {
            get
            {
                return mBatchHistory.Where((i) => i != null).OrderByDescending((i) => i.Id).Select((i) => i.ToString()).ToArray();
            }
        }
        /// <summary>
        /// This is the slowest batch.
        /// </summary>
        public string BatchSlow { get { return mBatchSlow == null ? "" : mBatchSlow.ToString(); } }
        /// <summary>
        /// This is the fastest batch.
        /// </summary>
        public string BatchFast { get { return mBatchFast == null ? "" : mBatchFast.ToString(); } }
        #endregion
        #region ToString()
        /// <summary>
        /// This is the default status of the stats.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("InProgress={5} Processed={0} Errors={1} Average={2} Total={6} @ {3:F2}tps ({4:u})"
                , mCurrent.Messages, mTotalErrors, StatsCounter.LargeTime(mCurrent.Average)
                , mCurrent.Tps, mCurrent.Created, mTotalMessagesIn - mCurrent.Messages, StatsCounter.LargeTime(mCurrent.Extent));
        }
        #endregion
    }
}
