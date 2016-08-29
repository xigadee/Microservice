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
    /// <typeparam name="H">The clientholder type.</typeparam>
    public class MessagingSenderBase<C, M, H> : MessagingServiceBase<C, M, H, SenderPartitionConfig>, ISender
        where H : ClientHolder<C, M>, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The string based channel id.</param>
        /// <param name="priorityPartitions">The number of priority channels. Null denotes a single channel of priority one.</param>
        public MessagingSenderBase(string channelId
            , IEnumerable<SenderPartitionConfig> priorityPartitions
            , IBoundaryLogger boundaryLogger = null)
            : base(channelId, priorityPartitions, boundaryLogger)
        {
        } 
        #endregion

        #region ProcessMessage(TransmissionPayload payload)
        /// <summary>
        /// This method resolves the client and processes the message.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        public virtual async Task ProcessMessage(TransmissionPayload payload)
        {
            int? start = null;
            H client = null;
            try
            {
                client = ClientResolve(payload.Message.ChannelPriority);
                start = client.Statistics.ActiveIncrement();
                await client.Transmit(payload);
            }
            catch (Exception ex)
            {
                LogExceptionLocation("ProcessMessage (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                if (client != null)
                    client.Statistics.ErrorIncrement();
                throw;
            }
            finally
            {
                if (client != null && start.HasValue)
                    client.Statistics.ActiveDecrement(start.Value);
            }
        } 
        #endregion
    }
}
