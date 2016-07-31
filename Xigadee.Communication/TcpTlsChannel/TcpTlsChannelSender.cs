using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{


    public class TcpTlsChannelSender: MessagingSenderBase<TcpTlsConnection, TcpTlsMessage, TcpTlsClientHolder>
    {
        public TcpTlsChannelSender(string channelId, IEnumerable<SenderPartitionConfig> priorityPartitions, IBoundaryLogger boundaryLogger = null) : base(channelId, priorityPartitions, boundaryLogger)
        {
        }
    }
}
