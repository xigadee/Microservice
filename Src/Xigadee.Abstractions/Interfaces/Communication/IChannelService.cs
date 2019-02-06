using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by the Communication service and allows
    /// components to query the system channels.
    /// </summary>
    public interface IChannelService
    {
        /// <summary>
        /// Gets the channels.
        /// </summary>
        IEnumerable<Channel> Channels { get; }
        /// <summary>
        /// Adds the specific channel.
        /// </summary>
        /// <param name="item">The channel to add.</param>
        /// <returns>Returns true if the channel is added successfully.</returns>
        bool Add(Channel item);
        /// <summary>
        /// Removes the specific channel.
        /// </summary>
        /// <param name="item">The channel to remove.</param>
        /// <returns>Returns true if the channel is removed successfully.</returns>
        bool Remove(Channel item);
        /// <summary>
        /// Returns true if the channel exists in the specific direction.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>Returns true if the channel and direction exist.</returns>
        bool Exists(string channelId, ChannelDirection direction);
        /// <summary>
        /// Attempts to retrieve the channel object.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="channel">The channel object.</param>
        /// <returns>Returns true if the channel and direction exist, else false.</returns>
        bool TryGet(string channelId, ChannelDirection direction, out Channel channel);

    }

}
