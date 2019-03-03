using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This bridge connects the listeners and senders together.
    /// </summary>
    public class ManualFabricBridge : CommunicationFabricBridgeBase, IManualCommunicationFabricBridge
    {
        protected List<ManualCommunicationAgent> mListeners = new List<ManualCommunicationAgent>();
        protected List<ManualCommunicationAgent> mSenders = new List<ManualCommunicationAgent>();
        private long mSendCount = 0;
        private long mSuccess = 0;
        private long mFailure = 0;
        ManualCommunicationAgent[] mActiveSenders = null;
        ManualCommunicationAgent[] mActiveListeners = null;

        ConcurrentDictionary<Guid, TransmissionPayloadHolder> mPayloadsActive = new ConcurrentDictionary<Guid, TransmissionPayloadHolder>();
        ConcurrentDictionary<Guid, TransmissionPayload> mPayloadsHistory = null;

        public ManualFabricBridge(ManualFabric fabric, ManualCommunicationFabricMode mode,
            bool payloadHistoryEnabled = true, int? retryAttempts = null):base(mode)
        {
            PayloadHistoryEnabled = payloadHistoryEnabled;
            RetryAttempts = retryAttempts;

            if (payloadHistoryEnabled)
                mPayloadsHistory = new ConcurrentDictionary<Guid, TransmissionPayload>();
        }

        /// <summary>
        /// Gets the retry attempts. Null if not specified.
        /// </summary>
        public int? RetryAttempts { get; }

        public override IListener GetListener()
        {
            var agent = new ManualCommunicationAgent(this, CommunicationAgentCapabilities.Listener);
            mListeners.Add(agent);
            agent.StatusChanged += Listener_StatusChanged;
            return agent;
        }

        private void Listener_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var newListeners = mListeners.Where((l) => l.Status == ServiceStatus.Running).ToArray();
            Interlocked.Exchange(ref mActiveListeners, newListeners);
        }

        public override ISender GetSender()
        {
            var agent = new ManualCommunicationAgent(this, CommunicationAgentCapabilities.Sender);
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

        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
            var key = e.ToConveyorKey();

            try
            {
                if (mActiveListeners.Length == 0)
                {
                    e.SignalSuccess();
                    return;
                }

                OnReceiveInvoke(sender, e);

                long count = Interlocked.Increment(ref mSendCount);

                switch (Mode)
                {
                    case ManualCommunicationFabricMode.Queue:
                        Sender_TransmitRoundRobin(e, count);
                        break;
                    case ManualCommunicationFabricMode.Broadcast:
                        Sender_TransmitBroadcast(e, count);
                        break;
                }
            }
            catch (Exception ex)
            {
                OnExceptionInvoke(sender, e, ex);
            }
        }

        private void Sender_TransmitRoundRobin(TransmissionPayload e, long count)
        {
            var listeners = mActiveListeners;

            int position = (int)(count % listeners.Length);
            Sender_Transmit(listeners[position], e);
        }

        private void Sender_TransmitBroadcast(TransmissionPayload e, long count)
        {
            var listeners = mActiveListeners;
            //Send as parallel requests to all the subscribers.
            Enumerable.Range(0, listeners.Length).AsParallel().ForEach((c) => Sender_Transmit(listeners[c], e));
        }

        private void Sender_Transmit(ManualCommunicationAgent listener, TransmissionPayload incoming)
        {
            var payload = incoming.Clone(SignalCompletion, true);

            payload.TraceWrite("Cloned", "ManualCommunicationBridgeAgent/PayloadCopy");

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


        private void SignalCompletion(bool success, Guid id)
        {
            if (success)
                Interlocked.Increment(ref mSuccess);
            else
                Interlocked.Increment(ref mFailure);

            TransmissionPayloadHolder holder;
            if (mPayloadsActive.TryRemove(id, out holder))
                mPayloadsHistory?.AddOrUpdate(id, holder.Payload, (i, p) => p);

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
