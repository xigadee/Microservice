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
    /// This is the interface used by poll algorithms.
    /// </summary>
    public interface IListenerClientPollAlgorithm
    {
        /// <summary>
        /// This is the number of allowed additional slots over the available slots.
        /// </summary>
        int AllowedOverage { get; set; }
        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        TimeSpan? PriorityRecalculateFrequency { get; set; }

        string Name { get; }

        TimeSpan MaxAllowedPollWait { get; set; } 

        TimeSpan MinExpectedPollWait { get; set; }

        decimal PollTimeReduceRatio { get; set; }

        double CapacityPercentage { get; set; }

    }

    /// <summary>
    /// This abstract class is the base class to allocate polling slots to the listener collection.
    /// </summary>
    public abstract class ListenerClientPollAlgorithmBase: IListenerClientPollAlgorithm
    {
        /// <summary>
        /// This property specifies the number of additional slots the clients can poll for on top of the maximum allowed.
        /// </summary>
        public int AllowedOverage { get; set; } = 5;

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
    }
}
