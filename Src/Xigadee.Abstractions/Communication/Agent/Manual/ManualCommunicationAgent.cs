using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ManualCommunicationAgent : CommunicationAgentBase
    {

        /// <summary>
        /// Occurs when a message is sent to the sender. This event is caught and is used to map to corresponding listeners.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnProcess;

        public ManualCommunicationAgent(ManualFabricBridge fabricBridge, CommunicationAgentCapabilities capabilities, ServiceHandlerCollectionContext shIds = null)
            : base(capabilities, shIds)
        {
            FabricBridge = fabricBridge;
        }

        #region FabricBridge
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public ManualFabricBridge FabricBridge { get; }
        #endregion


        private void ProcessInvoke(TransmissionPayload payload)
        {
            try
            {
                OnProcess?.Invoke(this, payload);
            }
            catch (Exception ex)
            {
                Collector?.LogException("ManualChannelSender/ProcessInvoke", ex);
            }
        }

        #region Inject(TransmissionPayload payload, int? priority = null)
        /// <summary>
        /// This method injects a service message manually in to the Microservice.
        /// </summary>
        /// <param name="payload">The message payload.</param>
        /// <param name="priority">The optional priority. The default is 1.</param>
        public void Inject(TransmissionPayload payload, int? priority = null)
        {
            if (this.Status != ServiceStatus.Running)
            {
                payload.SignalSuccess();
                payload.TraceWrite($"Failed: {Status}", $"{nameof(ManualCommunicationAgent)}/{nameof(Inject)}");
                return;
            }

            try
            {
                var resolvedPriority = priority ?? payload.Message.ChannelPriority;

                IClientHolderV2 client;
                if (mListenerClients.TryGetValue(resolvedPriority, out client))
                {
                    ((ManualClientHolder)client).Inject(payload);
                    payload.TraceWrite($"Success: {client.Name}", $"{nameof(ManualCommunicationAgent)}/{nameof(Inject)}");
                }
                else
                {
                    payload.TraceWrite($"Unresolved: {ChannelId}/{resolvedPriority}", $"{nameof(ManualCommunicationAgent)}/{nameof(Inject)}");
                    payload.SignalSuccess();
                }
            }
            catch (Exception ex)
            {
                payload.TraceWrite($"Error: {ex.Message}", $"{nameof(ManualCommunicationAgent)}/{nameof(Inject)}");
            }

        } 
        #endregion

        protected override IClientHolderV2 ListenerClientCreate(ListenerPartitionConfig p)
        {
            var client = new ManualClientHolder();

            return client;
        }

        protected override void ListenerClientValidate(IClientHolderV2 client, List<MessageFilterWrapper> newList)
        {
            throw new NotImplementedException();
        }

        protected override IClientHolderV2 SenderCreate(SenderPartitionConfig p)
        {
            var client = new ManualClientHolder();

            client.IncomingAction = (t) => ProcessInvoke(t);

            return client;
        }
    }
}
