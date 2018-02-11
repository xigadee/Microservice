using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract partial class CommunicationAgentBase<S>: ISender
    {
        /// <summary>
        /// This is the client collection.
        /// </summary>
        protected ConcurrentDictionary<int, ClientHolder> mSenderClients = new ConcurrentDictionary<int, ClientHolder>();

        public virtual IEnumerable<ClientHolder> SenderClients => mSenderClients.Values;

        public List<SenderPartitionConfig> SenderPriorityPartitions { get;set; }

        protected abstract ClientHolder SenderClientResolve(int priority);


        #region SenderSettingsValidate()
        /// <summary>
        /// This method is called at start-up and can be used to validate any sender specific settings.
        /// </summary>
        protected virtual void SenderSettingsValidate()
        {

        } 
        #endregion

        public virtual void SenderStart()
        {

        }
        public virtual void SenderStop()
        {

        }

        #region --> SenderTransmit(TransmissionPayload payload)
        /// <summary>
        /// This method resolves the client and processes the message.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        public virtual async Task SenderTransmit(TransmissionPayload payload)
        {
            int? start = null;
            ClientHolder client = null;
            try
            {
                client = SenderClientResolve(payload.Message.ChannelPriority);

                start = client.StatisticsInternal.ActiveIncrement();

                await client.Transmit(payload);

                payload.TraceWrite($"Sent: {client.Name}", "MessagingSenderBase/ProcessMessage");
            }
            catch (Exception ex)
            {
                LogExceptionLocation($"{nameof(SenderTransmit)} (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                payload.TraceWrite($"Exception: {ex.Message}", "MessagingSenderBase/ProcessMessage");
                if (client != null)
                    client.StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                if (client != null && start.HasValue)
                    client.StatisticsInternal.ActiveDecrement(start.Value);
            }
        }
        #endregion

    }
}
