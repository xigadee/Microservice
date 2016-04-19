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

        #region ShouldSkip
        /// <summary>
        /// This method returns true if the client should be skipped for this poll.
        /// </summary>
        /// <returns>Returns true if the poll should be skipped.</returns>
        public override bool ShouldSkip(ClientPriorityHolderMetrics context)
        {
            //Get the timespan since the last poll
            var lastPollTimeSpan = context.LastPollTimeSpan;

            //Check whether we have waited the minimum poll time, if not skip
            if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value < context.MinExpectedPollWait)
                return true;

            //Check whether we have waited over maximum poll time, then poll
            if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value > CalculatedMaximumPollWait(context))
                return false;

            return context.SkipCountDecrement();

        }
        #endregion

        #region CalculatedMaximumPollWait
        /// <summary>
        /// This method is used to reduce the poll interval when the client reaches a certain success threshold
        /// for polling frequency, which is set of an increasing scale at 75%.
        /// </summary>
        public TimeSpan CalculatedMaximumPollWait(ClientPriorityHolderMetrics context)
        {
            var rate = context.PollSuccessRate;
            //If we have a poll success rate under the threshold then return the maximum value.
            if (!context.PollTimeReduceRatio.HasValue || rate < context.PollTimeReduceRatio.Value)
                return MaxAllowedPollWait;

            decimal adjustRatio = ((1M - rate) / (1M - context.PollTimeReduceRatio.Value)); //This tends to 0 when the rate hits 100%

            double minTime = MinExpectedPollWait.TotalMilliseconds;
            double maxTime = MaxAllowedPollWait.TotalMilliseconds;
            double difference = maxTime - minTime;

            if (difference <= 0)
                return TimeSpan.FromMilliseconds(minTime);

            double newWait = (double)((decimal)difference * adjustRatio);

            return TimeSpan.FromMilliseconds(minTime + newWait);
        }
        #endregion

        #region CalculateSlots(int available, ClientPriorityHolderMetrics context)
        /// <summary>
        /// This method calculates the number of slots to take from the amount available.
        /// </summary>
        /// <param name="available">The available slots.</param>
        /// <param name="context">The metrics.</param>
        /// <returns>Returns the number of slots to take.</returns>
        public override int CalculateSlots(int available, ClientPriorityHolderMetrics context)
        {
            double ratelimitAdjustment = context.RateLimiter?.RateLimitAdjustmentPercentage ?? 1D;

            //We make sure that a small fraction rate limit adjust resolves to zero as we use ceiling to make even small fractional numbers go to one.
            return (int)Math.Ceiling((double)available * CapacityPercentage * Math.Round(ratelimitAdjustment, 2, MidpointRounding.AwayFromZero));
        } 
        #endregion

        #region CapacityReset()
        /// <summary>
        /// This method is used to reset the capacity calculation.
        /// </summary>
        public override void CapacityReset(ClientPriorityHolderMetrics context)
        {
            context.PollAttemptedBatch = 0;
            context.PollAchievedBatch = 0;
            context.CapacityPercentage = 0.75D;
        }
        #endregion

        #region CapacityPercentageRecalculate(ClientPriorityHolderMetrics context)
        /// <summary>
        /// This method is used to recalcualte the capacity percentage.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override void CapacityPercentageRecalculate(ClientPriorityHolderMetrics context)
        {
            if (context.PollAttemptedBatch > 0)
            {
                //If the actual and reserved tend to 100% we want the capacity to grow to 95%
                double capacityPercentage = context.CapacityPercentage * ((double)context.PollAchievedBatch / (double)context.PollAttemptedBatch) * 1.05D;

                if (capacityPercentage >= 0.95D)
                    capacityPercentage = 0.95D;
                else if (capacityPercentage <= 0.01D)
                    capacityPercentage = 0.01D;

                context.CapacityPercentage = capacityPercentage;
            }
        }
        #endregion

        #region PriorityRecalculate(long? queueLength)
        /// <summary>
        /// This is the priority based on the elapsed poll tick time and the overall priority.
        /// It is used to ensure that clients with the overall same base priority are accessed 
        /// so the one polled last is then polled first the next time.
        /// </summary>
        public override long PriorityRecalculate(long? queueLength, ClientPriorityHolderMetrics context)
        {
            long newPriority = (context.IsDeadletter ? 0xFFFFFFFF : 0xFFFFFFFFFFFF);

            try
            {
                if (context.PriorityTickCount.HasValue)
                    newPriority += StatsContainer.CalculateDelta(Environment.TickCount, context.PriorityTickCount.Value);

                context.PriorityTickCount = Environment.TickCount;
                //Add the queue length to add the listener with the greatest number of messages.
                context.PriorityQueueLength = queueLength;

                newPriority += context.PriorityQueueLength ?? 0;

                newPriority = (long)((decimal)newPriority * context.PriorityWeighting);
            }
            catch (Exception ex)
            {
            }

            context.PriorityCalculated = newPriority;
            return newPriority;
        }
        #endregion

        /// <summary>
        /// This method recalculates the skip count based on the success of the last poll.
        /// </summary>
        /// <param name="success">The flag indicating whether the last poll was successful.</param>
        /// <param name="context">The metrics.</param>
        public override void SkipCountRecalculate(bool success, ClientPriorityHolderMetrics context)
        {
            if (success)
                context.SkipCount = 0;
            else
                context.SkipCount = 20;

        }
    }
}
