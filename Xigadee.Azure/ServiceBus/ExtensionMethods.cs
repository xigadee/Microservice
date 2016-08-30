using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// These extension methods connect the service bus listeners in to the pipeline.
    /// </summary>
    public static class AzureServiceBusExtensionMethods
    {
        #region ServiceBusConnectionValidate(this IEnvironmentConfiguration Configuration, string serviceBusConnection)
        /// <summary>
        /// This method validates that the service bus connection is set.
        /// </summary>
        /// <param name="Configuration">The configuration.</param>
        /// <param name="serviceBusConnection">The alternate connection.</param>
        /// <returns>Returns the connection from either the parameter or from the settings.</returns>
        private static string ServiceBusConnectionValidate(this IEnvironmentConfiguration Configuration, string serviceBusConnection)
        {
            var conn = serviceBusConnection ?? Configuration.ServiceBusConnection();

            if (string.IsNullOrEmpty(conn))
                throw new ArgumentNullException("Service bus connection string cannot be resolved. Please check the config settings has been set.");

            return conn;
        } 
        #endregion

        public static ChannelPipelineIncoming AttachAzureServiceBusQueueListener(this ChannelPipelineIncoming cpipe
            , string connectionName
            , string serviceBusConnection = null
            , bool isDeadLetterListener = false
            , string mappingChannelId = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBQueueListener> onCreate = null)
        {

            var component = new AzureSBQueueListener(
                  cpipe.Channel.Id
                , cpipe.Pipeline.Configuration.ServiceBusConnectionValidate(serviceBusConnection)
                , connectionName
                , priorityPartitions ?? cpipe.Channel.Partitions.Cast<ListenerPartitionConfig>().ToList()
                , isDeadLetterListener
                , mappingChannelId
                , resourceProfiles ?? cpipe.Channel.ResourceProfiles
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.AttachListener(component, false);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AttachAzureServiceBusQueueSender(this ChannelPipelineOutgoing cpipe
            , string connectionName
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , string serviceBusConnection = null
            , IBoundaryLogger boundaryLogger = null
            , Action<AzureSBQueueSender> onCreate = null)
        {
            var component = new AzureSBQueueSender(
                  cpipe.Channel.Id
                , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
                , connectionName
                , priorityPartitions ?? cpipe.Channel.Partitions.Cast<SenderPartitionConfig>().ToList()
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.AttachSender(component, false);

            return cpipe;
        }

        public static ChannelPipelineIncoming AttachAzureServiceBusTopicListener(this ChannelPipelineIncoming cpipe
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
                , priorityPartitions ?? cpipe.Channel.Partitions.Cast<ListenerPartitionConfig>().ToList()
                , subscriptionId
                , isDeadLetterListener
                , deleteOnStop
                , listenOnOriginatorId
                , mappingChannelId
                , deleteOnIdleTime
                , resourceProfiles ?? cpipe.Channel.ResourceProfiles
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);
            
            onCreate?.Invoke(component);

            cpipe.AttachListener(component, setFromChannelProperties);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AttachAzureServiceBusTopicSender(this ChannelPipelineOutgoing cpipe
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
                , priorityPartitions ?? cpipe.Channel.Partitions.Cast<SenderPartitionConfig>().ToList()
                , boundaryLogger ?? cpipe.Channel.BoundaryLogger);

            onCreate?.Invoke(component);

            cpipe.AttachSender(component, setFromChannelProperties);

            return cpipe;
        }

    }
}
