using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is the outwards facing interface for the command harness.
    /// </summary>
    public interface ICommandHarnessDispath: IMicroserviceDispatch
    {
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
        void Process(ServiceMessageHeaderFragment header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , ServiceMessageHeaderFragment responseHeader = null
            , int ResponseChannelPriority = 1
            );

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
        void Process(ServiceMessageHeaderFragment header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , ServiceMessageHeader responseHeader = null
            , int ResponseChannelPriority = 1
            );
    }
}
