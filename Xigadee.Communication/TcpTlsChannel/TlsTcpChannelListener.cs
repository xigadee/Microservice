using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TcpTlsChannelListener: MessagingListenerBase<TcpTlsConnection, TcpTlsMessage, TcpTlsClientHolder>
    {
        public TcpTlsChannelListener(string channelId, IEnumerable<ListenerPartitionConfig> priorityPartitions = null, string mappingChannelId = null, IEnumerable<ResourceProfile> resourceProfiles = null, IBoundaryLogger boundaryLogger = null) : base(channelId, priorityPartitions, mappingChannelId, resourceProfiles, boundaryLogger)
        {
        }
    }
}
