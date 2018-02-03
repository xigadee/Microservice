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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class provides helper methods when dealing with the ServiceMessage transport entity.
    /// </summary>
    public static class ServiceMessageHelper
    {
        static JsonContractSerializer sSerializer = new JsonContractSerializer();

        #region PayloadJsonDeserialize(this byte[] blob)
        /// <summary>
        /// This method deserializes a Service Message using the blob provided.
        /// </summary>
        /// <param name="data">The message to deserialize.</param>
        /// <returns>A the original message.</returns>
        public static ServiceMessage PayloadJsonDeserialize(this byte[] data)
        {
            return sSerializer.Deserialize<ServiceMessage>(data);
        }
        #endregion
        #region PayloadJsonSerialize(this ServiceMessage message)
        /// <summary>
        /// This method serializes a Service Message using a JSON serializer.
        /// </summary>
        /// <param name="message">The message to clone.</param>
        /// <returns>A byte array of the original message.</returns>
        public static byte[] PayloadJsonSerialize(this ServiceMessage message)
        {
            //First clone the service message.
            return sSerializer.Serialize(message);
        } 
        #endregion

        #region Clone(this ServiceMessage message)
        /// <summary>
        /// This method clones a Service Message using a JSON serializer.
        /// </summary>
        /// <param name="message">The message to clone.</param>
        /// <returns>A clone of the original message.</returns>
        public static ServiceMessage Clone(this ServiceMessage message)
        {
            //First clone the service message.
            byte[] data = message.PayloadJsonSerialize();

            return data.PayloadJsonDeserialize();
        }
        #endregion

        #region ConvertMessageHeadersToLowercase(this ServiceMessage message)
        /// <summary>
        /// Converts the message headers to lowercase.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void ConvertMessageHeadersToLowercase(this ServiceMessage message)
        {
            message.ChannelId = message.ChannelId?.ToLowerInvariant();
            message.MessageType = message.MessageType?.ToLowerInvariant();
            message.ActionType = message.ActionType?.ToLowerInvariant();

            message.ResponseChannelId = message.ResponseChannelId?.ToLowerInvariant();
            message.ResponseMessageType = message.ResponseMessageType?.ToLowerInvariant();
            message.ResponseActionType = message.ResponseActionType?.ToLowerInvariant();
        } 
        #endregion

        #region StatusSet(this ServiceMessage message, string status, string statusDescription)
        /// <summary>
        /// This extension method sets the message status.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="status">The status string.</param>
        /// <param name="statusDescription">The status description.</param>
        public static void StatusSet(this ServiceMessage message, string status, string statusDescription)
        {
            message.Status = status;
            message.StatusDescription = statusDescription;
        }
        #endregion
        #region StatusSet(this ServiceMessage message, int status, string statusDescription)
        /// <summary>
        /// This extension method sets the message status.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="status">The status.</param>
        /// <param name="statusDescription">The status description.</param>
        public static void StatusSet(this ServiceMessage message, int status, string statusDescription)
        {
            message.Status = status.ToString();
            message.StatusDescription = statusDescription;
        }
        #endregion

        #region ResponseSet...
        /// <summary>
        /// This extension method sets response destination.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action.</param>
        /// <param name="priority">The optional priority. The default is 1.</param>
        public static void ResponseSet(this ServiceMessage message, string channelId, string messageType, string actionType, int priority = 1)
        {
            message.ResponseChannelId = channelId;
            message.ResponseMessageType = messageType;
            message.ResponseActionType = actionType;
            message.ResponseChannelPriority = priority;
        }
        /// <summary>
        /// This extension method sets response destination.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="header">The service message header.</param>
        /// <param name="priority">The optional priority. The default is 1.</param>
        public static void ResponseSet(this ServiceMessage message, ServiceMessageHeader header, int priority = 1)
        {
            message.ResponseChannelId = header.ChannelId;
            message.ResponseMessageType = header.MessageType;
            message.ResponseActionType = header.ActionType;
            message.ResponseChannelPriority = priority;
        }
        #endregion
        #region DestinationGet(this ServiceMessage message)
        /// <summary>
        /// This method gets the destination as a ServiceMessageHeader object.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>The header.</returns>
        public static ServiceMessageHeader DestinationGet(this ServiceMessage message)
        {
            return new ServiceMessageHeader(message.ChannelId, message.MessageType, message.ActionType);
        }
        #endregion
        #region ResponseGet(this ServiceMessage message)
        /// <summary>
        /// This method gets the response destination as a ServiceMessageHeader object.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>The header.</returns>
        public static ServiceMessageHeader ResponseGet(this ServiceMessage message)
        {
            return new ServiceMessageHeader(message.ResponseChannelId, message.ResponseMessageType, message.ResponseActionType);
        } 
        #endregion

        #region SetDestination...
        /// <summary>
        /// This method updates the destination and priority information for the message based on the contract and the optional priority flag.
        /// </summary>
        /// <typeparam name="C">The message contract.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="priority">This is the optional priority. If this is set to a value it will replace the original priority.</param>
        public static ServiceMessage SetDestination<C>(this ServiceMessage message, int? priority = null)
            where C : IMessageContract
        {
            string channelId, messageType, actionType;
            if (!ExtractContractInfo<C>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(C));
            return message.SetDestination(channelId, messageType, actionType, priority);
        }
        /// <summary>
        /// This method updates the destination and priority information for the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channelId">The delivery channelId</param>
        /// <param name="messageType">The message channelId</param>
        /// <param name="actionType">The message action channelId</param>
        /// <param name="priority">This is the optional priority. If this is set to a value it will replace the original priority.</param>
        /// <returns>The cloned message with the alternative destination information.</returns>
        public static ServiceMessage SetDestination(this ServiceMessage message, string channelId, string messageType, string actionType, int? priority = null)
        {
            message.ChannelId = channelId;
            message.MessageType = messageType;
            message.ActionType = actionType;

            if (priority.HasValue)
                message.ChannelPriority = priority.Value;

            return message;
        }
        /// <summary>
        /// This method updates the destination and priority information for the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="header">The service message header.</param>
        /// <param name="priority">This is the optional priority. If this is set to a value it will replace the original priority.</param>
        /// <returns>The cloned message with the alternative destination information.</returns>
        public static ServiceMessage SetDestination(this ServiceMessage message, ServiceMessageHeader header, int? priority = null)
        {
            message.ChannelId = header.ChannelId;
            message.MessageType = header.MessageType;
            message.ActionType = header.ActionType;

            if (priority.HasValue)
                message.ChannelPriority = priority.Value;

            return message;
        }
        #endregion

        #region Forward...
        /// <summary>
        /// This method clones a message and then changes its destination information to match the contract.
        /// </summary>
        /// <typeparam name="C">The contract type.</typeparam>
        /// <param name="message">The message to forward.</param>
        /// <param name="priority">This is the optional priority. If this is set to a value it will replace the original priority.</param>
        /// <returns>The cloned message with the alternative destination information.</returns>
        public static ServiceMessage Forward<C>(this ServiceMessage message, int? priority = null)
            where C : IMessageContract
        {
            return message.Clone().SetDestination<C>(priority);
        }
        /// <summary>
        /// This method clones a message and then changes its destination information to match the parameters passed.
        /// </summary>
        /// <param name="message">The original message.</param>
        /// <param name="channelId">The delivery channelId</param>
        /// <param name="messageType">The message channelId</param>
        /// <param name="actionType">The message action channelId</param>
        /// <param name="priority">This is the optional priority. If this is set to a value it will replace the original priority.</param>
        /// <returns>The cloned message with the alternative destination information.</returns>
        public static ServiceMessage Forward(this ServiceMessage message, string channelId, string messageType, string actionType, int? priority = null)
        {
            return message.Clone().SetDestination(channelId, messageType, actionType, priority);
        }
        #endregion

        #region ToResponse(this ServiceMessage message...
        /// <summary>
        /// This extension method turns a request message in to its corresponding response message by using the originator information as the new correlation information.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <returns>Returns a new message.</returns>
        public static ServiceMessage ToResponse(this ServiceMessage message)
        {
            var baseMessage = CreateMessageBase();

            baseMessage.CorrelationServiceId = message.OriginatorServiceId;
            baseMessage.CorrelationUTC = message.OriginatorUTC;
            baseMessage.CorrelationKey = message.OriginatorKey;
            baseMessage.DispatcherTransitCount = message.DispatcherTransitCount;
            baseMessage.Holder = new ServiceHandlerContext();

            return baseMessage;
        }
        #endregion
        #region ToResponse<C>(this ServiceMessage message...
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="message"></param>
        /// <param name="location"></param>
        /// <param name="correlationId"></param>
        /// <param name="messageId"></param>
        /// <param name="serviceId"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static ServiceMessage ToResponse<C>(this ServiceMessage message
            , string location = null, string correlationId = null
            , string messageId = null, string serviceId = null)
            where C:IMessageContract
        {
            var baseMessage = CreateMessageBase<C>();

            baseMessage.CorrelationServiceId = serviceId ?? message.CorrelationServiceId;
            baseMessage.CorrelationUTC = message.CorrelationUTC;
            baseMessage.CorrelationKey = correlationId ?? message.OriginatorKey;
            baseMessage.DispatcherTransitCount = message.DispatcherTransitCount;

            return baseMessage;
        }
        #endregion

        #region CreateMessageBase()
        /// <summary>
        /// This method creates a new service message base.
        /// </summary>
        /// <returns>The message.</returns>
        public static ServiceMessage CreateMessageBase()
        {
            return new ServiceMessage
            {
                IsReplay = false,
                IsNoop = false
            };
        } 
        #endregion
        #region CreateMessageBase<C>()
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <returns></returns>
        public static ServiceMessage CreateMessageBase<C>()
            where C: IMessageContract
        {
            ServiceMessage baseMessage = CreateMessageBase();

            string channelId, messageType, actionType;
            if (!ExtractContractInfo<C>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(C));
            baseMessage.ChannelId = channelId;
            baseMessage.MessageType = messageType;
            baseMessage.ActionType = actionType;

            return baseMessage;
        }
        #endregion

        #region ToServiceMessageHeader<I>()
        /// <summary>
        /// This helper creates a service message header from the contract.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns>A ServiceMessageHeader or null if the contract info cannot be matched.</returns>
        public static ServiceMessageHeader ToServiceMessageHeader<I>()
            where I: IMessageContract
        {
            string channelId, messageType, actionType;
            if (!ExtractContractInfo<I>(out channelId, out messageType, out actionType))
                return null;

            return new ServiceMessageHeader(channelId, messageType, actionType);
        }
        #endregion

        #region ExtractContractInfo<C>(out string channelId, out string messageType, out string actionType)
        /// <summary>
        /// This method extracts the three metadata parameters from the contract
        /// used to route the message around the system
        /// </summary>
        /// <typeparam name="C">The contract channelId</typeparam>
        /// <param name="channelId">The delivery channelId</param>
        /// <param name="messageType">The message channelId</param>
        /// <param name="actionType">The message action channelId</param>
        public static bool ExtractContractInfo<C>(out string channelId, out string messageType, out string actionType)
            where C : IMessageContract
        {
            return ExtractContractInfo(typeof(C), out channelId, out messageType, out actionType);
        } 
        #endregion
        #region ExtractContractInfo(ChannelId objectType, out string channelId, out string messageType, out string actionType)
        /// <summary>
        /// This method extracts the three metadata parameters from the contract
        /// used to route the message around the system
        /// </summary>
        /// <param name="objectType">The contract channelId</param>
        /// <param name="channelId">The delivery channelId</param>
        /// <param name="messageType">The message channelId</param>
        /// <param name="actionType">The message action channelId</param>
        public static bool ExtractContractInfo(Type objectType, out string channelId, out string messageType, out string actionType)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            channelId = null;
            messageType = null;
            actionType = null;

            var attributes = objectType
                .GetCustomAttributes(typeof(ContractAttribute), false)
                .Cast<ContractAttribute>()
                .ToArray();

            if (attributes.Length == 0)
                return false;

            channelId = attributes[0].ChannelId;
            messageType = attributes[0].MessageType;
            actionType = attributes[0].ActionType;

            return true;
        } 
        #endregion
    }
}