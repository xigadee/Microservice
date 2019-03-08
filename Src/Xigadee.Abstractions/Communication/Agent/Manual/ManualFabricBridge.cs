using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This bridge connects the listeners and senders together.
    /// </summary>
    [DebuggerDisplay("{Mode}:Agents[L={mListeners.Count}|S={mSenders.Count}] Payload[A={mPayloadsActive.Count}|H={mPayloadsHistory?.Count??0}] {Id}")]
    public class ManualFabricBridge : CommunicationFabricBridgeBase, IManualCommunicationFabricBridge
    {
        #region Declarations
        long mSendCount = 0;
        long mSuccess = 0;
        long mFailure = 0;

        List<ManualCommunicationAgent> mListeners = new List<ManualCommunicationAgent>();
        List<ManualCommunicationAgent> mSenders = new List<ManualCommunicationAgent>();

        Dictionary<string,ManualCommunicationAgent[]> mActiveSenders = null;
        Dictionary<string,ManualCommunicationAgent[]> mActiveListeners = null;

        ConcurrentDictionary<Guid, TransmissionPayloadHolder> mPayloadsActive = new ConcurrentDictionary<Guid, TransmissionPayloadHolder>();
        ConcurrentDictionary<Guid, TransmissionPayloadHolder> mPayloadsHistory = null;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="fabric"></param>
        /// <param name="mode">The distribution mode.</param>
        /// <param name="payloadHistoryEnabled">Specifies whether payload history is supported. The default is true.</param>
        /// <param name="retryAttempts">The number of retry attempts for the payload.</param>
        public ManualFabricBridge(ManualFabric fabric, ManualCommunicationFabricMode mode,
            bool payloadHistoryEnabled = true, int? retryAttempts = null) : base(mode)
        {
            PayloadHistoryEnabled = payloadHistoryEnabled;
            RetryAttempts = retryAttempts;

            if (payloadHistoryEnabled)
                mPayloadsHistory = new ConcurrentDictionary<Guid, TransmissionPayloadHolder>();
        }
        #endregion

        /// <summary>
        /// Gets the retry attempts. Null if not specified.
        /// </summary>
        public int? RetryAttempts { get; }

        #region Listener...
        /// <summary>
        /// This method gets a new listener.
        /// </summary>
        /// <returns>Returns the new listener.</returns>
        public override IListener GetListener()
        {
            var agent = new ManualCommunicationAgent(CommunicationAgentCapabilities.Listener);
            mListeners.Add(agent);
            agent.StatusChanged += Listener_StatusChanged;
            return agent;
        }

        private Dictionary<string, ManualCommunicationAgent[]> GetActive(List<ManualCommunicationAgent> agents)
        {
            var active = agents
                .Where((l) => l.Status == ServiceStatus.Running);

            var dict = active
                .Select((l) => l.ChannelId).Distinct()
                .ToDictionary((a) => a, (a) => active.Where((l) => l.ChannelId == a).ToArray());

            return dict;
        }

        private void Listener_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var dict = GetActive(mListeners);
            Interlocked.Exchange(ref mActiveListeners, dict);
        }
        #endregion

        #region Sender ...
        /// <summary>
        /// This method gets a new sender.
        /// </summary>
        /// <returns>The sender.</returns>
        public override ISender GetSender()
        {
            var agent = new ManualCommunicationAgent(CommunicationAgentCapabilities.Sender);
            mSenders.Add(agent);
            agent.StatusChanged += Sender_StatusChanged;
            agent.OnProcess += Sender_OnProcess;
            return agent;
        }

        private void Sender_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var dict = GetActive(mSenders);
            Interlocked.Exchange(ref mActiveSenders, dict);
        }
        #endregion

        #region Sender_OnProcess(object sender, TransmissionPayload e)
        /// <summary>
        /// This method distributes the incoming payload to the relevant senders based on the 
        /// distribution algorithm chosen.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The payload.</param>
        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
            try
            {
                OnReceiveInvoke(sender, e);
                ManualCommunicationAgent[] listeners;

                var channel = e.Message.ChannelId;

                if (!mActiveListeners.TryGetValue(channel, out listeners) || listeners.Length == 0)
                {
                    e.SignalSuccess();
                    return;
                }

                long count = Interlocked.Increment(ref mSendCount);

                switch (Mode)
                {
                    case ManualCommunicationFabricMode.Queue:
                        Sender_TransmitRoundRobin(listeners, e, count);
                        break;
                    case ManualCommunicationFabricMode.Broadcast:
                        Sender_TransmitBroadcast(listeners, e, count);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("{Mode} is not supported.");
                }
            }
            catch (Exception ex)
            {
                e.SignalFail();
                OnExceptionInvoke(sender, e, ex);
            }
        }
        #endregion

        #region Sender_TransmitRoundRobin...
        /// <summary>
        /// Do a round robin distribution to one of the listening clients.
        /// </summary>
        /// <param name="listeners">The active listeners</param>
        /// <param name="e">The payload.</param>
        /// <param name="count">The send count.</param>
        private void Sender_TransmitRoundRobin(ManualCommunicationAgent[] listeners, TransmissionPayload e, long count)
        {
            int position = (int)(count % listeners.Length);
            Sender_Transmit(listeners[position], e);
        }
        #endregion
        #region Sender_TransmitBroadcast...
        /// <summary>
        /// Do a broadcast to all the listening clients.
        /// </summary>
        /// <param name="listeners">The active listeners</param>
        /// <param name="e">The payload.</param>
        /// <param name="count">The send count.</param>
        private void Sender_TransmitBroadcast(ManualCommunicationAgent[] listeners, TransmissionPayload e, long count)
        {
            //Send as parallel requests to all the subscribers.
            listeners.ForEach((c) => Sender_Transmit(c, e));
        }
        #endregion

        #region Sender_Transmit ...
        /// <summary>
        /// Clone the payload and transmit to the listener specified.
        /// </summary>
        /// <param name="listener">The manual listener.</param>
        /// <param name="incoming">The payload to transmit.</param>
        private void Sender_Transmit(ManualCommunicationAgent listener, TransmissionPayload incoming)
        {
            try
            {
                var payload = PayloadClone(incoming);

                payload.TraceWrite($"Transmit -> {listener.ChannelId} -> {incoming.Message.ChannelPriority}");

                mPayloadsActive.AddOrUpdate(payload.Id, new TransmissionPayloadHolder(payload, incoming, listener), (g, p) => p);

                OnTransmitInvoke(listener, payload);

                listener.Inject(payload);
            }
            catch (Exception ex)
            {
                listener.Collector?.LogException("Unhandled exception in the BridgeAgent", ex);
            }
        } 
        #endregion


        private TransmissionPayload PayloadClone(TransmissionPayload incoming)
        {
            var payload = incoming.Clone(SignalCompletion, true);

            return payload;
        }


        private void SignalCompletion(bool success, Guid id)
        {
            if (success)
                Interlocked.Increment(ref mSuccess);
            else
                Interlocked.Increment(ref mFailure);

            TransmissionPayloadHolder holder;
            if (mPayloadsActive.TryRemove(id, out holder))
                mPayloadsHistory?.AddOrUpdate(id, holder, (i, p) => p);
            else
                throw new ArgumentOutOfRangeException();
            //if (!success && mRetryAttempts.HasValue)
            //{
            //    //holder.Listener.
            //}
        }

        #region PayloadsAllSignalled
        /// <summary>
        /// This method identifies whether there are active payloads in flight.
        /// </summary>
        public override bool PayloadsAllSignalled => mPayloadsActive.IsEmpty; 
        #endregion
    }
}
