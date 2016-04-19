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
        public TimeSpan MaxAllowedPollWait { get; set; }
        /// <summary>
        /// This is the minimum wait time that the client should wait before it is polled.
        /// </summary>
        public TimeSpan MinExpectedPollWait { get; set; }
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

            MaxAllowedPollWait = algorithm.MaxAllowedPollWait;
            MinExpectedPollWait = algorithm.MinExpectedPollWait;

            if (MinExpectedPollWait > MaxAllowedPollWait)
                MinExpectedPollWait = MaxAllowedPollWait;

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

        public long PollAttemptedBatch
        {
            get { return mPollAttemptedBatch; }
            set
            {
                Interlocked.Exchange(ref mPollAttemptedBatch, value);
            }
        }
        public long PollAchievedBatch
        {
            get { return mPollAchievedBatch; }
            set
            {
                Interlocked.Exchange(ref mPollAchievedBatch, value);
            }
        }
        /// <summary>
        /// This is the current skip count for the holder.
        /// </summary>
        public int SkipCount { get { return mSkipCount; } }

        public bool SkipCountDecrement()
        {
            //Check whether the skip count is greater that zero, and if so then skip
            if (Interlocked.Decrement(ref mSkipCount) > 0)
                return true;

            return false;
        }

        #region ShouldSkip
        /// <summary>
        /// This method returns true if the client should be skipped for this poll.
        /// </summary>
        /// <returns>Returns true if the poll should be skipped.</returns>
        public bool ShouldSkip()
        {
            return Algorithm.ShouldSkip(this);
        }
        #endregion

        public int Reserve(int available)
        {
            int takenCalc = Algorithm.CalculateSlots(available, this);

            LastPollTickCount = Environment.TickCount;
            Interlocked.Increment(ref mPollIn);
            LastReserved = takenCalc;
            return takenCalc;
        }

        public int? LastReserved { get; set; }

        #region Release(bool exception)
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
                Algorithm.CapacityPercentageRecalculate(this);
        } 
        #endregion

        public int PollBegin(int reserved)
        {
            Interlocked.Add(ref mPollAttempted, reserved);
            Interlocked.Add(ref mPollAttemptedBatch, reserved);

            Interlocked.Increment(ref mPolls);

            return (int)MinExpectedPollWait.TotalMilliseconds;
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


        #region CapacityPercentage
        /// <summary>
        /// This is the calcualted percentage that determines how many of the available reserved slots the client will take.
        /// </summary>
        public double CapacityPercentage
        {
            get
            {
                return mCapacityPercentage;
            }
            set
            {
                Interlocked.Exchange(ref mCapacityPercentage, value);
            }
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
            Algorithm.CapacityReset(this);
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
                    return MaxAllowedPollWait;

                decimal adjustRatio = ((1M - rate) / (1M - mPollTimeReduceRatio.Value)); //This tends to 0 when the rate hits 100%

                double minTime = MinExpectedPollWait.TotalMilliseconds;
                double maxTime = MaxAllowedPollWait.TotalMilliseconds;
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
