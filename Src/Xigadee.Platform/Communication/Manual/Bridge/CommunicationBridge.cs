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
    /// The communication bridge is used to simulate the work of a messaging bus.
    /// This can be used for integration testing, or for actual production systems.
    /// </summary>
    public class CommunicationBridge
    {
        List<ManualChannelListener> mListeners = new List<ManualChannelListener>();
        List<ManualChannelSender> mSenders = new List<ManualChannelSender>();
        long mSendCount = 0;
        JsonContractSerializer mSerializer = new JsonContractSerializer();

        /// <summary>
        /// This is the default constructor that specifies the broadcast mode.
        /// </summary>
        /// <param name="mode">This property specifies how the bridge communicates to the senders from the listeners.</param>
        public CommunicationBridge(CommunicationBridgeMode mode)
        {
            Mode = mode;
        }

        /// <summary>
        /// This is the communication mode that the bridge is working under.
        /// </summary>
        public CommunicationBridgeMode Mode { get; set;}

        /// <summary>
        /// This method adds a listener to the bridge.
        /// </summary>
        /// <returns>The listener.</returns>
        public IListener AddListener(ManualChannelListener listener)
        {
            mListeners.Add(listener);

            return listener;
        }
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
        /// This method adds a sender to the bridge.
        /// </summary>
        /// <returns>The sender.</returns>
        public ISender AddSender(ManualChannelSender sender)
        {
            sender.OnProcess += Sender_OnProcess;
            mSenders.Add(sender);

            return sender;
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

            mListeners[position].Inject(PayloadCopy(e));
        }

        private void Sender_TransmitBroadcast(TransmissionPayload e, long count)
        {
            
            for(int c = 0; c< mListeners.Count; c++)
                mListeners[c].Inject(PayloadCopy(e));
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
