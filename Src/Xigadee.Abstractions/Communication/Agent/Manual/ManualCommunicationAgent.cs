//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Xigadee
//{
//    public class ManualCommunicationAgent: CommunicationAgentBase
//    {
//        public ManualCommunicationAgent(ServiceHandlerIdCollection shIds = null) 
//            : base(CommunicationAgentCapabilities.Bidirectional, shIds)
//        {
//        }

//        #region Fabric
//        /// <summary>
//        /// This is the Azure connection class.
//        /// </summary>
//        public ManualFabricBridge Fabric { get; set; }
//        #endregion

//        /// <summary>
//        /// Occurs when a message is sent to the sender. This event is caught and is used to map to corresponding listeners.
//        /// </summary>
//        public event EventHandler<TransmissionPayload> OnProcess;

//        private void ProcessInvoke(TransmissionPayload payload)
//        {
//            try
//            {
//                OnProcess?.Invoke(this, payload);
//            }
//            catch (Exception ex)
//            {
//                Collector?.LogException("ManualChannelSender/ProcessInvoke", ex);
//            }
//        }

//        /// <summary>
//        /// This method injects a service message manually in to the Microservice.
//        /// </summary>
//        /// <param name="payload">The message payload.</param>
//        /// <param name="priority">The optional priority. The default is 1.</param>
//        public void Inject(TransmissionPayload payload, int? priority = null)
//        {
//            if (this.Status != ServiceStatus.Running)
//            {
//                payload.SignalSuccess();
//                payload.TraceWrite($"Failed: {Status}", "ManualChannelListener/Inject");
//                return;
//            }

//            try
//            {
//                var client = ClientResolve(priority ?? mDefaultPriority ?? 1);
//                client.Inject(payload);
//                payload.TraceWrite($"Success: {client.Name}", "ManualChannelListener/Inject");
//            }
//            catch (Exception ex)
//            {
//                payload.TraceWrite($"Error: {ex.Message}", "ManualChannelListener/Inject");
//            }

//        }

//        protected override IClientHolderV2 ListenerClientCreate(ListenerPartitionConfig p)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void ListenerClientValidate(IClientHolderV2 client, List<MessageFilterWrapper> newList)
//        {
//            throw new NotImplementedException();
//        }

//        protected override IClientHolderV2 SenderCreate(SenderPartitionConfig p)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
