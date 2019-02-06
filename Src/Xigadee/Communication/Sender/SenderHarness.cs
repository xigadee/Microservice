using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This abstract harness holds a message listener and allows unit testing to be performed on it.
    /// </summary>
    /// <typeparam name="L">The listener type.</typeparam>
    public abstract class SenderHarness<L> : MessagingHarness<L>
        where L : class, ISender, IService
    {
        /// <summary>
        /// This override sets the priority partitions.
        /// </summary>
        /// <param name="service">The service.</param>
        protected override void Configure(L service)
        {
            base.Configure(service);

            //service.ListenerPriorityPartitions = PriorityPartitions;
        }
        /// <summary>
        /// This is the set of default priority partitions. Override if you wish to change.
        /// </summary>
        public virtual List<SenderPartitionConfig> PriorityPartitions => SenderPartitionConfig.Init(0, 1).ToList();

        
    }
}
