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

        public class ChannelPipelineOutgoing: ChannelPipelineBase
        {
            public ChannelPipelineOutgoing(MicroservicePipeline pipeline, Channel channel):base(pipeline, channel)
            {

            }
        }

        public class ChannelPipelineIncoming: ChannelPipelineBase
        {
            public ChannelPipelineIncoming(MicroservicePipeline pipeline, Channel channel) : base(pipeline, channel)
            {

            }
        }

        public abstract class ChannelPipelineBase
        {
            public ChannelPipelineBase(MicroservicePipeline pipeline, Channel channel)
            {
                Pipeline = pipeline;
                Channel = channel;
            }

            public MicroservicePipeline Pipeline { get;}

            Channel Channel { get; }
        }
    }
}
