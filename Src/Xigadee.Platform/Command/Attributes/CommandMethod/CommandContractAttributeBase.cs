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
    /// This is the base class.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public abstract class CommandContractAttributeBase: Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContractAttributeBase"/> class.
        /// </summary>
        /// <param name="header">The service message header.</param>
        protected CommandContractAttributeBase(ServiceMessageHeader header)
        {
            Header = header;
        }
        /// <summary>
        /// This the is the constructor when using a messaging contract.
        /// </summary>
        /// <param name="interfaceType">The interface type that inherits from IMessageContract</param>
        protected CommandContractAttributeBase(Type interfaceType)
        {
            if (!interfaceType.IsInterface || interfaceType.IsSubclassOf(typeof(IMessageContract)))
                throw new ArgumentOutOfRangeException("interfaceType must be an interface and derived from IMessageContract");

            string channelId, messageType, actionType;

            if (!ServiceMessageHelper.ExtractContractInfo(interfaceType, out channelId, out messageType, out actionType))
                throw new InvalidOperationException($"Unable to locate contract attributes for {interfaceType.Name}");

            Header = new ServiceMessageHeader(channelId, messageType, actionType);
        }
        /// <summary>
        /// The channel identifier.
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
        public ServiceMessageHeader Header { get; }
    }
}
