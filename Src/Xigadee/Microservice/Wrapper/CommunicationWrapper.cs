﻿using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This wrapper holds the communication service.
    /// </summary>
    public class CommunicationWrapper: WrapperBase, IMicroserviceCommunication
    {
        /// <summary>
        /// This container holds the communication components.
        /// </summary>
        protected CommunicationContainer mCommunication;

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="communication">The communication container.</param>
        /// <param name="getStatus">The function to return the service status.</param>
        public CommunicationWrapper(CommunicationContainer communication, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mCommunication = communication;
        }

        //Communication
        #region RegisterListener(IListener listener)
        /// <summary>
        /// This method regsiters a listener.
        /// </summary>
        /// <typeparam name="C">The listener channelId that implements IListener</typeparam>
        public virtual IListener RegisterListener(IListener listener)
        {
            ValidateServiceNotStarted();
            mCommunication.ListenerAdd(listener);
            return listener;
        }
        #endregion
        #region RegisterSender(ISender sender)
        /// <summary>
        /// This method registers a sender that implements ISender.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        public virtual ISender RegisterSender(ISender sender)
        {
            ValidateServiceNotStarted();
            mCommunication.SenderAdd(sender);
            return sender;
        }
        #endregion
        //Channels
        #region RegisterChannel(Channel logger)
        /// <summary>
        /// This method can be used to manually register an Collector?.
        /// </summary>
        public virtual Channel RegisterChannel(Channel channel)
        {
            ValidateServiceNotStarted();
            mCommunication.Add(channel);
            return channel;
        }
        #endregion

        #region HasChannel(string channelId, ChannelDirection direction)
        /// <summary>
        /// This method returns true if the channel is registered for that specific direction.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="direction">The channel direction.</param>
        /// <returns>Returns true if the channel exists.</returns>
        public bool HasChannel(string channelId, ChannelDirection direction)
        {
            return mCommunication.Exists(channelId, direction);
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
        public bool TryGet(string channelId, ChannelDirection direction, out Channel channel) => mCommunication.TryGet(channelId, direction, out channel);
        #endregion

        #region Channels
        /// <summary>
        /// This is a list of jobs currently register in the service.
        /// </summary>
        public virtual IEnumerable<Channel> Channels
        {
            get
            {
                if (mCommunication.Channels == null)
                    yield break;
                else
                    foreach (var channel in mCommunication.Channels)
                        yield return channel;
            }
        }
        #endregion
    }
}
