namespace Xigadee
{
    /// <summary>
    /// This is the generic channel interface
    /// </summary>
    public interface IPipelineChannel<out P>: IPipelineChannel, IPipelineExtension<P>
        where P:IPipeline
    {
    }
    /// <summary>
    /// This is the generic channel interface
    /// </summary>
    public interface IPipelineChannel
    {
        /// <summary>
        /// This is the channel definition.
        /// </summary>
        Channel Channel { get; }
    }

    /// <summary>
    /// This interface is used by incoming pipeline channels.
    /// </summary>
    public interface IPipelineChannelIncoming: IPipelineChannel { }
    /// <summary>
    /// This interface is used by incoming pipeline channels.
    /// </summary>
    /// <typeparam name="P">The pipeline type</typeparam>
    public interface IPipelineChannelIncoming<out P>: IPipelineChannelIncoming,IPipelineChannel<P>
        where P : IPipeline
    {
    }
    /// <summary>
    /// This interface is used by outgoing pipeline channels
    /// </summary>
    public interface IPipelineChannelOutgoing: IPipelineChannel { }
    /// <summary>
    /// This interface is used by outgoing pipeline channels
    /// </summary>
    /// <typeparam name="P">The pipeline type</typeparam>
    public interface IPipelineChannelOutgoing<out P>: IPipelineChannelOutgoing,IPipelineChannel<P>
        where P : IPipeline
    {
    }
    /// <summary>
    /// This interface is used by bi-directional broadcast pipeline channels.
    /// </summary>
    public interface IPipelineChannelBroadcast: IPipelineChannelIncoming, IPipelineChannelOutgoing
    {
        /// <summary>
        /// This is the channel definition.
        /// </summary>
        Channel ChannelListener { get; }
        /// <summary>
        /// This is the channel definition.
        /// </summary>
        Channel ChannelSender { get; }
    }
    /// <summary>
    /// This interface is used by bi-directional broadcast pipeline channels.
    /// </summary>
    /// <typeparam name="P">The pipeline type</typeparam>
    public interface IPipelineChannelBroadcast<out P>: IPipelineChannelBroadcast, IPipelineChannelIncoming<P>, IPipelineChannelOutgoing<P>
    where P : IPipeline
    {
    }
}
