using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base interface implemented by listeners.
    /// </summary>
    public interface IListener : IMessaging
    {
        /// <summary>
        /// This is a list of active clients.
        /// </summary>
        IEnumerable<IClientHolder> ListenerClients { get; }

        /// <summary>
        /// This method is used to change the supported command endpoint addresses. This happens when a command starts or stops in the microservice.
        /// </summary>
        /// <param name="supported">The supported command destinations.</param>
        void ListenerCommandsActiveChange(List<MessageFilterWrapper> supported);

        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        string ListenerMappingChannelId { get; set;}

        /// <summary>
        /// This is the list of resource profiles attached to the listener.
        /// Resource profiles are use to implement rate limiting for incoming requests.
        /// </summary>
        List<ResourceProfile> ListenerResourceProfiles {get;set;}

        /// <summary>
        /// This contains the listener priority partitions.
        /// </summary>
        List<ListenerPartitionConfig> ListenerPriorityPartitions { get; set; }

        /// <summary>
        /// This boolean property determines whether the listener supports polling.
        /// </summary>
        bool ListenerPollSupported { get; }

        /// <summary>
        /// This boolean property determines whether the listener requires a poll
        /// </summary>
        bool ListenerPollRequired { get; }

        /// <summary>
        /// This is the async poll.
        /// </summary>
        /// <returns>Async.</returns>
        Task ListenerPoll();
    }
}
