using System.Collections.Generic;

namespace Xigadee
{
    public static partial class AzureServiceBusExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="component"></param>
        /// <param name="channelId"></param>
        /// <param name="priorityPartitions"></param>
        /// <param name="resourceProfiles"></param>
        /// <param name="connectionName"></param>
        /// <param name="serviceBusConnection"></param>
        public static void ConfigureAzureMessaging<P>(this IAzureServiceBusMessagingService<P> component
            , string channelId
            , IEnumerable<P> priorityPartitions
            , IEnumerable<ResourceProfile> resourceProfiles
            , string connectionName
            , string serviceBusConnection)
            where P : PartitionConfig
        {
            //Set the standard properties.
            component.ConfigureMessaging(channelId, priorityPartitions, resourceProfiles);

            component.Connection = new AzureServiceBusConnection(connectionName, serviceBusConnection);
        }
    }
}
