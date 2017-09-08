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
    /// This class is used to wrap the dispatch functionality.
    /// </summary>
    public class DispatchWrapper: WrapperBase, IMicroserviceDispatch
    {
        private Action<TransmissionPayload, string> ExecuteOrEnqueue { get; }

        private readonly IPayloadSerializationContainer mSerializer;

        /// <summary>
        /// Identifies whether the payload trace is enabled.
        /// </summary>
        protected readonly bool mTransmissionPayloadTraceEnabled;

        internal DispatchWrapper(IPayloadSerializationContainer serializer, Action<TransmissionPayload, string> executeOrEnqueue 
            ,Func<ServiceStatus> getStatus, bool traceEnabled) : base(getStatus)
        {
            ExecuteOrEnqueue = executeOrEnqueue;
            mSerializer = serializer;
            mTransmissionPayloadTraceEnabled = traceEnabled;
        }

        #region --> Process ...
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <typeparam name="C">The message contract.</typeparam>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// by the receiving commands.</param>
        public void Process<C>(object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null)
            where C : IMessageContract
        {
            string channelId, messageType, actionType;
            if (!ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(C));

            Process(channelId, messageType, actionType, package, ChannelPriority, options, release);
        }
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
        public void Process(string ChannelId, string MessageType = null, string ActionType = null
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null)
        {
            var header = new ServiceMessageHeader(ChannelId, MessageType, ActionType);

            Process(header, package, ChannelPriority, options, release);
        }

        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="header">The message header to identify the recipient.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// by the receiving commands.</param>
        public void Process(ServiceMessageHeader header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null)
        {
            var message = new ServiceMessage(header);
            message.ChannelPriority = ChannelPriority;
            if (package != null)
                message.Blob = mSerializer.PayloadSerialize(package);

            Process(message, options, release);
        }

        /// <summary>
        /// This method injects a service message in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// by the receiving commands.</param>
        public void Process(ServiceMessage message
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null)
        {
            var payload = new TransmissionPayload(message
                , release: release
                , options: options);

            Process(payload);
        }

        /// <summary>
        /// This method injects a payload in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="payload">The transmission payload to execute.</param>
        public void Process(TransmissionPayload payload)
        {
            ValidateServiceStarted();

            if (mTransmissionPayloadTraceEnabled)
            {
                payload.TraceEnabled = true;
                payload.TraceWrite("howdy");
            }

            ExecuteOrEnqueue(payload, "Incoming Process method request");
        }
        #endregion
    }
}
