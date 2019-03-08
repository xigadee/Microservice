using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This bridge connects the listeners and senders together.
    /// </summary>
    public class ManualFabricBridge : CommunicationFabricBridgeBase, IManualCommunicationFabricBridge
    {
        #region Declarations
        protected List<ManualCommunicationAgent> mListeners = new List<ManualCommunicationAgent>();
        protected List<ManualCommunicationAgent> mSenders = new List<ManualCommunicationAgent>();

        private long mSendCount = 0;
        private long mSuccess = 0;
        private long mFailure = 0;

        ManualCommunicationAgent[] mActiveSenders = null;
        ManualCommunicationAgent[] mActiveListeners = null;

        ConcurrentDictionary<Guid, TransmissionPayloadHolder> mPayloadsActive = new ConcurrentDictionary<Guid, TransmissionPayloadHolder>();
        ConcurrentDictionary<Guid, TransmissionPayload> mPayloadsHistory = null;
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
                mPayloadsHistory = new ConcurrentDictionary<Guid, TransmissionPayload>();
        } 
        #endregion

        /// <summary>
        /// Gets the retry attempts. Null if not specified.
        /// </summary>
        public int? RetryAttempts { get; }

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

        private void Listener_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var newListeners = mListeners.Where((l) => l.Status == ServiceStatus.Running).ToArray();
            Interlocked.Exchange(ref mActiveListeners, newListeners);
        }

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
            var newSenders = mSenders.Where((l) => l.Status == ServiceStatus.Running).ToArray();
            Interlocked.Exchange(ref mActiveSenders, newSenders);
        }

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

                var listeners = mActiveListeners.Where((c) => c.ChannelId == e.Message.ChannelId).ToArray();

                if (listeners.Length == 0)
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
        /// <summary>
        /// Do a broadcast to all the listening clients.
        /// </summary>
        /// <param name="listeners">The active listeners</param>
        /// <param name="e">The payload.</param>
        /// <param name="count">The send count.</param>
        private void Sender_TransmitBroadcast(ManualCommunicationAgent[] listeners, TransmissionPayload e, long count)
        {
            //Send as parallel requests to all the subscribers.
            Enumerable.Range(0, listeners.Length).AsParallel().ForEach((c) => Sender_Transmit(listeners[c], e));
        }

        /// <summary>
        /// Clone the payload and transmit to the listener specified.
        /// </summary>
        /// <param name="listener">The manual listener.</param>
        /// <param name="incoming">The payload to transmit.</param>
        private void Sender_Transmit(ManualCommunicationAgent listener, TransmissionPayload incoming)
        {
            var payload = PayloadClone(incoming);

            payload.TraceWrite($"Transmit -> {listener.ChannelId} -> {incoming.Message.ChannelPriority}");

            mPayloadsActive.AddOrUpdate(payload.Id, new TransmissionPayloadHolder(payload, listener), (g, p) => p);

            OnTransmitInvoke(listener, payload);

            try
            {
                listener.Inject(payload);
            }
            catch (Exception ex)
            {
                listener.Collector?.LogException("Unhandled exception in the BridgeAgent", ex);
            }
        }


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
                mPayloadsHistory?.AddOrUpdate(id, holder.Payload, (i, p) => p);
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
