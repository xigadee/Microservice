using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This container contains the listeners and senders for a particular communication protocol.
    /// </summary>
    /// <seealso cref="Xigadee.IMessaging" />
    public abstract class CommunicationAgentContainer<S>: ServiceBase<S>, IMessaging
        where S : CommunicationAgentContainerStatistics, new()
    {
        /// <summary>
        /// This is the channel id for the messaging agent.
        /// </summary>
        public string ChannelId { get; set; }
        /// <summary>
        /// This property specifies whether the boundary logging is active for the messaging service.
        /// </summary>
        public bool? BoundaryLoggingActive { get; set; }

        /// <summary>
        /// The originator Id for the service.
        /// </summary>
        public MicroserviceId OriginatorId { get; set; }
        /// <summary>
        /// This is the system wide service handlers.
        /// </summary>
        public IServiceHandlers ServiceHandlers { get; set; }
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; set; }
        /// <summary>
        /// The shared service container.
        /// </summary>
        public ISharedService SharedServices { get; set; }
    }

    public class CommunicationAgentContainerStatistics: StatusBase
    {

    }
}
