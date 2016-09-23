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

namespace Xigadee
{
    /// <summary>
    /// This attribute can be set against a command method to register it for automatic registration as a remote command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandContractAttribute:Attribute
    {
        public CommandContractAttribute(string channelId = null, string messageType = null, string actionType = null)
        {
            ChannelId = channelId;
            MessageType = messageType;
            ActionType = actionType;
            Header = new ServiceMessageHeader(channelId, messageType, actionType);
        }

        public CommandContractAttribute(Type interfaceType)
        {
            if (!interfaceType.IsInterface || interfaceType.IsSubclassOf(typeof(IMessageContract)))
                throw new ArgumentOutOfRangeException("interfaceType must be an interface and derived from IMessageContract");

        }

        public string ChannelId { get; protected set; }

        public string MessageType { get; protected set; }

        public string ActionType { get; protected set; }

        /// <summary>
        /// The converted header.
        /// </summary>
        public ServiceMessageHeader Header { get; protected set; }
    }


}
