using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the pipeline configuration.
    /// </summary>
    public abstract class ChannelPipelineBase<P>: MicroservicePipelineExtension<P>, IPipelineChannel<P>
        where P:IPipeline
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channel">The channel.</param>
        public ChannelPipelineBase(P pipeline, Channel channel):base(pipeline)
        {
            Channel = channel;
        }

        /// <summary>
        /// This is the registered channel
        /// </summary>
        public Channel Channel { get; }
    }
}
