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

namespace Xigadee
{
    /// <summary>
    /// This interface is registered by the CommandContainer and is used to notify subscribing parties of changes
    /// to the active command collection. This typically can happen when master jobs start or stop during operation.
    /// </summary>
    public interface ISupportedMessageTypes
    {
        /// <summary>
        /// This event is used to signal a change of registered commands.
        /// </summary>
        event EventHandler<SupportedMessagesChangeEventArgs> OnCommandChange;
        /// <summary>
        /// This is the supported message types for the listener.
        /// </summary>
        List<MessageFilterWrapper> SupportedMessages { get; }
    }

    /// <summary>
    /// This class is used to wrap cammand message changes.
    /// </summary>
    public class SupportedMessagesChangeEventArgs:EventArgs
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="messages">The message list.</param>
        public SupportedMessagesChangeEventArgs(List<MessageFilterWrapper> messages)
        {
            Messages = messages;
        }
        /// <summary>
        /// The current list of messages supported.
        /// </summary>
        public List<MessageFilterWrapper> Messages { get; }
    }
}
