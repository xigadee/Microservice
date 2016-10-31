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

#region using
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public static class BrokeredMessageHelper
    {
        public static void AssignMessageHelpers<C>(this AzureClientHolder<C,BrokeredMessage> client)
            where C : ClientEntity
        {
            client.MessagePack = Pack;
            client.MessageUnpack = Unpack;
            client.MessageSignal = MessageSignal;
        }

        #region ToSafeLower(string value)
        /// <summary>
        /// This method is to fix an issue on service bus where filters are case sensitive
        /// but our message types and action types are not.
        /// </summary>
        /// <param name="value">The incoming value.</param>
        /// <returns>The outgoing lowercase value.</returns>
        private static string ToSafeLower(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.ToLowerInvariant();
        } 
        #endregion

        #region MessagePack(ServiceMessage sMessage)
        /// <summary>
        /// This method packs the ServiceMessage in to the BrokeredMessage format
        /// for communication through the Azure Service Bus.
        /// </summary>
        /// <param name="sMessage">The ServiceMessage object to convert.</param>
        /// <returns>Returns a converted BrokeredMessage from transmission.</returns>
        public static BrokeredMessage Pack(TransmissionPayload payload)
        {
            ServiceMessage sMessage = payload.Message;
            BrokeredMessage bMessage;
            if (sMessage.Blob == null)
                bMessage = new BrokeredMessage();
            else
                bMessage = new BrokeredMessage(sMessage.Blob);

            bMessage.Properties.Add("SecurityHeader", sMessage.SecurityHeader);
            bMessage.Properties.Add("SecurityPayload", sMessage.SecurityPayload);
            bMessage.Properties.Add("SecuritySignature", sMessage.SecuritySignature);

            bMessage.Properties.Add("OriginatorKey", sMessage.OriginatorKey);
            bMessage.Properties.Add("OriginatorServiceId", sMessage.OriginatorServiceId);
            bMessage.Properties.Add("OriginatorUTC", sMessage.OriginatorUTC);

            bMessage.Properties.Add("ResponseChannelId", ToSafeLower(sMessage.ResponseChannelId));
            bMessage.Properties.Add("ResponseChannelPriority", sMessage.ResponseChannelPriority.ToString());
            bMessage.Properties.Add("ResponseMessageType", ToSafeLower(sMessage.ResponseMessageType));
            bMessage.Properties.Add("ResponseActionType", ToSafeLower(sMessage.ResponseActionType));

            //FIX: Case sensitive pattern matchin in ServiceBus.
            bMessage.Properties.Add("ChannelId", ToSafeLower(sMessage.ChannelId));
            bMessage.Properties.Add("MessageType", ToSafeLower(sMessage.MessageType));
            bMessage.Properties.Add("ActionType", ToSafeLower(sMessage.ActionType));

            bMessage.Properties.Add("IsNoop", sMessage.IsNoop ? "1" : "0");
            bMessage.Properties.Add("IsReplay", sMessage.IsReplay ? "1" : "0");

            bMessage.CorrelationId = sMessage.CorrelationServiceId;

            bMessage.Properties.Add("ProcessCorrelationKey", sMessage.ProcessCorrelationKey);

            bMessage.Properties.Add("CorrelationKey", sMessage.CorrelationKey);
            bMessage.Properties.Add("CorrelationServiceId", ToSafeLower(sMessage.CorrelationServiceId));
            bMessage.Properties.Add("CorrelationUTC", sMessage.CorrelationUTC.HasValue ? sMessage.CorrelationUTC.Value.ToString("o") : null);

            bMessage.Properties.Add("DispatcherTransitCount", sMessage.DispatcherTransitCount);

            bMessage.Properties.Add("Status", sMessage.Status);
            bMessage.Properties.Add("StatusDescription", sMessage.StatusDescription);

            return bMessage;
        }
        #endregion
        #region MessageUnpack(BrokeredMessage bMessage)
        /// <summary>
        /// This method unpacks the azure service bus message in to a generic ServiceMessage object
        /// which can be processed by the service..
        /// /// </summary>
        /// <param name="bMessage">The Azure BrokeredMessage to convert.</param>
        /// <returns>Returns a generic ServiceMessage class for processing.</returns>
        public static ServiceMessage Unpack(BrokeredMessage bMessage)
        {
            var sMessage = new ServiceMessage();

            if (bMessage.Properties.ContainsKey("SecurityHeader"))
                sMessage.SecurityHeader = bMessage.Properties["SecurityHeader"] as string;

            if (bMessage.Properties.ContainsKey("SecurityPayload"))
                sMessage.SecurityPayload = bMessage.Properties["SecurityPayload"] as string;

            if (bMessage.Properties.ContainsKey("SecuritySignature"))
                sMessage.SecuritySignature = bMessage.Properties["SecuritySignature"] as string;

            sMessage.EnqueuedTimeUTC = bMessage.EnqueuedTimeUtc;

            sMessage.OriginatorKey = bMessage.Properties["OriginatorKey"] as string;
            sMessage.OriginatorServiceId = bMessage.Properties["OriginatorServiceId"] as string;
            sMessage.OriginatorUTC = (DateTime)bMessage.Properties["OriginatorUTC"];

            sMessage.ResponseChannelId = bMessage.Properties["ResponseChannelId"] as string;
            if (bMessage.Properties.ContainsKey("ResponseMessageType"))
                sMessage.ResponseMessageType = bMessage.Properties["ResponseMessageType"] as string;
            if (bMessage.Properties.ContainsKey("ResponseActionType"))
                sMessage.ResponseActionType = bMessage.Properties["ResponseActionType"] as string;

            if (bMessage.Properties.ContainsKey("ResponseChannelPriority"))
            {
                string value = bMessage.Properties["ResponseChannelPriority"] as string;
                int responsePriority;
                if (string.IsNullOrEmpty(value) || !int.TryParse(value, out responsePriority))
                    responsePriority = 0;
                sMessage.ResponseChannelPriority = responsePriority;
            }

            sMessage.ChannelId = bMessage.Properties["ChannelId"] as string;
            sMessage.MessageType = bMessage.Properties["MessageType"] as string;
            sMessage.ActionType = bMessage.Properties["ActionType"] as string;

            sMessage.IsNoop = bMessage.Properties["IsNoop"] as string == "1";
            sMessage.IsReplay = bMessage.Properties["IsReplay"] as string == "1";

            if (bMessage.Properties.ContainsKey("ProcessCorrelationKey"))
                sMessage.ProcessCorrelationKey = bMessage.Properties["ProcessCorrelationKey"] as string;

            sMessage.CorrelationKey = bMessage.Properties["CorrelationKey"] as string;
            sMessage.CorrelationServiceId = bMessage.Properties["CorrelationServiceId"] as string;
            DateTime serviceUTC;
            if (bMessage.Properties.ContainsKey("CorrelationUTC") &&
                DateTime.TryParse(bMessage.Properties["CorrelationUTC"] as string, out serviceUTC))
                sMessage.CorrelationUTC = serviceUTC;

            sMessage.DispatcherTransitCount = (int)bMessage.Properties["DispatcherTransitCount"];

            sMessage.Status = bMessage.Properties["Status"] as string;
            sMessage.StatusDescription = bMessage.Properties["StatusDescription"] as string;

            sMessage.Blob = bMessage.GetBody<byte[]>();

            sMessage.FabricDeliveryCount = bMessage.DeliveryCount;

            return sMessage;
        }
        #endregion 

        public static void MessageSignal(BrokeredMessage message, bool success)
        {
            if (message.State != MessageState.Active)
                return;

            if (success)
                message.Complete();
            else
                message.Abandon();
        }
    }
}
