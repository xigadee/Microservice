using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ChannelExtensionMethods
    {
        public static ChannelPipelineIncoming AddChannelIncoming(this MicroservicePipeline pipeline, string channelId, string description = null)
        {     
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Incoming, description));

            return new ChannelPipelineIncoming(pipeline, channel);
        }

        public static ChannelPipelineOutgoing AddChannelOutgoing(this MicroservicePipeline pipeline, string channelId, string description = null)
        {
            var channel = pipeline.Service.RegisterChannel(new Channel(channelId, ChannelDirection.Outgoing, description));

            return new ChannelPipelineOutgoing(pipeline, channel);
        }

        public static ChannelPipelineOutgoing AddBoundaryLogger(this ChannelPipelineOutgoing cpipe, IBoundaryLogger boundaryLogger)
        {
            return cpipe;
        }
    }
}
