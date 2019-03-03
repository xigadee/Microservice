using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This bridge connects the listeners and senders together.
    /// </summary>
    public class ManualFabricBridge : CommunicationFabricBridgeBase, IManualCommunicationFabricBridge
    {
        protected HashSet<ManualCommunicationAgent> mListeners = new HashSet<ManualCommunicationAgent>();
        protected HashSet<ManualCommunicationAgent> mSenders = new HashSet<ManualCommunicationAgent>();

        public ManualFabricBridge(ManualFabric fabric, ManualCommunicationFabricMode mode,
            bool payloadHistoryEnabled = true, int? retryAttempts = null):base(mode)
        {
            PayloadHistoryEnabled = payloadHistoryEnabled;
            RetryAttempts = retryAttempts;
        }

        /// <summary>
        /// Gets the retry attempts. Null if not specified.
        /// </summary>
        public int? RetryAttempts { get; }

        public override IListener GetListener()
        {
            var agent = new ManualCommunicationAgent(this);
            mListeners.Add(agent);
            agent.StatusChanged += Listener_StatusChanged;
            return agent;
        }

        private void Listener_StatusChanged(object sender, StatusChangedEventArgs e)
        {
        }

        public override ISender GetSender()
        {
            var agent = new ManualCommunicationAgent(this);
            mSenders.Add(agent);
            agent.StatusChanged += Sender_StatusChanged;
            agent.OnProcess += Sender_OnProcess;
            return agent;
        }

        private void Sender_StatusChanged(object sender, StatusChangedEventArgs e)
        {
        }

        private void Sender_OnProcess(object sender, TransmissionPayload e)
        {
        }
    }
}
