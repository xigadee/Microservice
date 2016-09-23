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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// The contract attribute can be applied to IMessageContract to describe their specific routing data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ContractAttribute : Attribute
    {
        private readonly ServiceMessageHeader mHeader;

        /// <summary>
        /// This is the default
        /// </summary>
        /// <param name="channelId">The channelId.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="messageAction">The action type.</param>
        public ContractAttribute(string channelId, string messageType, string messageAction)
        {
            mHeader = new ServiceMessageHeader(channelId, messageType, messageAction);
        }

        /// <summary>
        /// The channelId
        /// </summary>
        public string ChannelId { get { return mHeader.ChannelId; } }
        /// <summary>
        /// The message type.
        /// </summary>
        public string MessageType { get { return mHeader.MessageType; } }
        /// <summary>
        /// The action type.
        /// </summary>
        public string ActionType { get { return mHeader.ActionType; } }
        /// <summary>
        /// The converted header.
        /// </summary>
        public ServiceMessageHeader Header { get { return mHeader; } }
    }
}
