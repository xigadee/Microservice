using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigureAzureMessagingHelper
    {
        /// <summary>
        /// Configures the Azure messaging settings.
        /// </summary>
        /// <typeparam name="P">The message type.</typeparam>
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
            component.ConfigureMessaging(channelId
                , priorityPartitions
                , resourceProfiles);

            component.AzureConn = new AzureConnection(connectionName, serviceBusConnection);
        }
    }
}
