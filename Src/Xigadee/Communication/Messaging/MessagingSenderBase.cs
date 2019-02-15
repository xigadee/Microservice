#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to create senders for various communication technologies using a 
    /// generic base class.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    /// <typeparam name="M">The client message type.</typeparam>
    /// <typeparam name="H">The client holder type.</typeparam>
    public class MessagingSenderBase<C, M, H> : MessagingServiceBase<C, M, H>, ISender
        where H : ClientHolder<C, M>, new()
        where C : class
    {
        #region PriorityPartitions
        /// <summary>
        /// This is the set of priority partitions used to provide different priority for messages.
        /// </summary>
        public List<SenderPartitionConfig> SenderPriorityPartitions { get; set; }
        #endregion

        public IEnumerable<ClientHolder> SenderClients => throw new NotImplementedException();

        #region SenderTransmit(TransmissionPayload payload)
        /// <summary>
        /// This method resolves the client and processes the message.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        public virtual async Task SenderTransmit(TransmissionPayload payload)
        {
            int? start = null;
            H client = null;
            try
            {
                client = ClientResolve(payload.Message.ChannelPriority);

                start = client.StatisticsInternal.ActiveIncrement();

                await client.Transmit(payload);

                payload.TraceWrite($"Sent: {client.Name}", "MessagingSenderBase/ProcessMessage");
            }
            catch (Exception ex)
            {
                LogExceptionLocation("ProcessMessage (Unhandled)", ex);
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
