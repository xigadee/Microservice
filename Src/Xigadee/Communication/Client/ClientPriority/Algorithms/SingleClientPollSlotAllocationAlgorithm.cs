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
    public class SingleClientPollSlotAllocationAlgorithm: ListenerClientPollAlgorithmBase
    {
        public override int CalculateSlots(int available, IClientPriorityHolderMetrics context)
        {
            //We make sure that a small fraction rate limit adjust resolves to zero as we use ceiling to make even small fractional numbers go to one.
            return available;
        }

        public override bool ShouldSkip(IClientPriorityHolderMetrics context)
        {
            return false;
        }

        public override void CapacityPercentageRecalculate(IClientPriorityHolderMetrics context)
        {
            context.CapacityPercentage = 1D;
        }

        #region CapacityReset(IClientPriorityHolderMetrics context)
        /// <summary>
        /// This method is used to reset the capacity calculation.
        /// </summary>
        public override void CapacityReset(IClientPriorityHolderMetrics context)
        {
            context.PollAttemptedBatch = 0;
            context.PollAchievedBatch = 0;
            context.CapacityPercentage = 1D;
        }
        #endregion

        #region PriorityRecalculate(long? queueLength)
        /// <summary>
        /// This is the priority based on the elapsed poll tick time and the overall priority.
        /// It is used to ensure that clients with the overall same base priority are accessed 
        /// so the one polled last is then polled first the next time.
        /// </summary>
        public override long PriorityRecalculate(long? queueLength, IClientPriorityHolderMetrics context, int? timeStamp = null)
        {
            context.PriorityCalculated = 1;
            return 1;
        }
        #endregion

        public override void PollMetricsRecalculate(bool success, bool hasErrored, IClientPriorityHolderMetrics context)
        {
            context.SkipCount = 0;
        }

        public override void InitialiseMetrics(IClientPriorityHolderMetrics context)
        {
            context.FabricPollWaitTime = (int)FabricPollWaitMax.TotalMilliseconds;
        }

        public override bool PastDueCalculate(IClientPriorityHolderMetrics context, int? timeStamp = default(int?))
        {
            return false;
        }

    }
}
