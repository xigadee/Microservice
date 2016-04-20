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
    /// This abstract class is the base class to allocate polling slots to the listener collection.
    /// </summary>
    public abstract class ListenerClientPollAlgorithmBase: IListenerClientPollAlgorithm
    {
        /// <summary>
        /// This property specifies the number of additional slots the clients can poll for on top of the maximum allowed.
        /// </summary>
        public int AllowedOverage { get; set; } = 5;

        /// <summary>
        /// This is the algorithm name.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        public TimeSpan? PriorityRecalculateFrequency { get; set; } = TimeSpan.FromMinutes(10);

        public TimeSpan MaxAllowedPollWait { get; set; } = TimeSpan.FromSeconds(1);

        public TimeSpan MinExpectedPollWait { get; set; } = TimeSpan.FromMilliseconds(100);

        public decimal PollTimeReduceRatio { get; set; } = 0.75M;

        public double CapacityPercentage { get; set; } = 0.75D;

        public abstract int CalculateSlots(int available, ClientPriorityHolderMetrics context);

        public abstract bool ShouldSkip(ClientPriorityHolderMetrics context);

        public abstract void CapacityPercentageRecalculate(ClientPriorityHolderMetrics context);

        public abstract void CapacityReset(ClientPriorityHolderMetrics context);

        public abstract long PriorityRecalculate(long? queueLength, ClientPriorityHolderMetrics context, int? timeStamp = null);

        public abstract void SkipCountRecalculate(bool success, ClientPriorityHolderMetrics context);

        public abstract void InitialiseMetrics(ClientPriorityHolderMetrics context);
    }
}
