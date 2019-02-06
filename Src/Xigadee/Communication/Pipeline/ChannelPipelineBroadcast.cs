namespace Xigadee
{
    /// <summary>
    /// This is the outgoing channel.
    /// </summary>
    public class ChannelPipelineBroadcast<P>: ChannelPipelineBase<P>, IPipelineChannelBroadcast<P>
        where P: IPipeline
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="channel">THe channel.</param>
        public ChannelPipelineBroadcast(P pipeline, Channel channelListener, Channel channelSender) : base(pipeline, null)
        {
            ChannelListener = channelListener;
            ChannelSender = channelSender;
        }


        /// <summary>
        /// This is the registered channel
        /// </summary>
        public Channel ChannelListener { get; }


        /// <summary>
        /// This is the registered channel
        /// </summary>
        public Channel ChannelSender { get; }
    }
}
