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
        public static ChannelPipelineOutgoing AddChannelOutgoing(this MicroservicePipeline pipeline
            , string channelId
            , string description = null
            , IEnumerable<SenderPartitionConfig> partitions = null
            , IBoundaryLogger bLogger = null
            , bool internalOnly = false
            , Action<ChannelPipelineOutgoing, Channel> assign = null
            )
        {
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Outgoing, description, bLogger, internalOnly));

            if (partitions != null)
                channel.Partitions = partitions.ToList();

            var cpipe = new ChannelPipelineOutgoing(pipeline, channel);

            assign?.Invoke(cpipe, cpipe.Channel);

            return cpipe;
        }
    }
}
