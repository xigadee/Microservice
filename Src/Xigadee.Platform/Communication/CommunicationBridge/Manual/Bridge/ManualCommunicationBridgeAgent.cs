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

        #region AddListener(ManualChannelListener listener)
        /// <summary>
        /// This method adds a listener to the bridge.
        /// </summary>
        /// <returns>The listener.</returns>
        protected IListener AddListener(ManualChannelListener listener)
        {
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
            mSenders.Add(sender);

            return sender;
        } 
        #endregion

        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
            try
            {
                OnReceiveInvoke(sender, e);

                if (mListeners.Count == 0)
                    return;

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
            int position = (int)(count % mListeners.Count);
            Sender_Transmit(position, e);
        }

        private void Sender_TransmitBroadcast(TransmissionPayload e, long count)
        {
            //Enumerable.Range(0,mListeners.Count).AsParallel().ForEach((c) => Sender_Transmit(c, e));
            for (int c = 0; c < mListeners.Count; c++)
                Sender_Transmit(c, e);
        }

        private void Sender_Transmit(int pos, TransmissionPayload e)
        {
            var listener = mListeners[pos];

            var payload = PayloadCopy(e);

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

            return new TransmissionPayload(clone);
        }
    }
}
