using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class TcpCommunicationPipelineExtensions
    {
        public static IPipelineChannelIncoming AttachTcpTlsBroadcastListener(this IPipelineChannelIncoming cpipe
            , string connectionName = null
            , string serviceBusConnection = null
            , string subscriptionId = null
            , bool deleteOnStop = true
            , bool listenOnOriginatorId = false
            , string mappingChannelId = null
            , TimeSpan? deleteOnIdleTime = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IBoundaryLogger boundaryLogger = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , Action<TcpTlsChannelListener> onCreate = null
            , bool setFromChannelProperties = true
            )
        {
            throw new NotImplementedException();
            //var component = new AzureSBQueueListener();

            //component.ConfigureAzureMessaging(
            //      cpipe.Channel.Id
            //    , priorityPartitions ?? cpipe.Channel.Partitions.Cast<ListenerPartitionConfig>()
            //    , resourceProfiles
            //    , connectionName ?? cpipe.Channel.Id
            //    , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
            //    );

            //component.IsDeadLetterListener = isDeadLetterListener;

            //onCreate?.Invoke(component);

            //cpipe.AttachListener(component, false);

            return cpipe;
        }
    }
}
