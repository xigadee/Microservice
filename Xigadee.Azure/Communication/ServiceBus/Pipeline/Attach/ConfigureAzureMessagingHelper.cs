using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigureAzureMessagingHelper
    {
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
