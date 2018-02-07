using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class CommunicationAgentBase<S>: ServiceBase<S>, IListener, ISender
        where S : StatusBase, new()
    {
        #region Declarations
        private object syncObject = new object();
        /// <summary>
        /// This is the supported list of message types for the client.
        /// </summary>
        protected List<MessageFilterWrapper> mSupportedMessageTypes = new List<MessageFilterWrapper>();
        #endregion

        public CommunicationAgentBase()
        {

        }

        public CommunicationAgentCapabilities Capabilities { get; protected set; }

        protected override void StartInternal()
        {
            throw new NotImplementedException();
        }

        protected override void StopInternal()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<ClientHolder> ListenerClients { get; }

        public virtual IEnumerable<ClientHolder> SenderClients { get; }

        /// <summary>
        /// This is the channel id that incoming messages will be mapped to.
        /// </summary>
        public string ListenerMappingChannelId { get; set; }

        public List<ResourceProfile> ResourceProfiles { get; set; }

        public List<ListenerPartitionConfig> PriorityPartitions { get; set; }
        List<SenderPartitionConfig> ISender.PriorityPartitions { get; set; }

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

        #region SenderTransmit(TransmissionPayload payload)
        /// <summary>
        /// This method resolves the client and processes the message.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        public virtual async Task SenderTransmit(TransmissionPayload payload)
        {
            int? start = null;
            ClientHolder client = null;
            try
            {
                client = SenderClientResolve(payload.Message.ChannelPriority);

                start = client.StatisticsInternal.ActiveIncrement();

                await client.Transmit(payload);

                payload.TraceWrite($"Sent: {client.Name}", "MessagingSenderBase/ProcessMessage");
            }
            catch (Exception ex)
            {
                LogExceptionLocation($"{nameof(SenderTransmit)} (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                payload.TraceWrite($"Exception: {ex.Message}", "MessagingSenderBase/ProcessMessage");
                if (client != null)
                    client.StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                if (client != null && start.HasValue)
                    client.StatisticsInternal.ActiveDecrement(start.Value);
            }
        } 
        #endregion

        protected abstract ClientHolder SenderClientResolve(int priority);

        #region SupportsChannel(string channel)
        /// <summary>
        /// This method compares the channel and returns true if it is supported.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>Returns true if the channel is supported.</returns>
        public bool SupportsChannel(string channel)
        {
            return string.Equals(channel, ChannelId, StringComparison.InvariantCultureIgnoreCase);
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


        #region BoundaryLoggingActive
        /// <summary>
        /// This property specifies whether the boundary logger is active.
        /// </summary>
        public bool? BoundaryLoggingActive { get; set; }
        #endregion
        #region ChannelId
        /// <summary>
        /// This is the ChannelId for the messaging service
        /// </summary>
        public string ChannelId
        {
            get;
            set;
        }
        #endregion

        #region LogExceptionLocation(string method)
        /// <summary>
        /// This helper method provides a class name and method name for debugging exceptions. 
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>A combination string.</returns>
        protected void LogExceptionLocation(string method, Exception ex)
        {
            Collector?.LogException($"{GetType().Name}/{method}", ex);
        }
        #endregion
        /// <summary>
        /// This is the system wide service handlers.
        /// </summary>
        public IServiceHandlers ServiceHandlers { get; set; }
        /// <summary>
        /// The originator Id for the service.
        /// </summary>
        public MicroserviceId OriginatorId { get; set; }
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; set; }
        /// <summary>
        /// The shared service container.
        /// </summary>
        public ISharedService SharedServices { get;set; }
    }

    [Flags]
    public enum CommunicationAgentCapabilities
    {
        Listener = 1,
        Sender = 3,
        Bidirectional = 3
    }
}
