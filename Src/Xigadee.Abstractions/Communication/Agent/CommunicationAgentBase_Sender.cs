﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Xigadee
{
    public abstract partial class CommunicationAgentBase<S>: ISender
    {
        /// <summary>
        /// This is the client collection.
        /// </summary>
        protected ConcurrentDictionary<int, IClientHolderV2> mSenderClients = new ConcurrentDictionary<int, IClientHolderV2>();
        /// <summary>
        /// This is the list of active clients.
        /// </summary>
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

        /// <summary>
        /// This method starts the senders for each of the priority partitions.
        /// </summary>
        protected virtual void SendersStart() => SenderPriorityPartitions?.ForEach((p) => SenderStart(p));
        /// <summary>
        /// This abstract method starts the specific sender for a priority partition
        /// </summary>
        /// <param name="p"></param>
        protected virtual void SenderStart(SenderPartitionConfig p)
        {
            try
            {
                var client = SenderCreate(p);
                client.Priority = p.Priority;
                client.ChannelId = ChannelId;
                client.Name = $"Sender|{ChannelId}|{p.Priority}";
                mSenderClients.AddOrUpdate(client.Priority, client, (i, ct) => client);
                ServiceStart(client);
            }
            catch (Exception ex)
            {
                Collector?.LogException($"{ProtocolId} Sender Start error for partition {p.Priority}", ex);
                throw;
            }
        }

        /// <summary>
        /// This method creates a new sender client. You must override this method.
        /// </summary>
        /// <param name="p">The sender partition configuration.</param>
        /// <returns>Returns the new client.</returns>
        protected abstract IClientHolderV2 SenderCreate(SenderPartitionConfig p);
        /// <summary>
        /// This method stops each of the active senders.
        /// </summary>
        protected virtual void SendersStop() => mSenderClients?.ForEach((v) => SenderStop(v.Value));
        /// <summary>
        /// This abstract method stops a particular sender client.
        /// </summary>
        /// <param name="client"></param>
        protected virtual void SenderStop(IClientHolderV2 client) => client.Stop();


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

                payload.TraceWrite($"Sent: {sender.Name}");
            }
            catch (Exception ex)
            {
                LogExceptionLocation($"{nameof(SenderTransmit)} (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                payload.TraceWrite($"Exception: {ex.Message}");

                sender?.ErrorIncrement();

                throw;
            }
            finally
            {
                if (start.HasValue)
                    sender?.ActiveDecrement(start.Value);
            }
        }
        #endregion
    }
}