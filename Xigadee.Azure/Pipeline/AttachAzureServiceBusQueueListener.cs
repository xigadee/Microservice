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
            }
}
