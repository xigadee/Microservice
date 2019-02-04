using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface contains the external Microservice communication commands.
    /// </summary>
    public interface IMicroserviceCommunication
    {
        /// <summary>
        /// This is the list of channel registered for the Microservice.
        /// </summary>
        IEnumerable<Channel> Channels { get; }
        /// <summary>
        /// This method registers a channel with the Microservice.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>Returns the channel registered.</returns>
        Channel RegisterChannel(Channel channel);
        /// <summary>
        /// This method returns true if the channel is registered.
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <param name="direction">The channel direction.</param>
        /// <returns></returns>
        bool HasChannel(string channelId, ChannelDirection direction);
        /// <summary>
        /// This method registers a listener channel.
        /// </summary>
        /// <param name="listener">The listener</param>
        /// <returns>Returns the listener</returns>
        IListener RegisterListener(IListener listener);
        /// <summary>
        /// This method registers a sender channel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns>Returns the sender.</returns>
        ISender RegisterSender(ISender sender);
    }
}
