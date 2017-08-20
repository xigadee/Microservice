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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This agent uses the manual channel agent for communication.
    /// </summary>
    public class ManualCommunicationBridgeAgent: CommunicationBridgeAgent
    {
        #region Declarations
        private long mSendCount = 0;
        private long mSuccess = 0;
        private long mFailure = 0;

        List<ManualChannelListener> mListeners = new List<ManualChannelListener>();
        List<ManualChannelSender> mSenders = new List<ManualChannelSender>();
        JsonContractSerializer mSerializer = new JsonContractSerializer();

        ConcurrentDictionary<Guid, TransmissionPayloadHolder> mPayloadsActive = new ConcurrentDictionary<Guid, TransmissionPayloadHolder>();
        ConcurrentDictionary<Guid, TransmissionPayload> mPayloadsHistory = null;

        ManualChannelSender[] mActiveSenders = null;
        ManualChannelListener[] mActiveListeners = null;

        private readonly int? mRetryAttempts;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="mode">The desired communication mode.</param>
        /// <param name="payloadHistoryEnabled">This property specifies whether the message history should be maintained.</param>
        /// <param name="retryAttempts">This is the number of retry delivery attempts that should be attempted. Leave this null if not required.</param>
        /// <param name="fabric">This is the connection fabric. If null, then a new fabric will be created.</param>
        public ManualCommunicationBridgeAgent(ManualFabricBridge fabric, CommunicationBridgeMode mode            
            , bool payloadHistoryEnabled = false
            , int? retryAttempts = null
            ) : base(mode)
        {
            Fabric = fabric?? throw new ArgumentNullException("fabric");
            mRetryAttempts = retryAttempts;

            PayloadHistoryEnabled = payloadHistoryEnabled;
            if (payloadHistoryEnabled)
            {
                mPayloadsHistory = new ConcurrentDictionary<Guid, TransmissionPayload>();
            }

        }
        #endregion        
        /// <summary>
        /// Gets the connection fabric.
        /// </summary>
        public ManualFabricBridge Fabric { get; }

        #region GetListener()
        /// <summary>
        /// This method returns a new listener.
        /// </summary>
        /// <returns>The listener.</returns>
        public override IListener GetListener()
        {
            var listener = new ManualChannelListener();
            return AddListener(listener);
        }
        #endregion
        #region GetSender()
        /// <summary>
        /// This method returns a new sender.
        /// </summary>
        /// <returns>The sender.</returns>
        public override ISender GetSender()
        {
            var sender = new ManualChannelSender();
            return AddSender(sender);
        }
        #endregion

        #region AddListener(ManualChannelListener listener)
        /// <summary>
        /// This method adds a listener to the bridge.
        /// </summary>
        /// <returns>The listener.</returns>
        protected IListener AddListener(ManualChannelListener listener)
        {
            listener.StatusChanged += Listener_StatusChanged;
            listener.Fabric = Fabric;

            mListeners.Add(listener);

            return listener;
        }
        #endregion
        #region AddSender(ManualChannelSender sender)
        /// <summary>
        /// This method adds a sender to the bridge.
        /// </summary>
        /// <returns>The sender.</returns>
        protected ISender AddSender(ManualChannelSender sender)
        {
            sender.OnProcess += Sender_OnProcess;
            sender.StatusChanged += Sender_StatusChanged;
            sender.Fabric = Fabric;

            mSenders.Add(sender);

            return sender;
        }
        #endregion

        private void Listener_StatusChanged(object component, StatusChangedEventArgs e)
        {
            var newListeners = mListeners.Where((l) => l.Status == ServiceStatus.Running).ToArray();
            Interlocked.Exchange(ref mActiveListeners, newListeners);
        }

        private void Sender_StatusChanged(object component, StatusChangedEventArgs e)
        {
            var newSenders = mSenders.Where((l) => l.Status == ServiceStatus.Running).ToArray();
            Interlocked.Exchange(ref mActiveSenders, newSenders);
        }

        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
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
                    case CommunicationBridgeMode.RoundRobin:
                        Sender_TransmitRoundRobin(e, count);
                        break;
                    case CommunicationBridgeMode.Broadcast:
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

        private void Sender_Transmit(ManualChannelListener listener, TransmissionPayload incoming)
        {
            var payload = incoming.Clone(SignalCompletion, true);

            payload.TraceWrite("Cloned", "ManualCommunicationBridgeAgent/PayloadCopy");

            mPayloadsActive.AddOrUpdate(payload.Id, new TransmissionPayloadHolder(payload,listener), (g, p) => p);

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
            if (mPayloadsActive.TryRemove(id, out holder) && PayloadHistoryEnabled)
                mPayloadsHistory.AddOrUpdate(id, holder.Payload, (i,p) => p);

            if (!success && mRetryAttempts.HasValue)
            {
                //holder.Listener.
            }
        }

        /// <summary>
        /// A boolean property indicating that all transmitted payloads have been successfully signalled.
        /// </summary>
        public override bool PayloadsAllSignalled { get { return mPayloadsActive.Count == 0; } }
    }
}
