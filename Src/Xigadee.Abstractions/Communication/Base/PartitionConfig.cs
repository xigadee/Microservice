#region using
using System;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base class for channel partition.
    /// </summary>
    public abstract class PartitionConfig
    {
        /// <summary>
        /// This is the default abstract constructor for the PartitionConfig class
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="fabricMaxMessageLock">The preferred fabric lock time. If this is not set, then the default of 4min30s is used.</param>
        protected internal PartitionConfig(int priority, TimeSpan? fabricMaxMessageLock = null)
        {
            Priority = priority;
            FabricMaxMessageLock = fabricMaxMessageLock ?? TimeSpan.FromMinutes(4.5d);
        }

        /// <summary>
        /// This is the numeric partition id.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// This is the message lock duration for the underlying fabric
        /// </summary>
        public TimeSpan? FabricMaxMessageLock { get; set; }
    }
}
