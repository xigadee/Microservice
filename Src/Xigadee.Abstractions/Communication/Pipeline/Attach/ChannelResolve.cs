using System;
namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This helper resolves a channel from the direction specified.
        /// </summary>
        /// <param name="cpipe">The pipeline.</param>
        /// <param name="direction"></param>
        /// <param name="throwIfChannelIsNull"></param>
        /// <returns>The resolved channel or null if not resolved and throwIfChannelIsNull is false. Otherwise an exception will be raised.</returns>
        public static Channel ToChannel(this IPipelineChannel cpipe, ChannelDirection direction, bool throwIfChannelIsNull = true)
        {
            Channel channel = null;

            if (cpipe is IPipelineChannelBroadcast)
                switch (direction)
                {
                    case ChannelDirection.Incoming:
                        channel = ((IPipelineChannelBroadcast)cpipe).ChannelListener;
                        break;
                    case ChannelDirection.Outgoing:
                        channel = ((IPipelineChannelBroadcast)cpipe).ChannelSender;
                        break;
                    default:
                        throw new NotSupportedException($"ChannelDirection {direction} not supported in {nameof(CorePipelineExtensions)}/{nameof(ToChannel)}");
                }
            else
                channel = cpipe.Channel;

            if (channel == null && throwIfChannelIsNull)
                throw new ArgumentNullException($"The pipe channel is null -> {direction}");

            return channel;
        }
    }
}
