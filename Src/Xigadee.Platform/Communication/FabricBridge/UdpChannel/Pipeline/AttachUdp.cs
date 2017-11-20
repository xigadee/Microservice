using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    /// <summary>
    /// These extensions are used to attach a UDP based listener and sender to a channel
    /// </summary>
    public static partial class UdpCommunicationPipelineExtensions
    {
        public static C AttachUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {


            return cpipe;
        }

        public static C AttachMulticastUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {


            return cpipe;
        }

        public static C AttachUdpSender<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {

            //var client = new UdpClient(

            return cpipe;
        }


        public static C AttachMulticastUdpSender<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {

            //var client = new UdpClient(

            return cpipe;
        }
    }
}
