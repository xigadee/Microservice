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
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This static class is used to set the key properties to enable messages to be transmitted
    /// over the Azure Service Bus.
    /// </summary>
    public static class AzureServiceBusMessageHelper
    {
        #region AssignMessageHelpers<C>(this AzureClientHolder<C,BrokeredMessage> client)
        /// <summary>
        /// This extension method set the Pack, Unpack and Signal functions for Azure Service Bus support.
        /// </summary>
        /// <typeparam name="C">The Client Entity type.</typeparam>
        /// <param name="client">The client to set.</param>
        public static void AssignMessageHelpers<C>(this AzureClientHolder<C, Microsoft.Azure.ServiceBus.Message> client)
            where C : ClientEntity
        {
            client.MessagePack = Pack;
            client.MessageUnpack = Unpack;
            client.MessageSignal = MessageSignal;
        } 
        #endregion

        #region MessagePack(ServiceMessage sMessage)
        /// <summary>
        /// This method packs the ServiceMessage in to the BrokeredMessage format
        /// for communication through the Azure Service Bus.
        /// </summary>
        /// <param name="payload">The payload object to convert to a BrokeredMessage.</param>
        /// <returns>Returns a BrokeredMessage object.</returns>
        public static Microsoft.Azure.ServiceBus.Message Pack(TransmissionPayload payload)
        {
            ServiceMessage sMessage = payload.Message;
            Microsoft.Azure.ServiceBus.Message bMessage;
            if (sMessage.Blob == null)
                bMessage = new Microsoft.Azure.ServiceBus.Message();
            else
                bMessage = new Microsoft.Azure.ServiceBus.Message(sMessage.Blob);

            bMessage.UserProperties.Add("SecuritySignature", sMessage.SecuritySignature);

            bMessage.UserProperties.Add("OriginatorKey", sMessage.OriginatorKey);
            bMessage.UserProperties.Add("OriginatorServiceId", sMessage.OriginatorServiceId);
            bMessage.UserProperties.Add("OriginatorUTC", sMessage.OriginatorUTC);

            bMessage.UserProperties.Add("ResponseChannelId", MessagingHelper.ToSafeLower(sMessage.ResponseChannelId));
            bMessage.UserProperties.Add("ResponseChannelPriority", sMessage.ResponseChannelPriority.ToString());
            bMessage.UserProperties.Add("ResponseMessageType", MessagingHelper.ToSafeLower(sMessage.ResponseMessageType));
            bMessage.UserProperties.Add("ResponseActionType", MessagingHelper.ToSafeLower(sMessage.ResponseActionType));

            //FIX: Case sensitive pattern match in ServiceBus.
            bMessage.UserProperties.Add("ChannelId", MessagingHelper.ToSafeLower(sMessage.ChannelId));
            bMessage.UserProperties.Add("MessageType", MessagingHelper.ToSafeLower(sMessage.MessageType));
            bMessage.UserProperties.Add("ActionType", MessagingHelper.ToSafeLower(sMessage.ActionType));

            bMessage.UserProperties.Add("IsNoop", sMessage.IsNoop ? "1" : "0");
            bMessage.UserProperties.Add("IsReplay", sMessage.IsReplay ? "1" : "0");

            bMessage.CorrelationId = sMessage.CorrelationServiceId;

            bMessage.UserProperties.Add("ProcessCorrelationKey", sMessage.ProcessCorrelationKey);

            bMessage.UserProperties.Add("CorrelationKey", sMessage.CorrelationKey);
            bMessage.UserProperties.Add("CorrelationServiceId", MessagingHelper.ToSafeLower(sMessage.CorrelationServiceId));
            bMessage.UserProperties.Add("CorrelationUTC", sMessage.CorrelationUTC.HasValue ? sMessage.CorrelationUTC.Value.ToString("o") : null);

            bMessage.UserProperties.Add("DispatcherTransitCount", sMessage.DispatcherTransitCount);

            bMessage.UserProperties.Add("Status", sMessage.Status);
            bMessage.UserProperties.Add("StatusDescription", sMessage.StatusDescription);

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
        public static ServiceMessage Unpack(Microsoft.Azure.ServiceBus.Message bMessage)
        {
            var sMessage = new ServiceMessage();

            if (bMessage.UserProperties.ContainsKey("SecuritySignature"))
                sMessage.SecuritySignature = bMessage.UserProperties["SecuritySignature"] as string;

            sMessage.EnqueuedTimeUTC = bMessage.SystemProperties.EnqueuedTimeUtc;

            sMessage.OriginatorKey = bMessage.UserProperties["OriginatorKey"] as string;
            sMessage.OriginatorServiceId = bMessage.UserProperties["OriginatorServiceId"] as string;
            sMessage.OriginatorUTC = (DateTime)bMessage.UserProperties["OriginatorUTC"];

            sMessage.ResponseChannelId = bMessage.UserProperties["ResponseChannelId"] as string;
            if (bMessage.UserProperties.ContainsKey("ResponseMessageType"))
                sMessage.ResponseMessageType = bMessage.UserProperties["ResponseMessageType"] as string;
            if (bMessage.UserProperties.ContainsKey("ResponseActionType"))
                sMessage.ResponseActionType = bMessage.UserProperties["ResponseActionType"] as string;

            if (bMessage.UserProperties.ContainsKey("ResponseChannelPriority"))
            {
                string value = bMessage.UserProperties["ResponseChannelPriority"] as string;
                int responsePriority;
                if (string.IsNullOrEmpty(value) || !int.TryParse(value, out responsePriority))
                    responsePriority = 0;
                sMessage.ResponseChannelPriority = responsePriority;
            }

            sMessage.ChannelId = bMessage.UserProperties["ChannelId"] as string;
            sMessage.MessageType = bMessage.UserProperties["MessageType"] as string;
            sMessage.ActionType = bMessage.UserProperties["ActionType"] as string;

            sMessage.IsNoop = bMessage.UserProperties["IsNoop"] as string == "1";
            sMessage.IsReplay = bMessage.UserProperties["IsReplay"] as string == "1";

            if (bMessage.UserProperties.ContainsKey("ProcessCorrelationKey"))
                sMessage.ProcessCorrelationKey = bMessage.UserProperties["ProcessCorrelationKey"] as string;

            sMessage.CorrelationKey = bMessage.UserProperties["CorrelationKey"] as string;
            sMessage.CorrelationServiceId = bMessage.UserProperties["CorrelationServiceId"] as string;
            DateTime serviceUTC;
            if (bMessage.UserProperties.ContainsKey("CorrelationUTC") &&
                DateTime.TryParse(bMessage.UserProperties["CorrelationUTC"] as string, out serviceUTC))
                sMessage.CorrelationUTC = serviceUTC;

            sMessage.DispatcherTransitCount = (int)bMessage.UserProperties["DispatcherTransitCount"];

            sMessage.Status = bMessage.UserProperties["Status"] as string;
            sMessage.StatusDescription = bMessage.UserProperties["StatusDescription"] as string;

            sMessage.Blob = bMessage.Body;

            sMessage.FabricDeliveryCount = bMessage.SystemProperties.DeliveryCount;

            return sMessage;
        }
        #endregion

        #region MessageSignal(BrokeredMessage message, bool success)
        /// <summary>
        /// This helper method signals to the underlying fabric that the message has succeeded or failed.
        /// </summary>
        /// <param name="message">The fabric message.</param>
        /// <param name="success">The message success status.</param>
        public static void MessageSignal(Microsoft.Azure.ServiceBus.Message message, bool success)
        {
            //await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            throw new NotSupportedException();

            //if (message.SystemProperties. != MessageState.Active)
            //    return;

            //if (success)
            //    message.Complete();
            //else
            //    message.Abandon();
        } 
        #endregion
    }
}
