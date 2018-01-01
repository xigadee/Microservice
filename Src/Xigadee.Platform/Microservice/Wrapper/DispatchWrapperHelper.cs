using System;

namespace Xigadee
{
    /// <summary>
    /// This static class provides helper function for direct dispatcher calls.
    /// </summary>
    public static class DispatchWrapperHelper
    {
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <typeparam name="C">The message contract.</typeparam>
        /// <param name="dispatcher">The Microservice dispatcher.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed by the receiving commands.</param>
        /// <param name="originatorServiceId">This optional parameter allows you to set the originator serviceId</param>
        public static void Process<C>(this IMicroserviceDispatch dispatcher
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , string originatorServiceId = null
            )
            where C : IMessageContract
        {
            string channelId, messageType, actionType;
            if (!ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType))
                throw new InvalidMessageContractException(typeof(C));

            dispatcher.Process(channelId, messageType, actionType, package, ChannelPriority, options, release, originatorServiceId);
        }
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="dispatcher">The Microservice dispatcher.</param>
        /// <param name="ChannelId">The incoming channel. This must be supplied.</param>
        /// <param name="MessageType">The message type. This may be null.</param>
        /// <param name="ActionType">The message action. This may be null.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed by the receiving commands.</param>
        /// <param name="originatorServiceId">This optional parameter allows you to set the originator serviceId</param>
        public static void Process(this IMicroserviceDispatch dispatcher
            , string ChannelId, string MessageType = null, string ActionType = null
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , string originatorServiceId = null
            )
        {
            var header = new ServiceMessageHeader(ChannelId, MessageType, ActionType);

            dispatcher.Process(header, package, ChannelPriority, options, release, originatorServiceId: originatorServiceId);
        }

        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="dispatcher">The Microservice dispatcher.</param>
        /// <param name="header">The message header to identify the recipient.</param>
        /// <param name="package">The object package to process.</param>
        /// <param name="ChannelPriority">The priority that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed by the receiving commands.</param>
        /// <param name="responseHeader">This is the optional response header</param>
        /// <param name="ResponseChannelPriority">This is the response channel priority. This will be set if the response header is not null. The default priority is 1.</param>
        /// <param name="originatorServiceId">This optional parameter allows you to set the originator serviceId</param>
        public static void Process(this IMicroserviceDispatch dispatcher
            , ServiceMessageHeader header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , ServiceMessageHeader responseHeader = null
            , int ResponseChannelPriority = 1
            , string originatorServiceId = null
            )
        {
            var message = new ServiceMessage(header, responseHeader);

            if (originatorServiceId != null)
                message.OriginatorServiceId = originatorServiceId;

            message.ChannelPriority = ChannelPriority;

            message.Blob = SerializationHolder.CreateWithObject(package);
            //if (package != null)
            //    message.Blob = dispatcher.Serialization.PayloadSerialize(package);

            if (responseHeader != null)
                message.ResponseChannelPriority = ResponseChannelPriority;

            dispatcher.Process(message, options, release);
        }

        /// <summary>
        /// This method injects a service message in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="dispatcher">The Microservice dispatcher.</param>
        /// <param name="message">The service message.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed by the receiving commands.</param>
        public static void Process(this IMicroserviceDispatch dispatcher
            , ServiceMessage message
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null)
        {
            var payload = new TransmissionPayload(message
                , release: release
                , options: options);

            dispatcher.Process(payload);
        }
    }
}
