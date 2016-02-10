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
    /// This is the base statistics class for commands that process messages.
    /// </summary>
    public class MessagingStatistics:StatusBase
    {
        #region Declarations
        private StatsContainer mStatsDefault;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MessagingStatistics():base()
        {
            mStatsDefault = new StatsContainer();
        }
        #endregion

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

        #region ErrorIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual void ErrorIncrement()
        {
            mStatsDefault.ErrorIncrement();
        }
        #endregion
        #region ActiveIncrement()
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        public virtual int ActiveIncrement()
        {
            return mStatsDefault.ActiveIncrement();
        }
        #endregion
        #region ActiveDecrement(long delta)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(int start)
        {
            return mStatsDefault.ActiveDecrement(start);
        }
        #endregion
        #region ActiveDecrement(TimeSpan delta)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(TimeSpan extent)
        {
            return mStatsDefault.ActiveDecrement(extent);
        }
        #endregion

        #region Message
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                return mStatsDefault.ToString();
            }
            set
            {
                base.Message = value;
            }
        }
        #endregion
        #region Batches
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public virtual string[] Batches
        {
            get
            {
                return mStatsDefault.Batches;
            }
        }

        /// <summary>
        /// This is the slowest batch.
        /// </summary>
        public string BatchSlow { get { return mStatsDefault.BatchSlow; } }
        /// <summary>
        /// This is the fastest batch.
        /// </summary>
        public string BatchFast { get { return mStatsDefault.BatchFast; } }
        #endregion
    }

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
        #region ActiveDecrement...
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="delta">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(int start)
        {
            int delta = CalculateDelta(Environment.TickCount, start);

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
            batch.Id = Interlocked.Increment(ref mBatchCount) ;
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
        public string BatchSlow { get { return mBatchSlow == null?"": mBatchSlow.ToString(); } }
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

    /// <summary>
    /// This class holds performance statistics.
    /// </summary>
    public class StatsCounter
    {
        #region Declarations
        private int mTickCountStart = Environment.TickCount;
        private int? mTickCountEnd = null;
        private int? mBatchSize;
        private long mMessages;
        private long mDelta;

        private int mMin = int.MaxValue;
        private int mMax = int.MinValue;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the constructor for the counter.
        /// </summary>
        /// <param name="BatchSize">The batch size at which point the stats will be set.</param>
        public StatsCounter(int? BatchSize = null)
        {
            Created = DateTime.UtcNow;
            mBatchSize = BatchSize;
        }
        #endregion
        #region Id
        /// <summary>
        /// This is the batch Id. This is incremented after each batch.
        /// </summary>
        public long Id { get; set; } 
        #endregion
        #region IsBatchComplete
        /// <summary>
        /// This boolean property identifies whether the batch in complete.
        /// </summary>
        public bool IsBatchComplete
        {
            get
            {
                return mBatchSize.HasValue && Messages >= mBatchSize.Value;
            }
        }
        #endregion
        #region Increment(int delta)
        /// <summary>
        /// This method increments the batchn with the message delta.
        /// </summary>
        /// <param name="delta">THe time in milliseconds that the recorded action took.</param>
        public void Increment(int delta)
        {
            Interlocked.Increment(ref mMessages);
            Interlocked.Add(ref mDelta, delta);

            ValidateReplace(ref mMin, delta, (current, incoming) => incoming  >0 && current > incoming);
            ValidateReplace(ref mMax, delta, (current, incoming) => current < incoming);
        }
        #endregion

        private void ValidateReplace(ref int current, int incoming, Func<int, int, bool> validate)
        {
            if (validate(current, incoming))
                current = incoming;
        }

        #region Stop()
        /// <summary>
        /// This method marks the batch complete whick sets the final batch delta for the stats.
        /// </summary>
        public void Stop()
        {
            mTickCountEnd = Environment.TickCount;
        }
        #endregion

        #region Extent
        /// <summary>
        /// This is the length of time since the counter started or the final time after it has been stopped.
        /// </summary>
        public TimeSpan Extent
        {
            get { return TimeSpan.FromMilliseconds(StatsContainer.CalculateDelta(mTickCountEnd ?? Environment.TickCount, mTickCountStart)); }
        } 
        #endregion

        #region Created
        /// <summary>
        /// This is the time the batch was created.
        /// </summary>
        public DateTime Created { get; private set; } 
        #endregion
        #region Tps
        /// <summary>
        /// This is the transaction per second for the batch.
        /// </summary>
        public double Tps
        {
            get
            {
                return Extent.TotalMilliseconds == 0?0:((double)Messages / (Extent.TotalMilliseconds/1000));
            }
        }
        #endregion
        #region Average
        /// <summary>
        /// This is the average time for an action to complete
        /// </summary>
        public TimeSpan? Average { get { return Messages > 0 ? TimeSpan.FromMilliseconds(((double)Delta / (double)Messages)) : default(TimeSpan?); } }
        #endregion

        #region Messages
        /// <summary>
        /// This is the total number of messages logged.
        /// </summary>
        public long Messages { get { return mMessages; } }
        #endregion
        #region Delta
        /// <summary>
        /// This is the total delta in milliseconds for the counter.
        /// </summary>
        public long Delta { get { return mDelta; } }
        #endregion

        #region ToString()
        /// <summary>
        /// This is the default summary of the batch.
        /// </summary>
        /// <returns>A string representation of the counter.</returns>
        public override string ToString()
        {
            try
            {
                return string.Format("Batch={0} Processed={1} Average={2} Min={3} Max={4} Total={5} @ {6:F2}tps ({7:u})"
                    , Id, Messages, LargeTime(Average)
                    , mMin == int.MaxValue?"":LargeTime(TimeSpan.FromMilliseconds(mMin))
                    , mMax == int.MinValue?"":LargeTime(TimeSpan.FromMilliseconds(mMax))
                    , LargeTime(Extent), Tps, Created);
            }
            catch (Exception ex)
            {
                return "ERR";
            }
        }
        #endregion

        #region LargeTime(TimeSpan? time, string defaultText="NA")

        static readonly Func<TimeSpan?, string> fnTimeConv = (time) =>
        {
            try
            {
                if (Math.Abs(time.Value.TotalMilliseconds) < 1000)
                    return string.Format("{0:F2}ms", time.Value.TotalMilliseconds);

                if (Math.Abs(time.Value.Days) > 0)
                    return time.Value.ToString(@"d'day'hh'h'mm'm'ss'.'ff's'");
                if (Math.Abs(time.Value.Hours) > 0)
                    return time.Value.ToString(@"hh'h'mm'm'ss'.'ff's'");
                if (Math.Abs(time.Value.Minutes) > 0)
                    return time.Value.ToString(@"mm'm'ss'.'ff's'");

                return time.Value.ToString(@"ss'.'ff's'");
            }
            catch (Exception)
            {
                return null;
            }
        };

        /// <summary>
        /// This helper converts a timespan in to a human readable time.
        /// </summary>
        /// <param name="time">The TimeSpan object to convert.</param>
        /// <param name="defaultText">The default text to display if the TimeSpan object is null. NA by default.</param>
        /// <returns>Returns a string representation of the time.</returns>
        public static string LargeTime(TimeSpan? timeIn, string defaultText = "NA")
        {
            if (!timeIn.HasValue)
                return defaultText;

            string output = fnTimeConv(timeIn);

            if (output == null)
                return "ERR";

            if (timeIn.Value.TotalMilliseconds<0)
                return "-"+output;

            return output;
        }
        #endregion
    }
}
