//#region Copyright
//// Copyright Hitachi Consulting
//// 
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// 
////    http://www.apache.org/licenses/LICENSE-2.0
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//#endregion

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.ServiceBus.Messaging;
//using Newtonsoft.Json;

//namespace Xigadee
//{
//    /// <summary>
//    /// This event source uses the Azure EventHub as the event source.
//    /// </summary>
//    public class QueueEventSource : AzureSBSenderBase<QueueClient, BrokeredMessage>, IEventSourceComponent
//    {
//        #region Declarations
//        /// <summary>
//        /// This is the default serializer for the outgoing content.
//        /// </summary>
//        private readonly JsonSerializer mJsonSerializer;

//        private readonly ResourceProfile mResourceProfile;
//        #endregion

//        /// <summary>
//        /// This is the name of the component.
//        /// </summary>
//        public string Name
//        {
//            get
//            {
//                return "AzureServiceBusQueue";
//            }
//        }

//        #region Constructor
//        /// <summary>
//        /// This is the default constructor for the Azure service bus sender.
//        /// </summary>
//        public QueueEventSource() :base()
//        {
//            mJsonSerializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
//            ListenerPriorityPartitions = SenderPartitionConfig.Init(1).ToList();
//        }
//        #endregion

//        #region ClientCreate()
//        /// <summary>
//        /// This override sets the transmit options for the client.
//        /// </summary>
//        /// <returns>Returns the client.</returns>
//        protected override AzureClientHolder<QueueClient, BrokeredMessage> ClientCreate(SenderPartitionConfig partition)
//        {
//            var client = base.ClientCreate(partition);
//            client.Name = mPriorityClientNamer(AzureConn.ConnectionName, partition.Priority);

//            //client.AssignMessageHelpers();

//            client.FabricInitialize = () => AzureConn.QueueFabricInitialize(client.Name);

//            //Set the method that creates the client.
//            client.ClientCreate = () => QueueClient.CreateFromConnectionString(AzureConn.ConnectionString, client.Name);

//            //We have to do this due to the stupid inheritance rules for Azure Service Bus.
//            client.MessageTransmit = async (b) => await client.Client.SendAsync(b);

//            client.MessagePack = PackEventSource;

//            return client;
//        }
//        #endregion

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="K"></typeparam>
//        /// <typeparam name="E"></typeparam>
//        /// <param name="originatorId"></param>
//        /// <param name="entry"></param>
//        /// <param name="utcTimeStamp"></param>
//        /// <param name="sync"></param>
//        /// <returns></returns>
//        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
//        {
//            try
//            {
//                var client = ClientResolve(1);
//                var payload = TransmissionPayload.Create();
//                payload.Message.Holder.SetObject(entry);

//                payload.Message.OriginatorServiceId = originatorId;
//                if (utcTimeStamp.HasValue)
//                    entry.UTCTimeStamp = utcTimeStamp.Value;

//                await client.Transmit(payload);
//            }
//            catch (Exception ex)
//            {
//                LogExceptionLocation("Write EventSource (Unhandled)", ex);
//                //OK, not sure what happened here, so we need to throw the exception.
//                throw ex;
//            }
//        }

//        #region Pack(TransmissionPayload payload)
//        /// <summary>
//        /// This method packs the ServiceMessage in to the BrokeredMessage format
//        /// for communication through the Azure Service Bus.
//        /// </summary>
//        /// <param name="payload">The payload to convert.</param>
//        /// <returns>Returns a converted BrokeredMessage from transmission.</returns>
//        public BrokeredMessage PackEventSource(TransmissionPayload payload)
//        {
//            var entry = payload.Message.Holder.Object as EventSourceEntryBase;

//            byte[] blob;

//            using (var stream = new MemoryStream())
//            using (var streamWriter = new StreamWriter(stream))
//            using (var textWriter = new JsonTextWriter(streamWriter))
//            {
//                mJsonSerializer.Serialize(textWriter, entry);
//                streamWriter.Flush();
//                stream.Position = 0;
//                blob = stream.ToArray();
//            }

//            BrokeredMessage bMessage = new BrokeredMessage(blob);

//            bMessage.Properties.Add("BatchId", entry.BatchId);
//            bMessage.Properties.Add("CorrelationId", entry.CorrelationId);
//            bMessage.Properties.Add("Key", entry.Key);
//            bMessage.Properties.Add("EntityType", entry.EntityType);
//            bMessage.Properties.Add("EntityVersion", entry.EntityVersion);
//            bMessage.Properties.Add("UTCTimeStamp", entry.UTCTimeStamp);

//            bMessage.Properties.Add("OriginatorId", OriginatorId);

//            return bMessage;
//        }
//        #endregion
//    }
//}
