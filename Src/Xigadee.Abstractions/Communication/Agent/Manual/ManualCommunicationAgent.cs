using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the manual agent used to accept incoming and transmit outgoing payloads with a Manual Fabric.
    /// </summary>
    public class ManualCommunicationAgent : CommunicationAgentBase
    {
        /// <summary>
        /// Occurs when a message is sent to the sender. 
        /// This event is caught and is used to transmit to the fabric to be distributed.
        /// </summary>
        public event EventHandler<TransmissionPayload> OnProcess;

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="capabilities">The capabilities that define whether it is a listener or a sender.</param>
        /// <param name="shIds">The specified service handlers.</param>
        public ManualCommunicationAgent(CommunicationAgentCapabilities capabilities, ServiceHandlerCollectionContext shIds = null)
            : base(capabilities, shIds)
        {
        }

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
                payload.SignalFail();
                payload.TraceWrite($"Failed agent not running: {Status}");
                return;
            }

            try
            {
                var resolvedPriority = priority ?? payload.Message.ChannelPriority;

                IClientHolderV2 client;
                if (mListenerClients.TryGetValue(resolvedPriority, out client))
                {
                    ((ManualClientHolder)client).Inject(payload);
                    payload.TraceWrite($"Success: {client.Name}");
                }
                else
                {
                    payload.TraceWrite($"Failed: Unresolved client priority {ChannelId}/{resolvedPriority}");
                    payload.SignalFail();
                }
            }
            catch (Exception ex)
            {
                payload.SignalFail();
                payload.TraceWrite($"Error: {ex.Message}");
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
