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
    public class ChannelPipelineOutgoing<P>: ChannelPipelineBase<P>, IPipelineChannelOutgoing<P>
        where P: IPipeline
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channel">THe channel.</param>
        public ChannelPipelineOutgoing(P pipeline, Channel channel) : base(pipeline, channel)
        {

        }
    }
}
