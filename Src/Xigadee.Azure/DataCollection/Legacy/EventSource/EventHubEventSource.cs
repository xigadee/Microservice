#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
    public class EventHubEventSource : AzureSBEventHubSender, IEventSourceComponent
    {

        public string Name
        {
            get
            {
                return "AzureEventSource";
            }
        }

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
                payload.Message.Holder.SetObject(entry);

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
        /// <param name="payload">The Transmission payload.</param>
        /// <returns>Returns a converted BrokeredMessage from transmission.</returns>
        public static EventData PackEventSource(TransmissionPayload payload)
        {
            ServiceMessage sMessage = payload.Message;
            var entry = payload.Message.Holder.Object as EventSourceEntry;

            EventData bMessage;
            if (sMessage.Holder == null)
                bMessage = new EventData();
            else
                bMessage = new EventData(sMessage.Holder);

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
