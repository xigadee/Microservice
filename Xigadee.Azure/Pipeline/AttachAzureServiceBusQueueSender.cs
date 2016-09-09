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
    public static partial class AzureExtensionMethods
    {
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
    }
}
