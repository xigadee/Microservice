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
    /// This class holds the client and associates statistics. 
    /// This also ensures that the client can only have one active poll.
    /// </summary>
    public class ClientPriorityHolder:StatisticsBase<ClientPriorityHolderStatistics>
    {
        #region Declarations
        /// <summary>
        /// This is the private reserve lock used to ensure concurrency when setting the IsActive flag.
        /// </summary>
        private object mReserveLock = new object();

        long mPollIn;
        long mPollOut;

        long mPollAttempted;
        long mPollAchieved;
        long mPollAttemptedBatch;
        long mPollAchievedBatch;

        int mPollException;
        double mCapacityPercentage;

        string mMappingChannel;

        private int mSkipCount = 0;

        private int mPolls, mPollsSuccess;

        private decimal? mPollTimeReduceRatio;

        IResourceRequestRateLimiter mLimiter;

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
        /// This is the main constructor.
        /// </summary>
        /// <param name="resourceTracker">The resource tracker.</param>
        /// <param name="client">The client to hold.</param>
        /// <param name="mappingChannelId">The mapping channel.param>
        /// <param name="maxAllowedPollWait">The maximum permitted poll length.</param>
        public ClientPriorityHolder(IResourceTracker resourceTracker
            , ClientHolder client, string mappingChannelId
            , TimeSpan? maxAllowedPollWait = null
            , TimeSpan? minExpectedPollWait = null
            , decimal? pollTimeReduceRatio = 0.75M
            )
        {
            if (client == null)
                throw new ArgumentNullException("Client");

            mPollTimeReduceRatio = pollTimeReduceRatio;

            mMaxAllowedPollWait = maxAllowedPollWait ?? TimeSpan.FromSeconds(1);
            mMinExpectedPollWait = minExpectedPollWait ?? TimeSpan.FromMilliseconds(100);

            if (mMinExpectedPollWait > mMaxAllowedPollWait)
                mMinExpectedPollWait = mMaxAllowedPollWait;

            mLimiter = resourceTracker.RegisterRequestRateLimiter(client.Name, client.ResourceProfiles);
            mMappingChannel = mappingChannelId;
            Client = client;
            mCapacityPercentage = 0.75D;
        } 
        #endregion

        #region Id
        /// <summary>
        /// This is the same id as the underlying client.
        /// </summary>
        public Guid Id
        {
            get
            {
                return Client.Id;
            }
        }
        #endregion
        #region IsActive
        /// <summary>
        /// A shortcut that defines whether the client is active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Client.IsActive;
            }
        } 
        #endregion
        #region IsDeadletter
        /// <summary>
        /// A shortcut that identifies whether the client is a deadletter
        /// </summary>
        public bool IsDeadletter
        {
            get
            {
                return Client.IsDeadLetter;
            }
        }
        #endregion

        #region Client
        /// <summary>
        /// This is the underlying client.
        /// </summary>
        public ClientHolder Client { get; private set; }
        #endregion

        #region PriorityTickCount
        /// <summary>
        /// This is the tick count of the last time the client priority was calculated.
        /// </summary>
        public int? PriorityTickCount { get; set; }
        #endregion
        #region PriorityCurrent
        /// <summary>
        /// This is the current client priority.
        /// </summary>
        public long? PriorityCurrent { get; set; }
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

                decimal adjustRatio = ((1M - rate) /(1M - mPollTimeReduceRatio.Value)); //This tends to 0 when the rate hits 100%

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

        #region LastPollTickCount
        /// <summary>
        /// This is the tickcount from the last time the client was polled for incoming requests.
        /// </summary>
        public int? LastPollTickCount { get; set; }
        #endregion
        #region LastPollId
        /// <summary>
        /// This is the pollId at the point the reservation is made. This is used as a concurrency check to ensure the locking is correct.
        /// </summary>
        public long? LastPollId { get; set; }
        #endregion
        #region LastReserved
        /// <summary>
        /// This is the last reserved slot value for the client.
        /// </summary>
        public int? LastReserved { get; set; }
        #endregion
        #region LastActual
        /// <summary>
        /// This is the last actual slot value for the client.
        /// </summary>
        public int? LastActual { get; private set; }
        #endregion

        #region LastException
        /// <summary>
        /// This is the last recorded exception that occurred during polling
        /// </summary>
        public Exception LastException { get; set; }
        public DateTime? LastExceptionTime { get; set; }
        #endregion

        #region ShouldSkip()
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

        #region --> IsReserved
        /// <summary>
        /// This boolean property identifies whether the client is currently reserved and active.
        /// </summary>
        public bool IsReserved { get; private set; }
        #endregion
        #region CapacityPercentage
        /// <summary>
        /// This is the calcualted percentage that determines how many of the available reserved slots the client will take.
        /// </summary>
        public double CapacityPercentage { get { return mCapacityPercentage; } } 
        #endregion

        #region Reserve(int capacity, out int taken, out long identifier)
        /// <summary>
        /// This method reserves the client, and returns the number of slots that it has taken based on
        /// previous history.
        /// </summary>
        /// <param name="capacity">The available capacity.</param>
        /// <returns>Returns the number of slots taken.</returns>
        public bool Reserve(int capacity, out int taken, out long identifier)
        {
            taken = 0;
            identifier = mPollIn;

            if (!IsActive)
                return false;

            lock (mReserveLock)
            {
                /// <summary>
                //Let's set thread safe barrier.
                if (IsReserved)
                    return false;

                //Ok, set a barrier to stop other processes getting in.
                IsReserved = true;
            }

            //Check that we won the interlocked battle
            //There are multiple threads running around here, so it pays to be paranoid.
            if (IsReserved)
            {
                identifier = Interlocked.Increment(ref mPollIn);

                double ratelimitAdjustment = 1D;

                if (Client.SupportsRateLimiting && mLimiter != null)
                    ratelimitAdjustment = mLimiter.RateLimitAdjustmentPercentage;

                //We make sure that a small fraction rate limit adjust resolves to zero as we use ceiling to make even small fractional numbers go to one.
                int takenCalc = (int)Math.Ceiling((double)capacity * CapacityPercentage * Math.Round(ratelimitAdjustment, 2, MidpointRounding.AwayFromZero));

                if (takenCalc > 0 && IsActive)
                {
                    LastPollTickCount = Environment.TickCount;
                    LastPollId = identifier;
                    LastReserved = taken = takenCalc;
                    return true;
                }
                else
                {
                    //Ok, we won the battle, but nothing to do here.
                    Release(identifier, false);
                }
            }

            return false;
        }
        #endregion
        #region Poll()
        /// <summary>
        /// This method is used to Poll the connection for waiting messages.
        /// </summary>
        /// <param name="wait">The time to wait, by default 50 ms.</param>
        /// <returns>Returns a list of TransmissionPayload objects to process.</returns>
        public async Task<List<TransmissionPayload>> Poll()
        {
            int wait = (int)mMinExpectedPollWait.TotalMilliseconds;

            Interlocked.Add(ref mPollAttempted, LastReserved.Value);
            Interlocked.Add(ref mPollAttemptedBatch, LastReserved.Value);

            Interlocked.Increment(ref mPolls);

            List<TransmissionPayload> payloads = null;
            int payloadCount = 0;
            try
            {
                payloads = await Client.MessagesPull(LastReserved.Value, wait, mMappingChannel);
                payloadCount = payloads?.Count ?? 0;
            }
            catch (Exception ex)
            {
                LastException = ex;
                LastExceptionTime = DateTime.UtcNow;
            }
            finally
            {
                Interlocked.Add(ref mPollAchieved, payloadCount);
                Interlocked.Add(ref mPollAchievedBatch, payloadCount);
                LastActual = payloadCount;
                if (payloadCount > 0)
                    Interlocked.Increment(ref mPollsSuccess);
                RecalculateSkipCount(payloadCount > 0);
            }

            return payloads;
        }

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

        private void RecalculateSkipCount(bool success)
        {
            if (success)
                mSkipCount = 0;
            else
            {
                var rate = PollSuccessRate;
                mSkipCount = rate <= 0 ? (int)50 : (int)Math.Round((decimal)1 / rate);
            }
        }
        #endregion
        #region Release(long slotId, bool exception)
        /// <summary>
        /// This method releases the holder so that it can be polled again.
        /// </summary>
        /// <param name="slotId">The last slot id.</param>
        /// <param name="exception">A flag indicating whether there was an exception.</param>
        /// <returns>Returns true if the holder returned all records requested.</returns>
        public bool Release(long slotId, bool exception)
        {
            if (mPollIn != slotId)
                throw new Exception("ClientPriorityHolder - unexpected concurrency exception - slots don't match");

            lock (mReserveLock)
            {
                Interlocked.Increment(ref mPollOut);

                if (exception)
                    Interlocked.Increment(ref mPollException);
                else
                    //Recalulate statistics.
                    CapacityPercentageRecalculate(LastActual, LastReserved);

                IsReserved = false;
            }

            return false;
        }
        #endregion

        #region CapacityPercentageRecalculate(long LastActual, int LastReserved)
        /// <summary>
        /// This method is used to recalcualte the capacity percentage.
        /// </summary>
        /// <param name="LastActual"></param>
        /// <param name="LastReserved"></param>
        /// <returns></returns>
        private bool CapacityPercentageRecalculate(long? lastActual, int? lastReserved)
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

        #region CalculatePriority()
        /// <summary>
        /// This is the priority based on the elapsed poll tick time and the overall priority.
        /// It is used to ensure that clients with the overall same base priority are accessed 
        /// so the one polled last is then polled first the next time.
        /// </summary>
        public long CalculatePriority()
        {
            long priority = (Client.Priority + 1) * (IsDeadletter ? 0xFFFFFFFFFFFF : 0xFFFFFFFF);

            try
            {
                if (PriorityTickCount.HasValue)
                    priority += StatsContainer.CalculateDelta(Environment.TickCount, PriorityTickCount.Value);

                PriorityTickCount = Environment.TickCount;

                //Add the queue length to add the listener with the greatest number of messages.
                PriorityQueueLength = Client.QueueLength();
                priority += PriorityQueueLength ?? 0;
            }
            catch (Exception)
            {
            }

            PriorityCurrent = priority;
            return priority;
        }
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

        #region Logging
        public void QueueTimeLog(DateTime? EnqueuedTimeUTC)
        {
            Client.QueueTimeLog(EnqueuedTimeUTC);
        }

        public void ActiveIncrement()
        {
            Client.ActiveIncrement();
        }

        public void ActiveDecrement(int TickCount)
        {
            Client.ActiveDecrement(TickCount);
        }

        public void ErrorIncrement()
        {
            Client.ErrorIncrement();
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method calculates the statistics for the client.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            try
            {
                mStatistics.IsReserved = IsReserved;
                mStatistics.LastReserved = LastReserved;
                mStatistics.CapacityPercentage = CapacityPercentage;
                mStatistics.PriorityCurrent = PriorityCurrent;
                mStatistics.Status = string.Format("Ratio: {0}/{1} Failed:{2} Hits:{3}/{4}", mPollAchieved, mPollAttempted, mPollException, mPollIn, mPollOut);
                mStatistics.PollLast = LastPollTimeSpan;
                mStatistics.Name = Client.DebugStatus;
                mStatistics.Client = Client.Statistics;
                mStatistics.MappingChannel = mMappingChannel;
                mStatistics.SkipCount = mSkipCount;

                mStatistics.LastException = LastException;
                mStatistics.LastExceptionTime = LastExceptionTime;

                mStatistics.PollSuccessRate = PollSuccessRate;
            }
            catch (Exception ex)
            {

            }
        } 
        #endregion
    }
}
