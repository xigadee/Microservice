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
    public static partial class CorePipelineExtensions
    {
        public static ChannelPipelineIncoming AddChannelIncoming(this MicroservicePipeline pipeline
            , string channelId
            , string description = null
            , IEnumerable<ListenerPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , IEnumerable<ResourceProfile> resourceProfiles = null
            , bool internalOnly = false
            , Action<ChannelPipelineIncoming, Channel> assign = null
            )
        {     
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Incoming, description, bLogger, internalOnly));

            if (partitions != null)
                channel.Partitions = partitions.ToList();
            if (resourceProfiles != null)
                channel.ResourceProfiles = resourceProfiles.ToList();

            var cpipe = new ChannelPipelineIncoming(pipeline, channel);

            assign?.Invoke(cpipe, cpipe.Channel);

            return cpipe;
        }

    }
}
