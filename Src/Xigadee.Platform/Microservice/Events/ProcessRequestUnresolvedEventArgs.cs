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
    /// This event argument class is used to notify when a message cannot be resolved.
    /// </summary>
    /// <seealso cref="Xigadee.MicroserviceEventArgs" />
    public class DispatcherRequestUnresolvedEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherRequestUnresolvedEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="reason">The failure reason.</param>
        public DispatcherRequestUnresolvedEventArgs(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason, DispatcherUnhandledAction policy)
        {
            Payload = payload;
            Reason = reason;
            Policy = policy;
        }
        /// <summary>
        /// The message payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// THe failure reason.
        /// </summary>
        public DispatcherRequestUnresolvedReason Reason { get; }
        /// <summary>
        /// This is the policy. You can change this in the event if you so wish.
        /// </summary>
        public DispatcherUnhandledAction Policy { get; set; }
    }

    /// <summary>
    /// This enumeration specifies the reason a message cannot be processed.
    /// </summary>
    public enum DispatcherRequestUnresolvedReason
    {
        /// <summary>
        /// The message handler not found, and the message was for internal processing only.
        /// </summary>
        MessageHandlerNotFound,
        /// <summary>
        /// The message could be processed externally but the outgoing channel could not be resolved.
        /// </summary>
        ChannelOutgoingNotFound
    }
}
