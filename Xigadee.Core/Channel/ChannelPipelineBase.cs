using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the outgoing channel.
    /// </summary>
    public class ChannelPipelineOutgoing: ChannelPipelineBase
    {
        public ChannelPipelineOutgoing(MicroservicePipeline pipeline, Channel channel) : base(pipeline, channel)
        {

        }
    }

    /// <summary>
    /// This is the incoming channel.
    /// </summary>
    public class ChannelPipelineIncoming: ChannelPipelineBase
    {
        public ChannelPipelineIncoming(MicroservicePipeline pipeline, Channel channel) : base(pipeline, channel)
        {

        }
    }

    /// <summary>
    /// This class is used to hold the pipeline configuration.
    /// </summary>
    public abstract class ChannelPipelineBase
    {
        public ChannelPipelineBase(MicroservicePipeline pipeline, Channel channel)
        {
            Pipeline = pipeline;
            Channel = channel;
        }
        /// <summary>
        /// This is the configuration pipeline.
        /// </summary>
        public MicroservicePipeline Pipeline { get; }
        /// <summary>
        /// This is the registered channel
        /// </summary>
        public Channel Channel { get; }

    }
}
