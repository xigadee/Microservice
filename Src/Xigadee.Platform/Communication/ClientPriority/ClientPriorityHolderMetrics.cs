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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the client priority metrics.
    /// </summary>
    public class ClientPriorityHolderMetrics: StatisticsBase<ClientPriorityHolderMetricsStatistics>
        , IClientPriorityHolderMetrics
    {
        #region Declarations
        long mPollIn, mPollOut;

        long mPollAttempted, mPollAchieved;

        long mPollAttemptedBatch, mPollAchievedBatch;

        int mPollException;

        double mCapacityPercentage;

        private int mSkipCount = 0;

        private int mPolls, mPollsSuccess;
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
            , int priority
            , decimal weighting)
        {
            Algorithm = algorithm;
            RateLimiter = rateLimiter;
            Priority = priority;
            PriorityWeighting = weighting;

            PollTimeReduceRatio = algorithm.PollTimeReduceRatio;

            MaxAllowedPollWait = algorithm.MaxAllowedWaitBetweenPolls;
            MinExpectedPollWait = algorithm.MinExpectedWaitBetweenPolls;

            if (MinExpectedPollWait > MaxAllowedPollWait)
                MinExpectedPollWait = MaxAllowedPollWait;

            mCapacityPercentage = algorithm.CapacityPercentage;
        }
        #endregion

        protected override void StatisticsRecalculate(ClientPriorityHolderMetricsStatistics stats)
        {
            stats.CapacityPercentage = CapacityPercentage;

            stats.LastOffered = LastOffered;
            stats.LastReserved = LastReserved;
            stats.LastActual = LastActual;
            stats.LastActualTime = LastActualTime;
            
            stats.FabricPollWaitTime = TimeSpan.FromMilliseconds(FabricPollWaitTime ?? 0).ToFriendlyString();
            stats.LastPoll = ConversionHelper.DeltaAsTimeSpan(LastPollTickCount).ToFriendlyString("Not polled");

            stats.MaxAllowedPollWait = MaxAllowedPollWait.ToFriendlyString();
            stats.MinExpectedPollWait = MinExpectedPollWait.ToFriendlyString();
            stats.PollAchievedBatch = PollAchievedBatch;
            stats.PollAttemptedBatch = PollAttemptedBatch;
            stats.PollSuccessRate = $"{PollSuccessRate.ToString("#0.##%")}";
            stats.PollTimeReduceRatio = $"{(PollTimeReduceRatio ?? 0).ToString("#0.##%")}";
            stats.Priority = Priority;
            stats.PriorityCalculated = PriorityCalculated;
            stats.QueueLength = PriorityQueueLength;
            stats.PriorityRecalculated = ConversionHelper.DeltaAsTimeSpan(PriorityTickCount).ToFriendlyString("Not recalculated");
            stats.PriorityWeighting = PriorityWeighting;
            stats.SkipCount = SkipCount;
            stats.Status = Status;
        }

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

        #region MaxAllowedPollWait
        /// <summary>
        /// This is the maximum wait time that the client can wait before it is polled.
        /// </summary>
        public TimeSpan MaxAllowedPollWait { get; set; }
        #endregion
        #region MinExpectedPollWait
        /// <summary>
        /// This is the minimum wait time that the client should wait before it is polled.
        /// </summary>
        public TimeSpan MinExpectedPollWait { get; set; } 
        #endregion

        public decimal? PollTimeReduceRatio
        {
            get; set;
        }

        #region PollAttemptedBatch
        /// <summary>
        /// This is the number of poll attempts for the current batch.
        /// </summary>
        public long PollAttemptedBatch
        {
            get { return mPollAttemptedBatch; }
            set
            {
                Interlocked.Exchange(ref mPollAttemptedBatch, value);
            }
        }
        #endregion
        #region PollAchievedBatch
        /// <summary>
        /// This is the number of successful polls for the current batch.
        /// </summary>
        public long PollAchievedBatch
        {
            get { return mPollAchievedBatch; }
            set
            {
                Interlocked.Exchange(ref mPollAchievedBatch, value);
            }
        }
        #endregion

        #region IsPollPastDue
        /// <summary>
        /// This property specifies whether the time since the last poll has exceeded the maximum time.
        /// </summary>
        public bool IsPollPastDue
        {
            get
            {
                return Algorithm.PastDueCalculate(this);
            }
        } 
        #endregion

        #region SkipCount
        /// <summary>
        /// This is the current skip count for the holder, i.e. the number of poll cycles 
        /// that will be missed until the underlying fabric is polled for new messages.
        /// </summary>
        public int SkipCount
        {
            get { return mSkipCount; }
            set
            {
                Interlocked.Exchange(ref mSkipCount, value);
            }
        }
        #endregion
        #region SkipCountDecrement()
        /// <summary>
        /// This method decrements the skip count.
        /// </summary>
        /// <returns>The method returns true if the skipcount is still positive, or false if the skip count is 0 or below.</returns>
        public bool SkipCountDecrement()
        {
            //Check whether the skip count is greater that zero, and if so then skip
            if (Interlocked.Decrement(ref mSkipCount) > 0)
                return true;

            return false;
        }
        #endregion
        #region ShouldSkip()
        /// <summary>
        /// This method returns true if the client should be skipped for this poll
        /// using the logic in the underlying algorithm.
        /// </summary>
        /// <returns>Returns true if the poll should be skipped.</returns>
        public bool ShouldSkip()
        {
            return Algorithm.ShouldSkip(this);
        }
        #endregion

        #region Reserve(int available)
        /// <summary>
        /// This method reserves a number of slots.
        /// </summary>
        /// <param name="available"></param>
        /// <returns>Returns the number of slots reserved.</returns>
        public int Reserve(int available)
        {
            int takenCalc = Algorithm.CalculateSlots(available, this);

            LastOffered = available;
            LastPollTickCount = Environment.TickCount;
            Interlocked.Increment(ref mPollIn);
            LastReserved = takenCalc;
            return takenCalc;
        } 
        #endregion

        #region LastReserved
        /// <summary>
        /// This is the last reserved slot count.
        /// </summary>
        public int? LastReserved { get; set; } 
        #endregion

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

        #region FabricPollWaitTime
        /// <summary>
        /// This is the time that the fabric should wait for a poll to receive a message.
        /// </summary>
        public int? FabricPollWaitTime { get; set; } 
        #endregion

        #region PollBegin(int reserved)
        /// <summary>
        /// This methid signals the start of a poll.
        /// </summary>
        /// <param name="reserved">The reserved slots count.</param>
        /// <returns>Returns the poll time.</returns>
        public int? PollBegin(int reserved)
        {
            Interlocked.Add(ref mPollAttempted, reserved);
            Interlocked.Add(ref mPollAttemptedBatch, reserved);

            Interlocked.Increment(ref mPolls);

            return FabricPollWaitTime;
        } 
        #endregion
        #region PollEnd(int payloadCount, bool hasErrored)
        /// <summary>
        /// This method signals the end of a poll.
        /// </summary>
        /// <param name="payloadCount">The payload count.</param>
        /// <param name="hasErrored">A flag indicating whether the poll errored.</param>
        public void PollEnd(int payloadCount, bool hasErrored)
        {
            Interlocked.Add(ref mPollAchieved, payloadCount);
            Interlocked.Add(ref mPollAchievedBatch, payloadCount);

            LastActual = payloadCount;
            LastActualTime = DateTime.UtcNow;

            if (payloadCount > 0)
                Interlocked.Increment(ref mPollsSuccess);

            Algorithm.PollMetricsRecalculate(payloadCount > 0, hasErrored, this);
        }
        #endregion

        #region LastActual
        /// <summary>
        /// This is the amount of slots last offered 
        /// </summary>
        public int? LastOffered { get; private set; }
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
            return Algorithm.PriorityRecalculate(queueLength, this, Environment.TickCount);
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
        public long? PriorityQueueLength { get; set; }
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
