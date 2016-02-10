using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Xigadee
{
    /// <summary>
    /// This event source uses the Azure EventHub as the event source.
    /// </summary>
    public class EventHubEventSource : AzureSBEventHubSender, IEventSource
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor for the Azure service bus sender.
        /// </summary>
        /// <param name="channelId">The channel Id of the sender.</param>
        /// <param name="connectionString">The Azure connection string.</param>
        /// <param name="connectionName">The connection name.</param>
        public EventHubEventSource(string channelId, string connectionString, string connectionName) :
            base(channelId, connectionString, connectionName)
        {
        }
        #endregion

        protected override AzureClientHolder<EventHubClient, EventData> ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);
            client.MessagePack = PackEventSource;
            return client;
        }


        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            try
            {
                var client = ClientResolve(1);
                var payload = TransmissionPayload.Create();
                payload.MessageObject = entry;

                payload.Message.OriginatorServiceId = originatorId;
                if (utcTimeStamp.HasValue)
                    entry.UTCTimeStamp = utcTimeStamp.Value;

                client.Transmit(payload);
            }
            catch (Exception ex)
            {
                LogExceptionLocation("Write EventSource (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                //throw ex;
            }
        }

        #region Pack(TransmissionPayload payload)
        /// <summary>
        /// This method packs the ServiceMessage in to the BrokeredMessage format
        /// for communication through the Azure Service Bus.
        /// </summary>
        /// <param name="sMessage">The ServiceMessage object to convert.</param>
        /// <returns>Returns a converted BrokeredMessage from transmission.</returns>
        public static EventData PackEventSource(TransmissionPayload payload)
        {
            ServiceMessage sMessage = payload.Message;
            var entry = payload.MessageObject as EventSourceEntry;

            EventData bMessage;
            if (sMessage.Blob == null)
                bMessage = new EventData();
            else
                bMessage = new EventData(sMessage.Blob);

            bMessage.Properties.Add("OriginatorServiceId", sMessage.OriginatorServiceId);

            bMessage.Properties.Add("ESEntityKey", entry.EntityKey);
            bMessage.Properties.Add("ESEntityType", entry.EntityType);
            bMessage.Properties.Add("ESEntityVersion", entry.EntityVersion);
            bMessage.Properties.Add("ESEventType", entry.EventType);
            bMessage.Properties.Add("ESUTCTimeStamp", entry.UTCTimeStamp);

            return bMessage;
        }


        #endregion
    }
}
