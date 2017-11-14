using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class UdpCommunicationPipelineExtensions
    {
        public static C AttachUdpListener<C>(this C cpipe
            , string connectionName = null
            , string mappingChannelId = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , Action<TcpTlsChannelListener> onCreate = null)
            where C : IPipelineChannelIncoming<IPipeline>
        {
            //throw new NotImplementedException();



            return cpipe;
        }

    }
}
