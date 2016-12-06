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
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This attribute can be set against a command method to register it for automatic registration as a remote command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    [DebuggerDisplay("{ChannelId}/{MessageType}/{ActionType}")]
    public class CommandContractAttribute:Attribute
    {
        public CommandContractAttribute(string channelId = null, string messageType = null, string actionType = null)
        {
            Header = new ServiceMessageHeader(channelId, messageType, actionType);
        }

        public CommandContractAttribute(string messageType, string actionType)
        {
            Header = new ServiceMessageHeader(null, messageType, actionType);
        }

        public CommandContractAttribute(Type interfaceType)
        {
            if (!interfaceType.IsInterface || interfaceType.IsSubclassOf(typeof(IMessageContract)))
                throw new ArgumentOutOfRangeException("interfaceType must be an interface and derived from IMessageContract");

            string channelId, messageType, actionType;

            if (!ServiceMessageHelper.ExtractContractInfo(interfaceType, out channelId, out messageType, out actionType))
                throw new InvalidOperationException("Unable to locate contract attributes for " + interfaceType.Name);

            Header = new ServiceMessageHeader(channelId, messageType, actionType);
        }

        /// <summary>
        /// The channelId
        /// </summary>
        public string ChannelId { get { return Header.ChannelId; } }
        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get { return Header.MessageType; } }
        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType { get { return Header.ActionType; } }

        /// <summary>
        /// The converted header.
        /// </summary>
        public ServiceMessageHeader Header { get; protected set; }
    }


}
