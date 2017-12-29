using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the base interface shared by both listeners and senders.
    /// </summary>
    public interface IMessaging
    {
        /// <summary>
        /// This is a list of active clients.
        /// </summary>
        IEnumerable<ClientHolder> Clients { get; }

        /// <summary>
        /// This is the channel id for the messaging agent.
        /// </summary>
        string ChannelId { get;set; }

        /// <summary>
        /// This is the data collector used to access the service boundary logger.
        /// </summary>
        IDataCollection Collector { get; set; }

        /// <summary>
        /// This property specifies whether the boundary logging is active for the messaging service.
        /// </summary>
        bool? BoundaryLoggingActive { get; set; }
    }
}
