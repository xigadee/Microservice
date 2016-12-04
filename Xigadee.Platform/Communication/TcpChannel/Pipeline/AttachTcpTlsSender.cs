using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class TcpCommunicationPipelineExtensions
    {
        public static ChannelPipelineOutgoing AttachTcpTlsSender(this ChannelPipelineOutgoing cpipe
            , string connectionName = null
            , IEnumerable<SenderPartitionConfig> priorityPartitions = null
            , string serviceBusConnection = null
            , Action<TcpTlsChannelSender> onCreate = null)
        {
            throw new NotImplementedException();
            //var component = new AzureSBQueueSender();

            //component.ConfigureAzureMessaging(
            //      cpipe.Channel.Id
            //    , priorityPartitions ?? cpipe.Channel.Partitions.Cast<SenderPartitionConfig>()
            //    , null
            //    , connectionName ?? cpipe.Channel.Id
            //    , serviceBusConnection ?? cpipe.Pipeline.Configuration.ServiceBusConnection()
            //    );

            //onCreate?.Invoke(component);

            //cpipe.AttachSender(component, false);

            return cpipe;
        }
    }
}
