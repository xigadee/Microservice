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
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// The communication bridge is used to abstract the work of a communication bus.
    /// This can be used for integration testing, or for actual production systems.
    /// You can switch in different technologies when needed, with the default settings based on a manual bridge.
    /// </summary>
    public class CommunicationBridge
    {
        List<IListener> mListeners = new List<IListener>();
        List<ISender> mSenders = new List<ISender>();
        long mSendCount = 0;
        JsonContractSerializer mSerializer = new JsonContractSerializer();
        CommunicationBridgeAgent mAgent;

        /// <summary>
        /// This is the default constructor that specifies the broadcast mode.
        /// </summary>
        /// <param name="mode">This property specifies how the bridge communicates to the senders from the listeners.</param>
        /// <param name="agent">This is the communication agent. When not set this defaults to the manual agent.</param>
        public CommunicationBridge(CommunicationBridgeMode mode, CommunicationBridgeAgent agent = null)
        {
            mAgent = agent ?? new ManualCommunicationBridgeAgent();
            mAgent.SetMode(mode);
            Mode = mode;
        }

        /// <summary>
        /// This is the communication mode that the bridge is working under.
        /// </summary>
        public CommunicationBridgeMode Mode { get; }

        /// <summary>
        /// This method returns a new listener.
        /// </summary>
        /// <returns>The listener.</returns>
        public IListener GetListener()
        {
            var listener = new ManualChannelListener();

            return AddListener(listener);
        }


        /// <summary>
        /// This method returns a new sender.
        /// </summary>
        /// <returns>The sender.</returns>
        public ISender GetSender()
        {
            var sender = new ManualChannelSender();

            return AddSender(sender);
        }

        /// <summary>
        /// This method adds a listener to the bridge.
        /// </summary>
        /// <returns>The listener.</returns>
        protected IListener AddListener(ManualChannelListener listener)
        {
            mListeners.Add(listener);

            return listener;
        }


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


        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
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

        private void Sender_TransmitRoundRobin(TransmissionPayload e, long count)
        {
            int position = (int)(count % mListeners.Count);

            ((ManualChannelListener)mListeners[position]).Inject(PayloadCopy(e));
        }

        private void Sender_TransmitBroadcast(TransmissionPayload e, long count)
        {
            
            for(int c = 0; c< mListeners.Count; c++)
                ((ManualChannelListener)mListeners[c]).Inject(PayloadCopy(e));
        }

        /// <summary>
        /// This method seperates the payloads so that they are different objects.
        /// </summary>
        /// <param name="inPayload">The incoming payload.</param>
        /// <returns>Returns a new payload.</returns>
        private TransmissionPayload PayloadCopy(TransmissionPayload inPayload)
        {
            //First clone the service message.
            byte[] data = mSerializer.Serialize(inPayload.Message);

            ServiceMessage clone = mSerializer.Deserialize<ServiceMessage>(data);

            return new TransmissionPayload(clone);
        }
    }
}
