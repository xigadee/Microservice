using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// These methods can be used to create a channel.
    /// </summary>
    public static class ChannelExtensionMethods
    {
        public static ChannelPipelineIncoming AddChannelIncoming(this MicroservicePipeline pipeline
            , string channelId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<ChannelPipelineIncoming> assign = null
            )
        {     
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Incoming, description, bLogger, internalOnly));

            if (partitions != null)
                channel.Partitions = partitions.ToList();
            if (resourceProfiles != null)
                channel.ResourceProfiles = resourceProfiles.ToList();

            var cpipe = new ChannelPipelineIncoming(pipeline, channel);

            assign?.Invoke(cpipe);

            return cpipe;
        }

        public static ChannelPipelineOutgoing AddChannelOutgoing(this MicroservicePipeline pipeline
            , string channelId
            , string description = null
            , IEnumerable<SenderPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , bool internalOnly = false
            , Action<ChannelPipelineOutgoing> assign = null
            )
        {
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Outgoing, description, bLogger, internalOnly));

            if (partitions != null)
                channel.Partitions = partitions.ToList();

            var cpipe = new ChannelPipelineOutgoing(pipeline, channel);

            assign?.Invoke(cpipe);

            return cpipe;
        }

        public static MicroservicePipeline Revert<C>(this C cpipe
            , Action<C> assign = null)
            where C: ChannelPipelineBase
        {
            assign?.Invoke(cpipe);

            return cpipe.Pipeline;
        }
    }
}
