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
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Xigadee;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the service message header identifier.
    /// </summary>
    [DebuggerDisplay("Header={ChannelId}({MessageType}|{ActionType})")]
    public struct ServiceMessageHeader:IEquatable<ServiceMessageHeader>
    {
        /// <summary>
        /// This constructor is used to set the message from its component parts.
        /// </summary>
        /// <param name="channelId">The channel the command is attached to.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        public ServiceMessageHeader(string channelId, string messageType = null, string actionType = null)
        {
            ChannelId = channelId;
            MessageType = messageType;
            ActionType = actionType;
        }

        /// <summary>
        /// This property returns true if part of the key is not set.
        /// </summary>
        public bool IsPartialKey => ActionType == null || MessageType == null || ChannelId == null;

        /// <summary>
        /// The channel id
        /// </summary>
        public string ChannelId { get; }

        /// <summary>
        /// The message channelId.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// The action channelId.
        /// </summary>
        public string ActionType { get; }


        public string ToPartialKey()
        {
            StringBuilder sb = new StringBuilder();

            Action<string, string, string> action =
                (channelId, messageType, actionType) =>
            {
                if (string.IsNullOrEmpty(channelId))
                    return;
                sb.Append(channelId.ToLowerInvariant());

                if (string.IsNullOrEmpty(messageType))
                    return;
                sb.Append('/');
                sb.Append(messageType.ToLowerInvariant());

                if (string.IsNullOrEmpty(actionType))
                    return;
                sb.Append('/');
                sb.Append(actionType.ToLowerInvariant());
            };

            action(ChannelId, MessageType, ActionType);

            return sb.ToString();
        }

        public string ToKey()
        {
            return ToKey(ChannelId, MessageType, ActionType);
        }

        #region ToKey...
        /// <summary>
        /// This extracts a service message request in to a single key request.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the key.</returns>
        public static string ToKey(ServiceMessage message)
        {
            return ToKey(message.ChannelId, message.MessageType, message.ActionType);
        }

        /// <summary>
        /// This method returns a two-part command request in to a single request.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="messageType">The command channelId.</param>
        /// <param name="actionType">The command action.</param>
        /// <returns>Returns the key.</returns>
        public static string ToKey(string channelId, string messageType, string actionType)
        {
            channelId = (channelId ?? "").Trim().ToLowerInvariant();
            messageType = (messageType ?? "").Trim().ToLowerInvariant();
            actionType = (actionType ?? "").Trim().ToLowerInvariant();

            return string.Format("{0}/{1}/{2}", channelId, messageType, actionType);
        }
        #endregion

        public static ServiceMessageHeader ToServiceMessageHeader(string key)
        {
            string[] keys = key.Split('/');
            if (keys.Length != 3)
                throw new FormatException(string.Format("The key '{0}' is not of the format type/messageType/actionType", key));

            return new ServiceMessageHeader(keys[0], keys[1], keys[2]);
        }


        public bool Equals(ServiceMessageHeader other)
        {
            return ToKey() == other.ToKey();
        }

        public override bool Equals(object obj)
        {
            if (obj is ServiceMessageHeader)
                return Equals((ServiceMessageHeader)obj);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = ToKey().GetHashCode();
                return result;
            }
        }
    }
}
