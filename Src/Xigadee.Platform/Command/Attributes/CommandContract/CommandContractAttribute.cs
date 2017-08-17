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
    public class CommandContractAttribute: CommandContractAttributeBase
    {
        /// <summary>
        /// This is the default constructor using a three part key.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        public CommandContractAttribute(string channelId = null, string messageType = null, string actionType = null)
            : base((channelId, messageType, actionType))
        {
        }
        /// <summary>
        /// This is the default constructor using a two part key where the channelId is derived from the channel that the command is attached to.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        public CommandContractAttribute(string messageType, string actionType) 
            : base((null, messageType, actionType))
        {
        }
        /// <summary>
        /// This the is the constructor when using a messaging contract.
        /// </summary>
        /// <param name="interfaceType">The interface type that inherits from IMessageContract</param>
        public CommandContractAttribute(Type interfaceType) : base(interfaceType)
        {
        }
    }
}
