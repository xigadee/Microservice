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
    /// This class holds the command message notification.
    /// </summary>
    public class CommandHolder: IEquatable<CommandHolder>
    {
        public CommandHolder(MessageFilterWrapper message, Func<TransmissionPayload, List<TransmissionPayload>, Task> command, string referenceId = null)
        {
            if (message == null)
                throw new CommandHolderException("message cannot be null");

            if (command == null)
                throw new CommandHolderException("command cannot be null");

            if (message.Header.IsPartialKey && message.Header.ChannelId == null)
                throw new CommandHolderException("You must supply a channel when using a partial key.");

            Message = message;
            Command = command;
            ReferenceId = referenceId;
        }
        /// <summary>
        /// This is the message filter.
        /// </summary>
        public MessageFilterWrapper Message { get; set; }

        /// <summary>
        /// This is the reference id used for aiding debugging.
        /// </summary>
        public string ReferenceId { get; set; }
        /// <summary>
        /// This is the actual function to execute the command.
        /// </summary>
        public Func<TransmissionPayload, List<TransmissionPayload>, Task> Command { get; set; }
        /// <summary>
        /// This override checks the items that are equivalent.
        /// </summary>
        /// <param name="other">The other command handle.</param>
        /// <returns>Returns true if they support the same message.</returns>
        public bool Equals(CommandHolder other)
        {
            return other.Message == Message;
        }
    }
}
