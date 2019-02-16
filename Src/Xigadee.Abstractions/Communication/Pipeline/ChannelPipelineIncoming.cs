namespace Xigadee
{
    /// <summary>
    /// This is the incoming channel.
    /// </summary>
    public class ChannelPipelineIncoming<P>: ChannelPipelineBase<P>, IPipelineChannelIncoming<P>
        where P: IPipeline
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channel">The channel.</param>
        public ChannelPipelineIncoming(P pipeline, Channel channel) : base(pipeline, channel)
        {

        }
    }
}
