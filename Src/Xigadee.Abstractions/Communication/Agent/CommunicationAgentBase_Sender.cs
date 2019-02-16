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
        protected ConcurrentDictionary<int, IClientHolder> mSenderClients = new ConcurrentDictionary<int, IClientHolder>();

        public virtual IEnumerable<IClientHolder> SenderClients => mSenderClients.Values;

        /// <summary>
        /// This contains the sender partitions.
        /// </summary>
        public List<SenderPartitionConfig> SenderPriorityPartitions { get; set; }

        #region SenderClientResolve(int priority)
        /// <summary>
        /// Resolves the sender client resolve.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <returns>The resolved client.</returns>
        protected virtual IClientHolder SenderClientResolve(int priority)
        {
            if ((mSenderClients?.Count ?? 0) == 0)
                throw new ClientsUndefinedMessagingException($"No Clients are defined for {ChannelId}");

            if (mSenderClients.ContainsKey(priority))
                return mSenderClients[priority];

            if (!mDefaultPriority.HasValue)
                throw new ClientsUndefinedMessagingException($"Channel={ChannelId} Priority={priority} cannot be found and a default priority value has not been set.");

            return mSenderClients[mDefaultPriority.Value];
        } 
        #endregion


        #region SenderSettingsValidate()
        /// <summary>
        /// This method is called at start-up and can be used to validate any sender specific settings.
        /// </summary>
        protected virtual void SenderSettingsValidate(){} 
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
            IClientHolder sender = null;
            try
            {
                sender = SenderClientResolve(payload.Message.ChannelPriority);

                start = sender.ActiveIncrement();

                await sender.Transmit(payload);

                payload.TraceWrite($"Sent: {sender.Name}", "MessagingSenderBase/ProcessMessage");
            }
            catch (Exception ex)
            {
                LogExceptionLocation($"{nameof(SenderTransmit)} (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                payload.TraceWrite($"Exception: {ex.Message}", "MessagingSenderBase/ProcessMessage");
                if (sender != null)
                    sender.ErrorIncrement();
                throw;
            }
            finally
            {
                if (sender != null && start.HasValue)
                    sender.ActiveDecrement(start.Value);
            }
        }
        #endregion

    }
}
