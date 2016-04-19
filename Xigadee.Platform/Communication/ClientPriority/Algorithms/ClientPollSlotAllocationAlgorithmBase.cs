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

        /// <summary>
        /// This is the priority recalculate frequency. Leave this null if you do not wish it to recalculate.
        /// </summary>
        public TimeSpan? PriorityRecalculateFrequency { get; set; } = TimeSpan.FromMinutes(10);
    }
}
