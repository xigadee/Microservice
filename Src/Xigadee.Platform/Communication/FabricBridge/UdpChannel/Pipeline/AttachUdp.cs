using System;
using System.Collections.Generic;
using System.Net;

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
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var listener = new UdpChannelListener(false, ep);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        public static C AttachMulticastUdpListener<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , Action<UdpChannelListener> action = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            var listener = new UdpChannelListener(true, ep);

            cpipe.AttachListener(listener, action, true);

            return cpipe;
        }

        public static C AttachUdpSender<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , Action<UdpChannelSender> action = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var sender = new UdpChannelSender(false, ep);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }


        public static C AttachMulticastUdpSender<C>(this C cpipe
            , IPEndPoint ep
            , string connectionName = null
            , IEnumerable<ListenerPartitionConfig> priorityPartitions = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , Action<UdpChannelSender> action = null
            )
            where C : IPipelineChannelOutgoing<IPipeline>
        {
            var sender = new UdpChannelSender(true, ep);

            cpipe.AttachSender(sender, action, true);

            return cpipe;
        }
    }
}
