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
    /// This is the default slot allocation algorithm.
    /// </summary>
    public class MultipleClientPollSlotAllocationAlgorithm: ListenerClientPollAlgorithmBase
    {
        #region CalculateMaximumPollWait(ClientPriorityHolder context)
        /// <summary>
        /// This method is used to reduce the poll interval when the client reaches a certain success threshold
        /// for polling frequency, which is set of an increasing scale at 75%.
        /// </summary>
        public void CalculateMaximumPollWait(ClientPriorityHolder context)
        {
            //var rate = PollSuccessRate;
            ////If we have a poll success rate under the threshold then return the maximum value.
            //if (!mPollTimeReduceRatio.HasValue || rate < mPollTimeReduceRatio.Value)
            //    return mMaxAllowedPollWait;

            //decimal adjustRatio = ((1M - rate) / (1M - mPollTimeReduceRatio.Value)); //This tends to 0 when the rate hits 100%

            //double minTime = mMinExpectedPollWait.TotalMilliseconds;
            //double maxTime = mMaxAllowedPollWait.TotalMilliseconds;
            //double difference = maxTime - minTime;

            //if (difference <= 0)
            //    return TimeSpan.FromMilliseconds(minTime);

            //double newWait = (double)((decimal)difference * adjustRatio);

            //return TimeSpan.FromMilliseconds(minTime + newWait);
        }
        #endregion

        #region CalculatePriority(ClientPriorityHolder context)
        /// <summary>
        /// This is the priority based on the elapsed poll tick time and the overall priority.
        /// It is used to ensure that clients with the overall same base priority are accessed 
        /// so the one polled last is then polled first the next time.
        /// </summary>
        public long CalculatePriority(ClientPriorityHolder context)
        {
            long priority = (context.IsDeadletter ? 0xFFFFFFFF : 0xFFFFFFFFFFFF);

            //try
            //{
            //    if (context.PriorityTickCount.HasValue)
            //        priority += StatsContainer.CalculateDelta(Environment.TickCount, context.PriorityTickCount.Value);

            //    context.PriorityTickCount = Environment.TickCount;

            //    //Add the queue length to add the listener with the greatest number of messages.
            //    context.PriorityQueueLength = context.Client.QueueLength();
            //    priority += context.PriorityQueueLength ?? 0;

            //    priority = (long)((decimal)priority * Client.Weighting);
            //}
            //catch (Exception)
            //{
            //}

            //PriorityCalculated = priority;
            return priority;
        }
        #endregion

        #region ShouldSkip(ClientPriorityHolder context)
        /// <summary>
        /// This method returns true if the client should be skipped for this poll.
        /// </summary>
        /// <returns>Returns true if the poll should be skipped.</returns>
        public bool ShouldSkip(ClientPriorityHolder context)
        {
            ////Get the timespan since the last poll
            //var lastPollTimeSpan = context.LastPollTimeSpan;

            ////Check whether we have waited the minimum poll time, if not skip
            ////if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value < context.MinExpectedPollWait)
            ////    return true;

            ////Check whether we have waited over maximum poll time, then poll
            //if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value > context.CalculatedMaximumPollWait)
            //    return false;

            //Check whether the skip count is greater that zero, and if so then skip
            //if (Interlocked.Decrement(ref mSkipCount) > 0)
            //    return true;

            return false;
        }
        #endregion

        #region CapacityReset(ClientPriorityHolder context)
        /// <summary>
        /// This method is used to reset the capacity calculation.
        /// </summary>
        public void CapacityReset(ClientPriorityHolder context)
        {
            //mPollAttemptedBatch = 0;
            //mPollAchievedBatch = 0;
            //mCapacityPercentage = 0.75D;
        }
        #endregion
    }
}
