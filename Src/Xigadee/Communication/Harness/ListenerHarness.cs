using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    /// <summary>
    /// This abstract harness holds a message listener and allows unit testing to be performed on it.
    /// </summary>
    /// <typeparam name="L">The listener type.</typeparam>
    public abstract class ListenerHarness<L>: MessagingHarness<L>
        where L:class, IListener, IService, IRequireSharedServices
    {
        /// <summary>
        /// This override sets the priority partitions.
        /// </summary>
        /// <param name="service">The service.</param>
        protected override void Configure(L service)
        {
            base.Configure(service);

            service.ListenerPriorityPartitions = PriorityPartitions;
        }

        /// <summary>
        /// Configures the specified configuration for the Azure Service Bus.
        /// </summary>
        /// <param name="configuration">The configuration parameters.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="mappingChannelId">The actual channel id for the fabric queue.</param>
        /// <param name="boundaryLoggingActive">A boolean property that specifies whether boundary logging is active.</param>
        public virtual void Configure(IEnvironmentConfiguration configuration
            , string channelId
            , string mappingChannelId = null
            , bool boundaryLoggingActive = true)
        {
            base.Configure(configuration, channelId, boundaryLoggingActive);
            Service.ListenerMappingChannelId = mappingChannelId;
        }

        /// <summary>
        /// Starts with the specified supported listener message types.
        /// </summary>
        /// <param name="supported">The supported message types.</param>
        public virtual void Start(List<MessageFilterWrapper> supported)
        {
            Start();
            Service.ListenerCommandsActiveChange(supported);
        }
        /// <summary>
        /// Starts with the specified supported listener message type.
        /// </summary>
        /// <param name="supported">The supported message type.</param>
        public virtual void Start(MessageFilterWrapper supported)
        {
            Start(new[]{ supported }.ToList());
        }

        /// <summary>
        /// This method starts the listener and prioritises the clients.
        /// </summary>
        public override void Start()
        {
            base.Start();
            Clients = new ClientPriorityCollection(new IListener[] { Service }.ToList(), null, PollAlgorithm,0);
        }
        /// <summary>
        /// This is the poll algorithm.
        /// </summary>
        public virtual IListenerClientPollAlgorithm PollAlgorithm => new MultipleClientPollSlotAllocationAlgorithm();
        /// <summary>
        /// This is the client priority collection.
        /// </summary>
        public ClientPriorityCollection Clients { get; set; }

        #region PriorityPartitions
        /// <summary>
        /// This is the set of default priority partitions. Override if you wish to change.
        /// </summary>
        public virtual List<ListenerPartitionConfig> PriorityPartitions => ListenerPartitionConfig.Init(0, 1).ToList();
        #endregion
    }
}
