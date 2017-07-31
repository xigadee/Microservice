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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This agent uses the manual channel agent for communication.
    /// </summary>
    public class ManualCommunicationBridgeAgent:CommunicationBridgeAgent
    {
        #region Declarations
        List<ManualChannelListener> mListeners = new List<ManualChannelListener>();
        List<ManualChannelSender> mSenders = new List<ManualChannelSender>();
        long mSendCount = 0;
        JsonContractSerializer mSerializer = new JsonContractSerializer();

        ConcurrentDictionary<Guid, TransmissionPayload> mSenderPayloads = new ConcurrentDictionary<Guid, TransmissionPayload>();

        ManualChannelSender[] mActiveSenders = null;
        ManualChannelListener[] mActiveListeners = null;

        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="mode">The desirec communication mode.</param>
        public ManualCommunicationBridgeAgent(CommunicationBridgeMode mode) : base(mode)
        {
        }
        #endregion

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

        #region AddListener(ManualChannelListener listener)
        /// <summary>
        /// This method adds a listener to the bridge.
        /// </summary>
        /// <returns>The listener.</returns>
        protected IListener AddListener(ManualChannelListener listener)
        {
            listener.StatusChanged += Listener_StatusChanged;

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
            mSenders.Add(sender);

            return sender;
        }
        #endregion

        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
            try
            {
                if (mActiveListeners.Length == 0)
                    return;

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

            Enumerable.Range(0, listeners.Length).AsParallel().ForEach((c) => Sender_Transmit(listeners[c], e));
            //foreach (var listener in listeners)
            //    Sender_Transmit(listener, e);
        }

        private void Sender_Transmit(ManualChannelListener listener, TransmissionPayload incoming)
        {
            var payload = PayloadCopy(incoming);

            OnTransmitInvoke(listener, payload);

            listener.Inject(payload);
        }

        /// <summary>
        /// This method seperates the payloads so that they are different objects.
        /// </summary>
        /// <param name="inPayload">The incoming payload.</param>
        /// <returns>Returns a new cloned payload.</returns>
        private TransmissionPayload PayloadCopy(TransmissionPayload inPayload)
        {
            //First clone the service message.
            byte[] data = mSerializer.Serialize(inPayload.Message);

            ServiceMessage clone = mSerializer.Deserialize<ServiceMessage>(data);

            var cloned = new TransmissionPayload(clone, release: SignalCompletion);

            mSenderPayloads.AddOrUpdate(cloned.Id, cloned, (g,p)=>p);

            return cloned;
        }

        private void SignalCompletion(bool success, Guid id)
        {
            if (success)
                Interlocked.Increment(ref mSuccess);
            else
                Interlocked.Increment(ref mFailure);

            TransmissionPayload payload;
            mSenderPayloads.TryRemove(id, out payload);
        }

        private int mSuccess = 0;
        private int mFailure = 0;

        /// <summary>
        /// A boolean property indicating that all transmitted payloads have been successfully signalled.
        /// </summary>
        public override bool PayloadsAllSignalled { get { return mSenderPayloads.Count == 0; } }
    }
}
