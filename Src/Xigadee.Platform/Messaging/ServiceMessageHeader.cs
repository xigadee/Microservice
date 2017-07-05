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
    [DebuggerDisplay("Header={ChannelId}({MessageType}|{ActionType}) Key={ToKey()} PKey={ToPartialKey()} Partial={IsPartialKey}")]
    public struct ServiceMessageHeader:IEquatable<ServiceMessageHeader>
    {
        /// <summary>
        /// This constructor is used to set the message from its component parts.
        /// </summary>
        /// <param name="channelId">The channel that the command is attached to.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        public ServiceMessageHeader(string channelId, string messageType = null, string actionType = null)
        {
            ChannelId = NullCheck(channelId);
            MessageType = NullCheck(messageType);
            ActionType = NullCheck(actionType);
        }

        /// <summary>
        /// This method ensures that whitespace or an empty string passed as an incoming parameter is converted to null.
        /// </summary>
        /// <param name="incoming">The incoming string.</param>
        /// <returns>The outgoing string, or null if this string is just whitespace or is an empty string.</returns>
        static string NullCheck(string incoming)
        {
            if (string.IsNullOrWhiteSpace(incoming))
                return null;

            return incoming;
        }

        /// <summary>
        /// This property returns true if part of the key is not set.
        /// </summary>
        public bool IsPartialKey => ActionType == null || MessageType == null || ChannelId == null;

        /// <summary>
        /// The channel identifier.
        /// </summary>
        public string ChannelId { get; }

        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType { get; }

        /// <summary>
        /// This method returns the partial key with the relevant parts in order by channelId / messageType / actionType
        /// if at any steps these are null, the key stops and returns.
        /// </summary>
        /// <returns>A string containing the partial key.</returns>
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

        /// <summary>
        /// This returns the full key.
        /// </summary>
        /// <returns>A string containing the key.</returns>
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
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        /// <returns>Returns the key.</returns>
        public static string ToKey(string channelId, string messageType, string actionType)
        {
            channelId = (channelId ?? "").Trim();
            messageType = (messageType ?? "").Trim();
            actionType = (actionType ?? "").Trim();

            return $"{channelId}/{messageType}/{actionType}".ToLowerInvariant();
        }
        #endregion

        /// <summary>
        /// This method parses the key in to its constituent parts 
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="System.FormatException">This exception is thrown if the key passed does not contain three forward slashes in the format of channelId/messageType/actionType</exception>
        /// <returns>A new ServiceMessageHeader object.</returns>
        public static ServiceMessageHeader ToServiceMessageHeader(string key)
        {
            string[] keys = key.Split('/');
            if (keys.Length != 3)
                throw new FormatException($"The key '{key}' is not of the format channelId/messageType/actionType");

            return new ServiceMessageHeader(keys[0], keys[1], keys[2]);
        }

        /// <summary>
        /// This is the equal override that matches other structs by use of their case insensitive keys.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ServiceMessageHeader other)
        {
            return ToKey() == other.ToKey();
        }

        /// <summary>
        /// This is the equals overside.
        /// </summary>
        /// <param name="obj">The incoming object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ServiceMessageHeader)
                return Equals((ServiceMessageHeader)obj);
            return false;
        }

        /// <summary>
        /// This returns the hash code for the key.
        /// </summary>
        /// <returns>Returns the unsigned integer hashcode.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = ToKey().GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator ServiceMessageHeader(ValueTuple<string, string, string> t)
        {
            return new ServiceMessageHeader(t.Item1, t.Item2, t.Item3);
        }


        /// <summary>
        /// Implicitly converts three strings in to a ServiceMessageHeader.
        /// </summary>
        /// <param name="t">The value tuple.</param>
        public static implicit operator ServiceMessageHeader(string t)
        {
            return ServiceMessageHeader.ToServiceMessageHeader(t);
        }

        /// <summary>
        /// This is the equals operator override.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Returns true if they match.</returns>
        public static bool operator ==(ServiceMessageHeader a, ServiceMessageHeader b)
        {
            return a.Equals(b);
        }
        /// <summary>
        /// This is the not equals operator override.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Returns true if they do not match.</returns>
        public static bool operator !=(ServiceMessageHeader a, ServiceMessageHeader b)
        {
            return !(a == b);
        }
    }
}
