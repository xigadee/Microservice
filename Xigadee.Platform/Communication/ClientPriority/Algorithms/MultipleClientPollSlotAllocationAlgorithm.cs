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
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the default slot allocation algorithm.
    /// </summary>
    public class MultipleClientPollSlotAllocationAlgorithm: ListenerClientPollAlgorithmBase
    {

        #region CalculateSlots(int available, ClientPriorityHolderMetrics context)
        /// <summary>
        /// This method calculates the number of slots to take from the amount available.
        /// </summary>
        /// <param name="available">The available slots.</param>
        /// <param name="context">The metrics.</param>
        /// <returns>Returns the number of slots to reserve from the amount available.</returns>
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

        #region PriorityRecalculate(long? queueLength, ClientPriorityHolderMetrics context, int? timeStamp = null)
        /// <summary>
        /// This is the priority based on the elapsed poll tick time and the overall priority.
        /// It is used to ensure that clients with the overall same base priority are accessed 
        /// so the one polled last is then polled first the next time.
        /// </summary>
        /// <param name="queueLength">This contains the current queue length for the underlying fabric.</param>
        /// <param name="context">This is the metrics context.</param>
        /// <param name="timeStamp">This is an optional parameter that defaults to the current tick count. You can set this value for unit testing.</param>
        /// <returns>Returns the new priority.</returns>
        public override long PriorityRecalculate(long? queueLength, ClientPriorityHolderMetrics context, int? timeStamp = null)
        {
            if (!timeStamp.HasValue)
                timeStamp = Environment.TickCount;

            long newPriority = 0xFFFFFFFFFFFF;

            try
            {
                if (context.PriorityTickCount.HasValue)
                    newPriority += ConversionHelper.CalculateDelta(timeStamp.Value, context.PriorityTickCount.Value);

                context.PriorityTickCount = timeStamp.Value;

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

        #region PollMetricsRecalculate(bool success, ClientPriorityHolderMetrics context)
        /// <summary>
        /// This method recalculates the metrics after the poll has returned from the fabric.
        /// </summary>
        /// <param name="success">The flag indicating whether the last poll was successful.</param>
        /// <param name="context">The metrics.</param>
        public override void PollMetricsRecalculate(bool success, bool hasErrored, ClientPriorityHolderMetrics context)
        {
            
            int newwait = (context.FabricPollWaitTime ?? (int)FabricPollWaitMin.TotalMilliseconds);
            if (!success && (context.LastReserved ?? 0) > 0 && (context.PriorityQueueLength ?? 0) > 0)
            {
                newwait += 100;
                if (newwait > (int)FabricPollWaitMax.TotalMilliseconds)
                    newwait = (int)FabricPollWaitMax.TotalMilliseconds;
            }
            else if (success && newwait > (int)FabricPollWaitMin.TotalMilliseconds)
            {
                newwait -= 100;
                if (newwait < (int)FabricPollWaitMin.TotalMilliseconds)
                    newwait = (int)FabricPollWaitMin.TotalMilliseconds;
            }

            context.FabricPollWaitTime = newwait;

            if (!hasErrored)
            {
                var rate = context.PollSuccessRate;
                context.SkipCount = rate <= 0 ? (int)50 : (int)Math.Round((decimal)1 / rate);
            }

        } 
        #endregion
        #region ShouldSkip
        /// <summary>
        /// This method returns true if the client should be skipped for this poll.
        /// </summary>
        /// <returns>Returns true if the poll should be skipped.</returns>
        public override bool ShouldSkip(ClientPriorityHolderMetrics context)
        {
            //Get the timespan since the last poll
            var lastPollTimeSpan = ConversionHelper.DeltaAsTimeSpan(context.LastPollTickCount);

            //Check whether we have waited the minimum poll time, if not skip
            if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value < context.MinExpectedPollWait)
                return true;

            //Check whether we have waited over maximum poll time, then poll
            if (lastPollTimeSpan.HasValue && lastPollTimeSpan.Value > RecalculateMaximumPollWait(context))
                return false;

            return context.SkipCountDecrement();
        }
        #endregion
        #region RecalculateMaximumPollWait(ClientPriorityHolderMetrics context)
        /// <summary>
        /// This method is used to reduce the poll interval when the client reaches a certain success threshold
        /// for polling frequency, which is set of an increasing scale at 75%.
        /// </summary>
        private TimeSpan RecalculateMaximumPollWait(ClientPriorityHolderMetrics context)
        {
            var rate = context.PollSuccessRate;
            //If we have a poll success rate under the threshold then return the maximum value.
            if (!context.PollTimeReduceRatio.HasValue || rate < context.PollTimeReduceRatio.Value)
                return MaxAllowedWaitBetweenPolls;

            decimal adjustRatio = ((1M - rate) / (1M - context.PollTimeReduceRatio.Value)); //This tends to 0 when the rate hits 100%

            double minTime = MinExpectedWaitBetweenPolls.TotalMilliseconds;
            double maxTime = MaxAllowedWaitBetweenPolls.TotalMilliseconds;
            double difference = maxTime - minTime;

            if (difference <= 0)
                return TimeSpan.FromMilliseconds(minTime);

            double newWait = (double)((decimal)difference * adjustRatio);

            return TimeSpan.FromMilliseconds(minTime + newWait);
        }
        #endregion

        public override void InitialiseMetrics(ClientPriorityHolderMetrics context)
        {
            context.FabricPollWaitTime = (int)FabricPollWaitMin.TotalMilliseconds;
        }

        public override bool PastDueCalculate(ClientPriorityHolderMetrics context, int? timeStamp = null)
        {
            if (timeStamp == null)
                timeStamp = Environment.TickCount;

            var timePassed = ConversionHelper.DeltaAsTimeSpan(context.LastPollTickCount, timeStamp);
            return timePassed.HasValue && timePassed.Value > context.MaxAllowedPollWait;
        }

        /// <summary>
        /// This method specifies that the algorithm supports a pass due client scan before the main scan.
        /// </summary>
        public override bool SupportPassDueScan { get { return true; } }
    }
}
