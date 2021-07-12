using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    public partial class CommunicationContainer: IChannelService
    {
        #region Declarations
        private ConcurrentDictionary<string, Channel> mChannelsIncoming;
        private ConcurrentDictionary<string, Channel> mChannelsOutgoing;
        #endregion

        #region Channels
        /// <summary>
        /// This is a list of the incoming and outgoing channels.
        /// </summary>
        public IEnumerable<Channel> Channels
        {
            get
            {
                return mChannelsIncoming.Values.Union(mChannelsOutgoing.Values);
            }
        }
        #endregion

        #region Add(Channel item)
        /// <summary>
        /// This method adds a channel to the collection
        /// </summary>
        /// <param name="item">The channel to add.</param>
        public virtual bool Add(Channel item)
        {
            if (item == null)
                throw new ArgumentNullException("item", "The channel cannot be null.");

            if (!item.BoundaryLoggingActive.HasValue)
                item.BoundaryLoggingActive = mPolicy.BoundaryLoggingActiveDefault;

            if (item.IsIncoming())
            {
                if (mChannelsIncoming.ContainsKey(item.Id))
                    throw new DuplicateChannelException(item.Id, item.Direction);

                mChannelsIncoming.TryAdd(item.Id, item);
            }

            if (item.IsOutgoing())
            {
                if (mChannelsOutgoing.ContainsKey(item.Id))
                    throw new DuplicateChannelException(item.Id, item.Direction);

                mChannelsOutgoing.TryAdd(item.Id, item);
            }

            return true;
        }
        #endregion
        #region Remove(Channel item)
        /// <summary>
        /// This method removes a channel from the collection.
        /// </summary>
        /// <param name="item">The channel item.</param>
        /// <returns>True if the channel is removed.</returns>
        public virtual bool Remove(Channel item)
        {
            Channel channel = null;
            bool success = false;

            if (item.IsIncoming())
                success |= mChannelsIncoming.TryRemove(item.Id, out channel);

            if (item.IsOutgoing())
                success |= mChannelsOutgoing.TryRemove(item.Id, out channel);

            return success;
        }
        #endregion
        #region Exists(string channelId, ChannelDirection direction)
        /// <summary>
        /// This method checks whether a channel has been added.
        /// </summary>
        /// <param name="channelId">The channel Id.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public bool Exists(string channelId, ChannelDirection direction)
        {
            if (string.IsNullOrWhiteSpace(channelId))
                return false;

            channelId = channelId.ToLowerInvariant();

            bool success = false;

            if (direction.IsIncoming())
                success |= mChannelsIncoming.ContainsKey(channelId);

            if (direction.IsOutgoing())
                success |= mChannelsOutgoing.ContainsKey(channelId);

            return success;
        }
        #endregion
        #region TryGet(string channelId, ChannelDirection direction, out Channel channel)
        /// <summary>
        /// This method attempts to retrieve and existing channel.
        /// </summary>
        /// <param name="channelId">The channel Id.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="channel">Returns the channel object.</param>
        /// <returns>Returns true if the channel is resolved.</returns>
        public bool TryGet(string channelId, ChannelDirection direction, out Channel channel)
        {
            channel = null;

            if (string.IsNullOrWhiteSpace(channelId))
                return false;

            channelId = channelId.ToLowerInvariant();

            if (mPolicy.AutoCreateChannels)
            {
                if (direction.IsIncoming())
                {
                    channel = mChannelsIncoming.GetOrAdd(channelId, (id) => new Channel(id, ChannelDirection.Incoming, isAutocreated: true));
                    return true;
                }

                if (direction.IsOutgoing())
                {
                    channel = mChannelsOutgoing.GetOrAdd(channelId, (id) => new Channel(id, ChannelDirection.Outgoing, isAutocreated: true));
                    return true;
                }
            }
            else
            {
                if (direction.IsIncoming()
                    & mChannelsIncoming.TryGetValue(channelId, out channel))
                    return true;

                if (direction.IsOutgoing()
                    & mChannelsOutgoing.TryGetValue(channelId, out channel))
                    return true;
            }

            return false;
        }
        #endregion

        #region PayloadIncomingRedirectCheck(TransmissionPayload payload)
        /// <summary>
        /// This method validates any rewrite rules for the incoming payload.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        protected virtual void PayloadIncomingRedirectCheck(TransmissionPayload payload)
        {
            var channelId = payload.Message.ChannelId;
            //Rewrite rule validate, and rewrite for incoming message.
            Channel channel;
            if (TryGet(channelId, ChannelDirection.Incoming, out channel))
            {
                if (channel.CouldRedirect)
                {
                    channel.Redirect(payload);
                    payload.TraceWrite("Redirected", "CommunicationContainer/PayloadIncomingRedirectCheck");
                }
            }
        }
        #endregion
        #region PayloadOutgoingRedirectChecks(TransmissionPayload payload)
        /// <summary>
        /// This method checks for any redirect rules for the outgoing payload.
        /// </summary>
        /// <param name="payload">The outgoing payload.</param>
        /// <returns>The outgoing channel.</returns>
        protected virtual Channel PayloadOutgoingRedirectChecks(TransmissionPayload payload)
        {
            //Rewrite rule validate, and rewrite for outgoing message.
            var channelId = payload.Message.ChannelId;
            Channel channel;
            if (TryGet(channelId, ChannelDirection.Outgoing, out channel))
            {
                if (channel.CouldRedirect)
                {
                    channel.Redirect(payload);
                    payload.TraceWrite("Redirected", "CommunicationContainer/PayloadOutgoingRedirectChecks");

                    //Get the new outgoing channel.
                    TryGet(payload.Message.ChannelId, ChannelDirection.Outgoing, out channel);
                }
            }

            return channel;
        } 
        #endregion
    }
}
