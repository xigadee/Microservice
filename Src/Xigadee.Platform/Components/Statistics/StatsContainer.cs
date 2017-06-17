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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This container holds the usage statistics.
    /// </summary>
    public class StatsContainer
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
        public StatsContainer(int batchHistoryCount = 15, int batchSize = 500)
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
        /// It returns the current tick count.
        /// </summary>
        public virtual int ActiveIncrement()
        {
            int tickCount = Environment.TickCount;
            Interlocked.Increment(ref mTotalMessagesIn);
            return tickCount;
        }
        #endregion
        #region ActiveDecrement...
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="start">The tick count when the process started.</param>
        public virtual int ActiveDecrement(int start)
        {
            int delta = ConversionHelper.CalculateDelta(Environment.TickCount, start);

            ActiveDecrementInternal(delta);

            return delta;
        }
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(TimeSpan extent)
        {
            int delta = (int)extent.TotalMilliseconds;

            ActiveDecrementInternal(delta);

            return delta;
        }

        private void ActiveDecrementInternal(int delta)
        {
            mCurrent.Increment(delta);
            mBatch.Increment(delta);

            if (mBatch.IsBatchComplete)
            {
                var oldBatch = Interlocked.Exchange(ref mBatch, new StatsCounter(mBatchSize > 0 ? mBatchSize : 500));
                oldBatch.Stop();
                BatchArchive(oldBatch);
            }
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
                , mCurrent.Messages //0
                , mTotalErrors //1
                , StatsCounter.LargeTime(mCurrent.Average) //2
                , mCurrent.Tps //3
                , mCurrent.Created //4
                , mTotalMessagesIn - mCurrent.Messages //5
                , StatsCounter.LargeTime(mCurrent.Extent)); //6
        }
        #endregion
    }

}
