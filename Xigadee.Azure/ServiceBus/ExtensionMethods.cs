using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class AzureServiceBusExtensionMethods
    {
        private static string ServiceBusConnectionValidate(this IEnvironmentConfiguration Configuration, string serviceBusConnection)
        {
            var conn = serviceBusConnection ?? Configuration.ServiceBusConnection();

            if (string.IsNullOrEmpty(conn))
                throw new ArgumentNullException("Service bus connection string cannot be resolved. Please check the config settings has been set.");

            return conn;
        }

        public static ChannelPipelineIncoming AddAzureSBQueueListener(this ChannelPipelineIncoming cpipe
            , string connectionName
            , string serviceBusConnection = null
            , bool isDeadLetterListener = false
            , string mappingChannelId = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBQueueListener> onCreate = null
            , bool setFromChannelProperties = true)
        {
            var component = new AzureSBQueueListener(
                  cpipe.Channel.Id
                , cpipe.Pipeline.Configuration.ServiceBusConnectionValidate(serviceBusConnection)
                , connectionName
                , priorityPartitions
                , isDeadLetterListener
                , mappingChannelId
                , resourceProfiles
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.AddListener(component, setFromChannelProperties);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AddAzureSBQueueSender(this ChannelPipelineOutgoing cpipe
            , string connectionName
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , string serviceBusConnection = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBQueueSender> onCreate = null
            , bool setFromChannelProperties = true)
        {
            var component = new AzureSBQueueSender(
                  cpipe.Channel.Id
                , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
                , connectionName
                , priorityPartitions
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.AddSender(component, setFromChannelProperties);

            return cpipe;
        }

        public static ChannelPipelineIncoming AddAzureSBTopicListener(this ChannelPipelineIncoming cpipe
            , string connectionName
            , string serviceBusConnection = null
            , string subscriptionId = null
            , bool isDeadLetterListener = false
            , bool deleteOnStop = true
            , bool listenOnOriginatorId = false
            , string mappingChannelId = null
            , TimeSpan? deleteOnIdleTime = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IBoundaryLogger boundaryLogger = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , Action<AzureSBTopicListener> onCreate = null
            , bool setFromChannelProperties = true)
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName cannot be null.");

            var component = new AzureSBTopicListener(
                  cpipe.Channel.Id
                , cpipe.Pipeline.Configuration.ServiceBusConnectionValidate(serviceBusConnection)
                , connectionName
                , priorityPartitions
                , subscriptionId
                , isDeadLetterListener
                , deleteOnStop
                , listenOnOriginatorId
                , mappingChannelId
                , deleteOnIdleTime
                , resourceProfiles
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);
            
            onCreate?.Invoke(component);

            cpipe.AddListener(component, setFromChannelProperties);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AddAzureSBTopicSender(this ChannelPipelineOutgoing cpipe
            , string connectionName
            , string serviceBusConnection = null
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBTopicSender> onCreate = null
            , bool setFromChannelProperties = true
            )
        {
            if (connectionName == null)
                throw new ArgumentNullException("connectionName cannot be null.");

            var component = new AzureSBTopicSender(
                  cpipe.Channel.Id
                , cpipe.Pipeline.Configuration.ServiceBusConnectionValidate(serviceBusConnection)
                , connectionName
                , priorityPartitions
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.AddSender(component, setFromChannelProperties);

            return cpipe;
        }

    }
}
