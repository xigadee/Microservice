using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This container holds the usage statistics.
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
    /// <summary>
    /// This class holds a record of messages that have signalled a slow down.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class ResourceRequestTrack
    {
        private readonly int mStart;
        private int mRetryCount = 0;
        private long mRetryTime = 0;
        private string mGroup;

        public ResourceRequestTrack(Guid id, string group)
        {
            mStart = Environment.TickCount;
            Id = id;
            mGroup = group;
        }

        /// <summary>
        /// This is the incoming profile id from the calling party.
        /// </summary>
        public Guid ProfileId { get; set; }

        #region Id
        /// <summary>
        /// This is the traceid of the payload that signalled a throttle request.
        /// </summary>
        public Guid Id { get; private set; } 
        #endregion

        /// <summary>
        /// This is the throttle time expiry.
        /// </summary>
        public TimeSpan? Active
        {
            get
            {
                return TimeSpan.FromMilliseconds(ConversionHelper.CalculateDelta(Environment.TickCount, mStart));
            }
        }

        public int RetryCount { get { return mRetryCount; } }

        /// <summary>
        /// This is the debug string for logging.
        /// </summary>
        public string Debug
        {
            get
            {
                return string.Format("[{0}]/{1} Retries={2}/{3} {4} - {5}", mGroup, ProfileId, mRetryTime, mRetryCount, Active, Id);
            }
        }

        public void RetrySignal(int delta, ResourceRetryReason reason)
        {
            Interlocked.Increment(ref mRetryCount);
            Interlocked.Add(ref mRetryTime, (long)delta);
        }
    }
    /// <summary>
    /// This is the holder for the resource statistics.
    /// </summary>
    [DebuggerDisplay("ResourceStatistics: {Name}={RateLimitAdjustmentPercentage}")]
    public class ResourceStatistics: MessagingStatistics
    {
        #region Declarations
        /// <summary>
        /// This concurrent dictionary holds the current list of active messages and their ratelimit request count.
        /// </summary>
        protected readonly ConcurrentDictionary<Guid, ResourceRequestTrack> mActive;

        protected readonly double mRateLimitCutoutPercentage; 

        protected readonly string mName;

        protected StatsCounter mJobs;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceStatistics()
        {
            mActive = new ConcurrentDictionary<Guid, ResourceRequestTrack>();
            mRateLimitCutoutPercentage = 1D;
        } 

        #endregion

        #region RateLimitCutoutPercentage
        /// <summary>
        /// This is the retry ratio at which point the incoming messahes will be stopped completely.
        /// </summary>
        public double RateLimitCutoutPercentage
        {
            get
            {
                return mRateLimitCutoutPercentage;
            }
        } 
        #endregion

        #region RetrySum
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public int RetrySum
        {
            get
            {
                return mActive.Values.Select(r => r.RetryCount).Sum();
            }
        }
        #endregion
        #region RetryRatio
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public double RetryRatio
        {
            get
            {
                int count = mActive.Count;

                return count == 0 ? 0 : (double)RetrySum / (double)count;
            }
        }
        #endregion
        #region RateLimitAdjustmentPercentage
        /// <summary>
        /// This is the current rate limit summation across the active payload requests.
        /// If rate limiting is not supported the value will be null.
        /// </summary>
        public double RateLimitAdjustmentPercentage
        {
            get
            {
                double ratio = RetryRatio / mRateLimitCutoutPercentage;

                //Ok, we have exceeded the cut off threshold. Stop everything.
                if (ratio >= 1)
                    return 0;

                if (ratio <= 0)
                    return 1;

                //Adjust the percentage so that as the ratio reduces the adjustment decreases.
                return 1 - ratio;
            }
        }
        #endregion

        /// <summary>
        /// This is a list of the currently active requests.
        /// </summary>
        public string[] Active
        {
            get
            {
                try
                {
                    return mActive.Values.Select((v) => v.Debug).ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        internal Guid Start(string name, string group, Guid profileId)
        {
            ActiveIncrement();
            var id = Guid.NewGuid();
            var item = mActive.GetOrAdd(id, new ResourceRequestTrack(id, group) { ProfileId = profileId } );
            return item.Id;
        }

        internal void End(string name, Guid profileId, int start, ResourceRequestResult result)
        {
            int delta = ActiveDecrement(start);
            ResourceRequestTrack outValue;
            if (!mActive.TryRemove(profileId, out outValue))
                return;

            Complete(outValue, TimeSpan.FromMilliseconds(delta));
        }

        internal void Retry(string name, Guid profileId, int start, ResourceRetryReason reason)
        {
            ErrorIncrement();
            int delta = ConversionHelper.CalculateDelta(Environment.TickCount, start);
            ResourceRequestTrack outValue;
            if (!mActive.TryGetValue(profileId, out outValue))
                return;

            outValue.RetrySignal(delta, reason);
        }

        private void Complete(ResourceRequestTrack outValue, TimeSpan delta)
        {
            
        }
    }
}
