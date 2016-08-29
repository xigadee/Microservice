using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class AzureServiceBusQueueExtensionMethods
    {
        public static ChannelPipelineIncoming AddAzureSBQueueListener(this ChannelPipelineIncoming cpipe
            , string connectionName
            , IEnumerable<ListenerPartitionConfig> priorityPartitions
            , string serviceBusConnection = null
            , bool isDeadLetterListener = false
            , string mappingChannelId = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBQueueListener> onCreate = null)
        {
            var component = new AzureSBQueueListener(
                  cpipe.Channel.Id
                , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
                , connectionName
                , priorityPartitions
                , isDeadLetterListener
                , mappingChannelId
                , resourceProfiles
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.Pipeline.AddListener(component);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AddAzureSBQueueSender(this ChannelPipelineOutgoing cpipe
            , string connectionName
            , IEnumerable<SenderPartitionConfig> priorityPartitions
            , string serviceBusConnection = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBQueueSender> onCreate = null)
        {
            var component = new AzureSBQueueSender(
                  cpipe.Channel.Id
                , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
                , connectionName
                , priorityPartitions
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.Pipeline.AddSender(component);

            return cpipe;
        }
    }
}
