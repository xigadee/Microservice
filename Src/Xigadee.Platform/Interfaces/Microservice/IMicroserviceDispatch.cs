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
    /// This interface is used to send messages directly to the Microservice for processing.
    /// </summary>
    public interface IMicroserviceDispatch
    {
        /// <summary>
        /// This method injects a payload in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="payload">The transmission payload to execute.</param>
        void Process(TransmissionPayload payload);
        /// <summary>
        /// This method injects a service message in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// by the receiving commands.</param>
        void Process(ServiceMessage message
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , Action<bool, Guid> release = null);
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="header">The message header to identify the recipient.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// <param name="responseHeader">This is the optional response header</param>
        /// <param name="responseChannelPriority">This is the response channel priority. This will be set if the response header is not null. The default priority is 1.</param>
        void Process(ServiceMessageHeader header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , Action<bool, Guid> release = null
            , ServiceMessageHeader responseHeader = null
            , int responseChannelPriority = 1);
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="ChannelId">The incoming channel. This must be supplied.</param>
        /// <param name="MessageType">The message type. This may be null.</param>
        /// <param name="ActionType">The message action. This may be null.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// by the receiving commands.</param>
        void Process(string ChannelId, string MessageType = null, string ActionType = null
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , Action<bool, Guid> release = null);

        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <typeparam name="C">The message contract.</typeparam>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// by the receiving commands.</param>
        void Process<C>(object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , Action<bool, Guid> release = null)
            where C : IMessageContract;
    }
}
