using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract partial class CommunicationAgentBase<S>
    {
        public virtual IEnumerable<ClientHolder> ListenerClients { get; }

        public List<ResourceProfile> ListenerResourceProfiles { get; set; }

        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        public string ListenerMappingChannelId { get; set; }

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
        /// <returns>
        /// Async.
        /// </returns>
        public virtual Task ListenerPoll()
        {
            throw new NotSupportedException($"{nameof(ListenerPoll)} is not supported.");
        }
        #endregion

        #region Update(List<MessageFilterWrapper> supported)
        /// <summary>
        /// This method is called with the list of currently supported message type supported by the Microservice.
        /// This can be used to filter the incoming messages where this is supported by the transport mechanism.
        /// The client should decide whether it should be active based on this list.
        /// </summary>
        public virtual void Update(List<MessageFilterWrapper> supported)
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

                if (newList == null || newList.Count == 0)
                {
                    ListenerClientsStop();
                    return;
                }

                if ((oldList == null || oldList.Count == 0) && newList.Count > 0)
                {
                    ListenerClientsStart();
                    return;
                }

                var deltaNew = newList.Except(oldList).ToList();
                var deltaOld = oldList.Except(newList).ToList();
                //OK, there is a small change. Let the clients know there is additional messages to filter on
                //and let them work out how to do it themselves.
                if (deltaNew.Count > 0 || deltaOld.Count > 0)
                    ListenerClients.ForEach((c) => ListenerClientValidate(c, newList));
            }
        }
        #endregion

        /// <summary>
        /// This method triggers a revalidates of the particular client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="newList">The new list of message filter wrappers.</param>
        protected abstract void ListenerClientValidate(ClientHolder client, List<MessageFilterWrapper> newList);

        protected abstract void ListenerClientsStart();

        protected abstract void ListenerClientsStop();
    }
}
