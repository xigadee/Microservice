using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    public static class ConfigureMessagingHelper
    {
        ///// <summary>
        ///// This sets the standard settings for the messaging service.
        ///// </summary>
        ///// <typeparam name="P">The partition type.</typeparam>
        ///// <param name="component">The component.</param>
        ///// <param name="channelId">The channel id.</param>
        ///// <param name="priorityPartitions">The partition collection.</param>
        ///// <param name="resourceProfiles">The optional resource profile. This only applies to the listeners.</param>
        //public static void ConfigureMessaging<P>(this IMessagingService<P> component
        //    , string channelId
        //    , IEnumerable<P> priorityPartitions
        //    , IEnumerable<ResourceProfile> resourceProfiles = null
        //    )
        //    where P : PartitionConfig
        //{
        //    component.ChannelId = channelId;

        //    if (priorityPartitions != null)
        //        component.ListenerPriorityPartitions = priorityPartitions.ToList();

        //    if (component is IListener && resourceProfiles != null)
        //        ((IListener)component).ListenerResourceProfiles = resourceProfiles.ToList();
        //}
    }
}
