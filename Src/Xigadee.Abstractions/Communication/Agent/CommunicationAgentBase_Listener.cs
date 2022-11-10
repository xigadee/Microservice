﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract partial class CommunicationAgentBase<S>:IListener
    {
        private long mClientsStarted = 0;
        private long mClientsStopped = 0;

        /// <summary>
        /// This is the client collection based on the specific priority.
        /// </summary>
        protected ConcurrentDictionary<int, IClientHolderV2> mListenerClients = new ConcurrentDictionary<int, IClientHolderV2>();

        /// <summary>
        /// This is the default priority. 1 if present
        /// </summary>
        protected int? mDefaultPriority;
        /// <summary>
        /// This method is used to name the client based on the priority.
        /// </summary>
        protected Func<string, int, string> mPriorityClientNamer = (s, i) => string.Format("{0}{1}", s, i == 1 ? "" : i.ToString());

        /// <summary>
        /// This is the set of partitions for the listener. A partition is an integer that identifies priority channel for communication. Higher is better so 2 will go before 1 (default).
        /// </summary>
        public List<ListenerPartitionConfig> ListenerPriorityPartitions { get; set; }
        /// <summary>
        /// This is the external set of clients.
        /// </summary>
        public virtual IEnumerable<IClientHolder> ListenerClients => mListenerClients.Values;

        /// <summary>
        /// This is the set of resource profiles for the listeners.
        /// </summary>
        public List<ResourceProfile> ListenerResourceProfiles { get; set; }

        #region ListenerMappingChannelId
        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        public string ListenerMappingChannelId { get; set; } = null;
        #endregion
        #region ListenerSettingsValidate()
        /// <summary>
        /// This method is called at start-up and can be used to validate any listener specific settings.
        /// </summary>
        protected virtual void ListenerSettingsValidate()
        {
        } 
        #endregion

        #region ListenerPoll
        /// <summary>
        /// This boolean property determines whether the listener supports polling.
        /// </summary>
        public bool ListenerPollSupported { get; protected set; }
        /// <summary>
        /// This boolean property determines whether the listener requires a poll
        /// </summary>
        public bool ListenerPollRequired { get; protected set; }
        /// <summary>
        /// This is the async poll.
        /// </summary>
        public virtual Task ListenerPoll()
        {
            throw new NotImplementedException($"{GetType().Name}/{nameof(ListenerPoll)} is not implemented.");
        }
        #endregion

        #region ListenerCommandsActiveChange(List<MessageFilterWrapper> supported)
        /// <summary>
        /// This method is called with the list of currently supported message type supported by the Microservice.
        /// This can be used to filter the incoming messages where this is supported by the transport mechanism.
        /// The client should decide whether it should be active based on this list.
        /// </summary>
        public virtual void ListenerCommandsActiveChange(List<MessageFilterWrapper> supported)
        {
            var newList = supported.Where((m) => string.Equals(m.Header.ChannelId, (ListenerMappingChannelId ?? ChannelId), StringComparison.InvariantCultureIgnoreCase)).ToList();
            var oldList = mSupportedMessageTypes;

            ListenerClientsValidate(oldList, newList);
        }
        #endregion
        #region ListenerClientsValidate(List<MessageFilterWrapper> oldList, List<MessageFilterWrapper> newList)
        /// <summary>
        /// This method is used to revalidate the clients when a message type is enabled or disabled, and stop or start the appropriate clients.
        /// </summary>
        protected virtual void ListenerClientsValidate(List<MessageFilterWrapper> oldList, List<MessageFilterWrapper> newList)
        {
            if (Status != ServiceStatus.Running)
                return;

            //We only want to process a single thread at this point.
            lock (syncObject)
            {
                mSupportedMessageTypes = newList;

                if ((newList?.Count??0) == 0)
                {
                    ListenerClientsStop();
                    return;
                }

                if (((oldList?.Count??0) == 0) && newList.Count > 0)
                {
                    ListenerClientsStart();
                    return;
                }

                var deltaNew = newList.Except(oldList).ToList();
                var deltaOld = oldList.Except(newList).ToList();
                //OK, there is a small change. Let the clients know there is additional messages to filter on
                //and let them work out how to do it themselves.
                if (deltaNew.Count > 0 || deltaOld.Count > 0)
                    mListenerClients.ForEach((c) => ListenerClientValidate(c.Value, newList));
            }
        }
        #endregion

        #region ListenerClientsStart()
        /// <summary>
        /// This method starts a client for all the configured partitions.
        /// </summary>
        protected virtual void ListenerClientsStart() => ListenerPriorityPartitions?.ForEach((p) => ListenerClientStart(p)); 
        #endregion
        #region ListenerClientStart(ListenerPartitionConfig p)
        /// <summary>
        /// This method starts the listeners for the agent.
        /// </summary>
        /// <param name="p">The listener partition.</param>
        protected virtual void ListenerClientStart(ListenerPartitionConfig p)
        {
            try
            {
                var client = ListenerClientCreate(p);

                client.Priority = p.Priority;
                client.ChannelId = ChannelId;
                client.MappingChannelId = ListenerMappingChannelId;
                client.Name = $"Listener|{ChannelId}|{p.Priority}";

                mListenerClients.AddOrUpdate(p.Priority, client, (i, ct) => client);

                ServiceStart(client);
                Interlocked.Increment(ref mClientsStarted);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"{ProtocolId} Client Start error for partition {p.Priority}", ex);
                throw;
            }

        }
        #endregion
        #region ListenerClientsStop()
        /// <summary>
        /// This method stops all the current listerners
        /// </summary>
        protected virtual void ListenerClientsStop()
        {
            mListenerClients?.ForEach((c) =>
                {
                    try
                    {
                        ListenerClientStop(c.Value);
                        Interlocked.Increment(ref mClientsStopped);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                });
        }
        #endregion

        /// <summary>
        /// This method creates a listener.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected abstract IClientHolderV2 ListenerClientCreate(ListenerPartitionConfig p);

        /// <summary>
        /// This method stops the specific listener.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void ListenerClientStop(IClientHolderV2 client) => client.Stop();

        /// <summary>
        /// This method triggers a revalidation of the particular client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="newList">The new list of message filter wrappers.</param>
        protected abstract void ListenerClientValidate(IClientHolderV2 client, List<MessageFilterWrapper> newList);

        #region ListenersTearUp()
        /// <summary>
        /// This method can be used to set up the listener environment.
        /// </summary>
        public virtual void ListenersTearUp()
        {

        }
        #endregion
        #region ListenersTearDown()
        /// <summary>
        /// This method can be used to clean up the listener environment.
        /// </summary>
        public virtual void ListenersTearDown()
        {

        } 
        #endregion
    }
}