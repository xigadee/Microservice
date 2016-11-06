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
        #region DestinationSet...
        /// <summary>
        /// This extension method sets destination.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="channelId">The channel id.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action.</param>
        /// <param name="priority">The optional priority. The default is 1.</param>
        public static void DestinationSet(this ServiceMessage message, string channelId, string messageType, string actionType, int priority = 1)
        {
            message.ChannelId = channelId;
            message.MessageType = messageType;
            message.ActionType = actionType;
            message.ChannelPriority = priority;
        }
        /// <summary>
        /// This extension method sets destination.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="header">The service message header.</param>
        /// <param name="priority">The optional message priority. The default is 1.</param>
        public static void DestinationSet(this ServiceMessage message, ServiceMessageHeader header, int priority = 1)
        {
            message.ChannelId = header.ChannelId;
            message.MessageType = header.MessageType;
            message.ActionType = header.ActionType;
            message.ChannelPriority = priority;
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
        /// <summary>
        /// This method gets the destination as a ServiceMessageHeader object.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>The header.</returns>
        public static ServiceMessageHeader DestinationGet(this ServiceMessage message)
        {
            return new ServiceMessageHeader(message.ChannelId, message.MessageType, message.ActionType);
        }
        /// <summary>
        /// This method gets the response destination as a ServiceMessageHeader object.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>The header.</returns>
        public static ServiceMessageHeader ResponseGet(this ServiceMessage message)
        {
            return new ServiceMessageHeader(message.ResponseChannelId, message.ResponseMessageType, message.ResponseActionType);
        }

        #region ToResponse(this ServiceMessage message...
        public static ServiceMessage ToResponse(this ServiceMessage message, byte[] blob = null)
        {
            var baseMessage = CreateMessageBase();

            baseMessage.CorrelationKey = message.OriginatorKey;
            baseMessage.CorrelationServiceId = message.OriginatorServiceId;
            baseMessage.CorrelationUTC = message.OriginatorUTC;

            baseMessage.DispatcherTransitCount = message.DispatcherTransitCount;

            baseMessage.Blob = blob;
            return baseMessage;
        }
        #endregion

        #region ToResponse<C>(this ServiceMessage message...
        public static ServiceMessage ToResponse<C>(this ServiceMessage message
            , string location = null, string correlationId = null
            , string messageId = null, string serviceId = null
            , byte[] blob = null)
            where C:IMessageContract
        {
            var baseMessage = CreateMessageBase<C>();

            baseMessage.CorrelationServiceId = serviceId ?? message.CorrelationServiceId;
            baseMessage.CorrelationUTC = message.CorrelationUTC;
            baseMessage.CorrelationKey = correlationId ?? message.OriginatorKey;
            baseMessage.DispatcherTransitCount = message.DispatcherTransitCount;

            baseMessage.Blob = blob;
            return baseMessage;
        }
        #endregion

        #region CreateMessageBase()
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
        public static ServiceMessage CreateMessageBase<C>()
            where C: IMessageContract
        {
            ServiceMessage baseMessage = CreateMessageBase();

            string channelId, messageType, actionType;
            ExtractContractInfo<C>(out channelId, out messageType, out actionType);
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
        public static ServiceMessageHeader? ToServiceMessageHeader<I>()
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
