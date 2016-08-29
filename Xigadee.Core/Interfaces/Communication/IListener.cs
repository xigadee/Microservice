using System.Collections.Generic;
namespace Xigadee
{
    /// <summary>
    /// This is the base interface implemented by listeners.
    /// </summary>
    public interface IListener : IMessaging
    {
        /// <summary>
        /// This method is used to change the supported filters. This happens when a command starts or stops in the microservice.
        /// </summary>
        /// <param name="supported"></param>
        void Update(List<MessageFilterWrapper> supported);

        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        string MappingChannelId { get; }

        /// <summary>
        /// This is the list of resource profiles attached to the listener.
        /// Resource profiles are use to implement rate limiting for incoming requests.
        /// </summary>
        List<ResourceProfile> ResourceProfiles {get;set;}

        /// <summary>
        /// This contains the listener priority partitions.
        /// </summary>
        List<ListenerPartitionConfig> PriorityPartitions { get; set; }
    }
}
