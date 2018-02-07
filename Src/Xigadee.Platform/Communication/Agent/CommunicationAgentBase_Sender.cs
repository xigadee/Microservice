using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract partial class CommunicationAgentBase<S>
    {
        public virtual IEnumerable<ClientHolder> SenderClients { get; }

        #region SenderTransmit(TransmissionPayload payload)
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

        protected abstract ClientHolder SenderClientResolve(int priority);
    }
}
