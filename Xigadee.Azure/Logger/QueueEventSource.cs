using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// This event source uses the Azure EventHub as the event source.
    /// </summary>
    public class QueueEventSource : AzureSBSenderBase<QueueClient, BrokeredMessage>, IEventSource
    {
        #region Declarations
        /// <summary>
        /// This is the default serializer for the outgoing content.
        /// </summary>
        private readonly JsonSerializer mJsonSerializer;

        private readonly ResourceProfile mResourceProfile;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the Azure service bus sender.
        /// </summary>
        /// <param name="channelId">The channel Id of the sender.</param>
        /// <param name="connectionString">The Azure connection string.</param>
        /// <param name="connectionName">The connection name.</param>
        public QueueEventSource(string channelId, string connectionString, string connectionName, ResourceProfile resourceProfile = null) :
            base(channelId, connectionString, connectionName, SenderPartitionConfig.Init(1))
        {
            mResourceProfile = resourceProfile;
            mJsonSerializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
        }
        #endregion

        #region ClientCreate()
        /// <summary>
        /// This override sets the transmit options for the client.
        /// </summary>
        /// <returns>Returns the client.</returns>
        protected override AzureClientHolder<QueueClient, BrokeredMessage> ClientCreate(SenderPartitionConfig partition)
        {
            var client = base.ClientCreate(partition);
            client.Name = mPriorityClientNamer(mAzureSB.ConnectionName, partition.Priority);

            //client.AssignMessageHelpers();

            client.FabricInitialize = () => mAzureSB.QueueFabricInitialize(client.Name);

            //Set the method that creates the client.
            client.ClientCreate = () => QueueClient.CreateFromConnectionString(mAzureSB.ConnectionString, client.Name);

            //We have to do this due to the stupid inheritance rules for Azure Service Bus.
            client.MessageTransmit = async (b) => await client.Client.SendAsync(b);

            client.MessagePack = PackEventSource;

            return client;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="originatorId"></param>
        /// <param name="entry"></param>
        /// <param name="utcTimeStamp"></param>
        /// <param name="sync"></param>
        /// <returns></returns>
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

                await client.Transmit(payload);
            }
            catch (Exception ex)
            {
                LogExceptionLocation("Write EventSource (Unhandled)", ex);
                //OK, not sure what happened here, so we need to throw the exception.
                throw ex;
            }
        }

        #region Pack(TransmissionPayload payload)
        /// <summary>
        /// This method packs the ServiceMessage in to the BrokeredMessage format
        /// for communication through the Azure Service Bus.
        /// </summary>
        /// <param name="payload">The payload to convert.</param>
        /// <returns>Returns a converted BrokeredMessage from transmission.</returns>
        public BrokeredMessage PackEventSource(TransmissionPayload payload)
        {
            var entry = payload.MessageObject as EventSourceEntryBase;

            byte[] blob;

            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var textWriter = new JsonTextWriter(streamWriter))
            {
                mJsonSerializer.Serialize(textWriter, payload.MessageObject);
                streamWriter.Flush();
                stream.Position = 0;
                blob = stream.ToArray();
            }

            BrokeredMessage bMessage = new BrokeredMessage(blob);

            bMessage.Properties.Add("BatchId", entry.BatchId);
            bMessage.Properties.Add("CorrelationId", entry.CorrelationId);
            bMessage.Properties.Add("Key", entry.Key);
            bMessage.Properties.Add("EntityType", entry.EntityType);
            bMessage.Properties.Add("EntityVersion", entry.EntityVersion);
            bMessage.Properties.Add("UTCTimeStamp", entry.UTCTimeStamp);

            bMessage.Properties.Add("OriginatorId", OriginatorId);

            return bMessage;
        }
        #endregion
    }
}
