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

namespace Xigadee
{
    /// <summary>
    /// This command provides additional method for the command harness dispatcher.
    /// </summary>
    /// <seealso cref="Xigadee.DispatchWrapper" />
    /// <seealso cref="Xigadee.ICommandHarnessDispath" />
    public class CommandHarnessDispatchWrapper : DispatchWrapper, ICommandHarnessDispath
    {
        internal CommandHarnessDispatchWrapper(CommandPolicy policy, IPayloadSerializationContainer serializer, Action<TransmissionPayload, string> executeOrEnqueue, Func<ServiceStatus> getStatus, bool traceEnabled) 
            : base(serializer, executeOrEnqueue, getStatus, traceEnabled)
        {
            Policy = policy;
        }
        /// <summary>
        /// Gets the command policy.
        /// </summary>
        protected CommandPolicy Policy { get; }

        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="header">The message header fragment to identify the recipient. The channel will be inserted by the command harness.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed by the receiving commands.</param>
        /// <param name="responseHeader">This is the optional response header fragment. The channel will be inserted by the harness</param>
        /// <param name="ResponseChannelPriority">This is the response channel priority. This will be set if the response header is not null. The default priority is 1.</param>
        public void Process(ServiceMessageHeaderFragment header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , ServiceMessageHeaderFragment responseHeader = null
            , int ResponseChannelPriority = 1
            )
        {
            Process((Policy.ChannelId, header)
            , package
            , ChannelPriority
            , options
            , release
            , responseHeader == null? (ServiceMessageHeader)null: (Policy.ResponseChannelId, responseHeader)
            , ResponseChannelPriority
            );
        }

        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="header">The message header fragment to identify the recipient. The channel will be inserted by the command harness.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed by the receiving commands.</param>
        /// <param name="responseHeader">This is the optional response header</param>
        /// <param name="ResponseChannelPriority">This is the response channel priority. This will be set if the response header is not null. The default priority is 1.</param>
        public void Process(ServiceMessageHeaderFragment header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , ServiceMessageHeader responseHeader = null
            , int ResponseChannelPriority = 1
            )
        {
            Process((Policy.ChannelId, header)
            , package
            , ChannelPriority
            , options
            , release
            , responseHeader
            , ResponseChannelPriority
            );
        }
    }
}
