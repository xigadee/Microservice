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
