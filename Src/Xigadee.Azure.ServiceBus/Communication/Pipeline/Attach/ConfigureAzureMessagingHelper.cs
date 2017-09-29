using System.Collections.Generic;

namespace Xigadee
{
    public static partial class AzureServiceBusExtensionMethods
    {

        /// <summary>
        /// Configures the azure messaging.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="component">The component.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="priorityPartitions">The priority partitions.</param>
        /// <param name="resourceProfiles">The resource profiles.</param>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="serviceBusConnection">The service bus connection.</param>
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
