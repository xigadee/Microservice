#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    public partial class CommunicationContainer: IChannelService
    {
        #region Declarations
        private Dictionary<string, Channel> mContainerIncoming;
        private Dictionary<string, Channel> mContainerOutgoing;
        #endregion

        #region Wrapper...
        /// <summary>
        /// This method is used to ensure either of both collections are invoked for channel based methods.
        /// </summary>
        /// <param name="direction">The channel direction.</param>
        /// <param name="incoming">The incoming function.</param>
        /// <param name="outgoing">The outgoing funtion.</param>
        /// <returns></returns>
        private bool? Wrapper(ChannelDirection direction, Func<bool?, bool?> incoming, Func<bool?, bool?> outgoing)
        {
            bool? result = null;

            if ((direction & ChannelDirection.Incoming) > 0)
                result = incoming?.Invoke(result);

            if ((direction & ChannelDirection.Outgoing) > 0)
                result = outgoing?.Invoke(result);

            return result;
        } 
        #endregion

        #region Channels
        /// <summary>
        /// This is a list of the incoming and outgoing channels.
        /// </summary>
        public IEnumerable<Channel> Channels
        {
            get
            {
                return mContainerIncoming.Values.Union(mContainerOutgoing.Values).Distinct();
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

            return Wrapper(item.Direction,
                (r) =>
                {
                    if (mContainerIncoming.ContainsKey(item.Id))
                        throw new DuplicateChannelException(item.Id, item.Direction);

                    mContainerIncoming.Add(item.Id, item);
                    return true;
                },
                (r) =>
                {
                    if (mContainerOutgoing.ContainsKey(item.Id))
                        throw new DuplicateChannelException(item.Id, item.Direction);

                    mContainerOutgoing.Add(item.Id, item);
                    return true;
                }) ?? false;
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
            var success = Wrapper(item.Direction,
                 (r) => mContainerIncoming.Remove(item.Id)?true:r
                ,(r) => mContainerOutgoing.Remove(item.Id)?true:r
                );

            return success ?? false;
        }
        #endregion
        #region Exists(string channelId, ChannelDirection direction)
        /// <summary>
        /// This method checks whether a channel has been adeed.
        /// </summary>
        /// <param name="channelId">The channel Id.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        public bool Exists(string channelId, ChannelDirection direction)
        {
            var success = Wrapper(direction,
              (r) => mContainerIncoming.ContainsKey(channelId) ? true : default(bool?)
            , (r) => mContainerOutgoing.ContainsKey(channelId) ? true : r
            );

            return success ?? false;
        }
        #endregion
        #region TryGet(string channelId, ChannelDirection direction, out Channel channel)
        /// <summary>
        /// This method attempts to retrieve and existing channel.
        /// </summary>
        /// <param name="channelId">The channel Id.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="channel">Returns the channel object.</param>
        /// <returns></returns>
        public bool TryGet(string channelId, ChannelDirection direction, out Channel channel)
        {
            channel = null;

            if ((direction & ChannelDirection.Incoming) > 0
                & mContainerIncoming.TryGetValue(channelId, out channel))
                return true;

            if ((direction & ChannelDirection.Outgoing) > 0
                & mContainerOutgoing.TryGetValue(channelId, out channel))
                return true;

            if (mPolicy.AutoCreateChannels)
            {
                channel = new Channel(channelId, direction, isAutocreated:true);
                Add(channel);
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
