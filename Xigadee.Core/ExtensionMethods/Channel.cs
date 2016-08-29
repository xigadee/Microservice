using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelExtensionMethods
    {
        public static ChannelPipelineIncoming AddChannelIncoming(this MicroservicePipeline pipeline
            , string channelId, string description = null, IBoundaryLogger bLogger = null)
        {     
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Incoming, description));

            if (bLogger != null)
                channel.BoundaryLogger = bLogger;

            return new ChannelPipelineIncoming(pipeline, channel);
        }

        public static ChannelPipelineOutgoing AddChannelOutgoing(this MicroservicePipeline pipeline
            , string channelId, string description = null, IBoundaryLogger bLogger = null)
        {
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Outgoing, description));

            if (bLogger != null)
                channel.BoundaryLogger = bLogger;

            return new ChannelPipelineOutgoing(pipeline, channel);
        }
    }
}
