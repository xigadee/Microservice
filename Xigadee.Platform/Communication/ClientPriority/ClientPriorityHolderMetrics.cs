#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This protected calss
    /// </summary>
    public class ClientPriorityHolderMetrics
    {
        #region Declarations
        long mPollIn;
        long mPollOut;

        long mPollAttempted;
        long mPollAchieved;
        long mPollAttemptedBatch;
        long mPollAchievedBatch;

        int mPollException;

        double mCapacityPercentage;

        private int mSkipCount = 0;


        private decimal? mPollTimeReduceRatio;

        private int mPolls, mPollsSuccess;
        /// <summary>
        /// This is the maximum wait time that the client can wait before it is polled.
        /// </summary>
        TimeSpan mMaxAllowedPollWait;
        /// <summary>
        /// This is the minimum wait time that the client should wait before it is polled.
        /// </summary>
        TimeSpan mMinExpectedPollWait;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="rateLimiter"></param>
        /// <param name="isDeadletter"></param>
        /// <param name="priority"></param>
        /// <param name="weighting"></param>
        public ClientPriorityHolderMetrics(IListenerClientPollAlgorithm algorithm
            , IResourceRequestRateLimiter rateLimiter
            , bool isDeadletter
            , int priority
            , decimal weighting)
        {
            Algorithm = algorithm;
            RateLimiter = rateLimiter;
            IsDeadletter = isDeadletter;
            Priority = priority;
            PriorityWeighting = weighting;

            mPollTimeReduceRatio = algorithm.PollTimeReduceRatio;

            mMaxAllowedPollWait = algorithm.MaxAllowedPollWait;
            mMinExpectedPollWait = algorithm.MinExpectedPollWait;

            if (mMinExpectedPollWait > mMaxAllowedPollWait)
                mMinExpectedPollWait = mMaxAllowedPollWait;

            mCapacityPercentage = algorithm.CapacityPercentage;
        }
        #endregion

        #region RateLimiter
        /// <summary>
        /// This is the resource rate limiter for the underlying client
        /// </summary>
        public IResourceRequestRateLimiter RateLimiter { get; }
        #endregion
        #region Algorithm
        /// <summary>
        /// This is the priority and allocation algorithm
        /// </summary>
        public IListenerClientPollAlgorithm Algorithm { get; }
        #endregion
        #region IsDeadletter
        /// <summary>
        /// A shortcut that identifies whether the client is a deadletter
        /// </summary>
        public bool IsDeadletter
        {
            get;
        }
        #endregion
        #region Priority
        /// <summary>
        /// This is the priority level for the underlying client.
        /// </summary>
        public int Priority { get; }
        #endregion
        #region PriorityWeighting
        /// <summary>
        /// This is the client weighting which is used to adjust the priority for the polling.
        /// This value is a percentage ratio.
        /// </summary>
        public decimal PriorityWeighting { get; }
        #endregion

        #region PollSuccessRate
        /// <summary>
        /// This is the ratio success of the poll hit rate.
        /// </summary>
        public decimal PollSuccessRate
        {
            get
            {
                if (mPolls == 0)
                    return 1;

                return (decimal)mPollsSuccess / (decimal)mPolls;
            }
        }
        #endregion

        /// <summary>
        /// This is the current skip count for the holder.
        /// </summary>
        public int SkipCount { get { return mSkipCount; } }


        public int Reserve(int available)
        {
            double ratelimitAdjustment = 1D;

            if (RateLimiter != null)
                ratelimitAdjustment = RateLimiter?.RateLimitAdjustmentPercentage ?? 1D;

            //We make sure that a small fraction rate limit adjust resolves to zero as we use ceiling to make even small fractional numbers go to one.
            int takenCalc = (int)Math.Ceiling((double)available * CapacityPercentage * Math.Round(ratelimitAdjustment, 2, MidpointRounding.AwayFromZero));


            LastPollTickCount = Environment.TickCount;
            Interlocked.Increment(ref mPollIn);
            LastReserved = takenCalc;
            return takenCalc;
        }

        public int? LastReserved { get; set; }

        /// <summary>
        /// This method releases the holder so that it can be polled again.
        /// </summary>
        /// <param name="exception">A flag indicating whether there was an exception.</param>
        /// <returns>Returns true if the holder returned all records requested.</returns>
        public void Release(bool exception)
        {
            Interlocked.Increment(ref mPollOut);

            if (exception)
                Interlocked.Increment(ref mPollException);
            else
                //Recalculate statistics.
                CapacityPercentageRecalculate(LastActual, LastReserved);
        }

        public int PollBegin(int reserved)
        {
            Interlocked.Add(ref mPollAttempted, reserved);
            Interlocked.Add(ref mPollAttemptedBatch, reserved);

            Interlocked.Increment(ref mPolls);

            return (int)mMinExpectedPollWait.TotalMilliseconds;
        }

        public void PollEnd(int payloadCount, bool hasErrored)
        {
            Interlocked.Add(ref mPollAchieved, payloadCount);
            Interlocked.Add(ref mPollAchievedBatch, payloadCount);

            LastActual = payloadCount;
            LastActualTime = DateTime.UtcNow;

            if (payloadCount > 0)
                Interlocked.Increment(ref mPollsSuccess);

            RecalculateSkipCount(payloadCount > 0);
        }

        private void RecalculateSkipCount(bool success)
        {
            mSkipCount = 10;
        }

        #region LastActual
        /// <summary>
        /// This is the last actual slot value for the client.
        /// </summary>
        public int? LastActual { get; private set; }
        /// <summary>
        /// This is the last time for a poll exception.
        /// </summary>
        public DateTime? LastActualTime { get; set; }
        #endregion

        #region ShouldSkip
        /// <summary>
        /// This method returns true if the client should be skipped for this poll.
        /// </summary>
        /// <returns>Returns true if the poll should be skipped.</returns>
        public bool ShouldSkip()
        {
            //Get the timespan since the last poll
            var lastPollTimeSpan = LastPollTimeSpan;

            //Check whether we have waited the minimum poll time, if not skip
            if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value < mMinExpectedPollWait)
                return true;

            //Check whether we have waited over maximum poll time, then poll
            if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value > CalculatedMaximumPollWait)
                return false;

            //Check whether the skip count is greater that zero, and if so then skip
            if (Interlocked.Decrement(ref mSkipCount) > 0)
                return true;

            return false;
        }
        #endregion
        #region CapacityPercentage
        /// <summary>
        /// This is the calcualted percentage that determines how many of the available reserved slots the client will take.
        /// </summary>
        public double CapacityPercentage { get { return mCapacityPercentage; } }
        #endregion
        #region CapacityPercentageRecalculate(long LastActual, int LastReserved)
        /// <summary>
        /// This method is used to recalcualte the capacity percentage.
        /// </summary>
        /// <param name="lastActual"></param>
        /// <param name="lastReserved"></param>
        /// <returns></returns>
        public bool CapacityPercentageRecalculate(long? lastActual, int? lastReserved)
        {
            if (mPollAttemptedBatch > 0)
            {
                //If the actual and reserved tend to 100% we want the capacity to grow to 95%
                double capacityPercentage = mCapacityPercentage * ((double)mPollAchievedBatch / (double)mPollAttemptedBatch) * 1.05D;

                if (capacityPercentage >= 0.95D)
                    capacityPercentage = 0.95D;
                else if (capacityPercentage <= 0.01D)
                    capacityPercentage = 0.01D;

                Interlocked.Exchange(ref mCapacityPercentage, capacityPercentage);

                return true;
            }
            return false;
        }
        #endregion

        #region PriorityRecalculate(long? queueLength)
        /// <summary>
        /// This is the priority based on the elapsed poll tick time and the overall priority.
        /// It is used to ensure that clients with the overall same base priority are accessed 
        /// so the one polled last is then polled first the next time.
        /// </summary>
        public long PriorityRecalculate(long? queueLength)
        {
            long newPriority = (IsDeadletter ? 0xFFFFFFFF : 0xFFFFFFFFFFFF);

            try
            {
                if (PriorityTickCount.HasValue)
                    newPriority += StatsContainer.CalculateDelta(Environment.TickCount, PriorityTickCount.Value);

                PriorityTickCount = Environment.TickCount;
                //Add the queue length to add the listener with the greatest number of messages.
                PriorityQueueLength = queueLength;

                newPriority += PriorityQueueLength ?? 0;

                newPriority = (long)((decimal)newPriority * PriorityWeighting);
            }
            catch (Exception ex)
            {
            }

            PriorityCalculated = newPriority;
            return newPriority;
        }
        #endregion
        #region PriorityCalculated
        /// <summary>
        /// This is the current client priority.
        /// </summary>
        public long? PriorityCalculated { get; set; }
        #endregion
        #region PriorityTickCount
        /// <summary>
        /// This is the tick count of the last time the client priority was calculated.
        /// </summary>
        public int? PriorityTickCount { get; set; }
        #endregion
        #region PriorityQueueLength
        /// <summary>
        /// This is the queue length last time the priority was calculated.
        /// </summary>
        public long? PriorityQueueLength { get; private set; }
        #endregion


        #region LastPollTimeSpan
        /// <summary>
        /// This is the calcualted time span from the last time the priority was calculated.
        /// </summary>
        public TimeSpan? LastPollTimeSpan
        {
            get
            {
                if (!LastPollTickCount.HasValue)
                    return null;

                return TimeSpan.FromMilliseconds(StatsContainer.CalculateDelta(Environment.TickCount, LastPollTickCount.Value));
            }
        }
        #endregion
        #region LastPollTickCount
        /// <summary>
        /// This is the tickcount from the last time the client was polled for incoming requests.
        /// </summary>
        public int? LastPollTickCount { get; private set; }
        #endregion

        #region CapacityReset()
        /// <summary>
        /// This method is used to reset the capacity calculation.
        /// </summary>
        public void CapacityReset()
        {
            mPollAttemptedBatch = 0;
            mPollAchievedBatch = 0;
            mCapacityPercentage = 0.75D;
        }
        #endregion
        #region CalculatedMaximumPollWait
        /// <summary>
        /// This method is used to reduce the poll interval when the client reaches a certain success threshold
        /// for polling frequency, which is set of an increasing scale at 75%.
        /// </summary>
        public TimeSpan CalculatedMaximumPollWait
        {
            get
            {
                var rate = PollSuccessRate;
                //If we have a poll success rate under the threshold then return the maximum value.
                if (!mPollTimeReduceRatio.HasValue || rate < mPollTimeReduceRatio.Value)
                    return mMaxAllowedPollWait;

                decimal adjustRatio = ((1M - rate) / (1M - mPollTimeReduceRatio.Value)); //This tends to 0 when the rate hits 100%

                double minTime = mMinExpectedPollWait.TotalMilliseconds;
                double maxTime = mMaxAllowedPollWait.TotalMilliseconds;
                double difference = maxTime - minTime;

                if (difference <= 0)
                    return TimeSpan.FromMilliseconds(minTime);

                double newWait = (double)((decimal)difference * adjustRatio);

                return TimeSpan.FromMilliseconds(minTime + newWait);
            }
        }
        #endregion

        #region Status
        /// <summary>
        /// This is the current status for the holder.
        /// </summary>
        public string Status
        {
            get
            {
                return string.Format("Ratio: {0}/{1} Failed:{2} Hits:{3}/{4}", mPollAchieved, mPollAttempted, mPollException, mPollIn, mPollOut);
            }
        }
        #endregion
    }
}
