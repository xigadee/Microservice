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
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This call wraps the incoming message and provides the ability to signal to the underlying
    /// listener that the message can be released.
    /// </summary>
    [DebuggerDisplay("S={SignalResult} {Message}")]
    public class TransmissionPayload
    {
        #region Declarations
        /// <summary>
        /// This is the unique id field.
        /// </summary>
        public readonly Guid Id;
        /// <summary>
        /// This action is used to signal that the message can be released back to the listener.
        /// </summary>
        private Action<bool, Guid> mListenerSignalRelease = null;
        /// <summary>
        /// This is the time that the payload was created.
        /// </summary>
        private readonly DateTime mCreateTime = DateTime.UtcNow;

        /// <summary>
        /// Gets the tick count when the payload was created.
        /// </summary>
        public int TickCount { get; } = Environment.TickCount;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="channelId">The channel Id.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="actionType">The action type.</param>
        /// <param name="release">The optional release method that can be used to signal to the underlying transport the message has completed.</param>
        /// <param name="options">The optional process options. By default this is internal or external</param>
        /// <param name="traceEnabled">Specifies whether the trace is enabled.</param>
        public TransmissionPayload(string channelId, string messageType, string actionType
            , Action<bool, Guid> release = null
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , bool traceEnabled = false)
            : this(new ServiceMessage { ChannelId = channelId, MessageType = messageType, ActionType = actionType }, release, options, traceEnabled)
        {
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="release">The optional release method that can be used to signal to the underlying transport the message has completed.</param>
        /// <param name="options">The optional process options. By default this is internal or external</param>
        /// <param name="traceEnabled">Specifies whether the trace is enabled.</param>
        public TransmissionPayload(ServiceMessage message
            , Action<bool, Guid> release = null
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , bool traceEnabled = false)
        {
            Message = message;
            mListenerSignalRelease = release;
            Options = options;
            Id = Guid.NewGuid();
            TraceEnabled = traceEnabled;
        }
        #endregion

        #region Extent
        /// <summary>
        /// This is the time that the payload has been in existence.
        /// </summary>
        public TimeSpan Extent
        {
            get { return (DateTime.UtcNow) - mCreateTime; }
        }
        #endregion
        #region CommsWait
        /// <summary>
        /// This is the time that the message spent on the fabric before it was received.
        /// </summary>
        public TimeSpan? CommsWait
        {
            get
            {
                DateTime? start = Message?.EnqueuedTimeUTC;
                if (!start.HasValue)
                    return null;

                var diff = mCreateTime - start.Value;

                if (diff.TotalMilliseconds < 0)
                    return TimeSpan.Zero;

                return diff;
            }
        }
        #endregion

        #region Static Create helper methods
        /// <summary>
        /// This static method creates a new TransmissionPayload object with an empty ServiceMessage.
        /// </summary>
        /// <returns>Returns the payload.</returns>
        public static TransmissionPayload Create(bool traceEnabled = false)
        {
            var message = new ServiceMessage();
            return new TransmissionPayload(message, traceEnabled: traceEnabled);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TransmissionPayload Create<T>(bool traceEnabled = false) where T : IMessageContract
        {
            string channelId, messageType, actionType;

            if (!ServiceMessageHelper.ExtractContractInfo(typeof(T), out channelId, out messageType, out actionType))
                throw new InvalidOperationException("Unable to locate contract attributes for " + typeof(T));

            var message = new ServiceMessage
            {
                ChannelId = channelId,
                MessageType = messageType,
                ActionType = actionType
            };

            return new TransmissionPayload(message, traceEnabled: traceEnabled);
        }
        #endregion

        #region Cancel
        /// <summary>
        /// This is the cancellation for the payload and should be checked for long running tasks.
        /// </summary>
        public CancellationToken Cancel { get; set; }
        #endregion

        #region Message
        /// <summary>
        /// This is the incoming service message to be processed.
        /// </summary>
        public ServiceMessage Message { get; private set; }
        #endregion

        #region Options
        /// <summary>
        /// This provides custom routing instructions to the dispatcher.
        /// </summary>
        public ProcessOptions Options { get; set; }
        #endregion

        #region Source
        /// <summary>
        /// This is the name of the client or component that created the message.
        /// </summary>
        public string Source { get; set; }
        #endregion

        #region MaxProcessingTime
        /// <summary>
        /// This is the expected processing time for the request. If this is not set the default will be used.
        /// </summary>
        public TimeSpan? MaxProcessingTime { get; set; }
        #endregion

        #region MessageCanSignal
        /// <summary>
        /// This readonly property identifies whether the message can be signalled complete.
        /// </summary>
        public bool MessageCanSignal { get { return mListenerSignalRelease != null; } }
        #endregion

        #region SignalSuccess()
        /// <summary>
        /// This method signal success for the message.
        /// </summary>
        public void SignalSuccess()
        {
            Signal(true);
        }
        #endregion
        #region SignalFail()
        /// <summary>
        /// This method signals a failure for the message.
        /// </summary>
        public void SignalFail()
        {
            Signal(false);
        }
        #endregion
        #region Signal(bool success)
        /// <summary>
        /// This method removes the message action and signals. 
        /// This can only be executed once.
        /// </summary>
        /// <param name="success">A boolean parameter to signal if the message was successful.</param>
        public void Signal(bool success)
        {
            //We only want to do this once.
            var release = Interlocked.Exchange<Action<bool, Guid>>(ref mListenerSignalRelease, null);

            if (release != null)
                try
                {
                    release(success, Id);
                    SignalResult = success;
                    TraceWrite(success?"Success":"Failure", "TransmissionPayload/Signal");
                }
                catch (Exception ex)
                {
                    //We are not interested in exceptions from here.
                    TraceWrite($"Exception: {ex.Message}", "TransmissionPayload/Signal");
                }
        }
        #endregion
        /// <summary>
        /// Records the signalled result.
        /// </summary>
        public bool? SignalResult { get; private set; }

        #region SecurityPrincipal
        /// <summary>
        /// This is the security principal generated from the incoming service message security headers.
        /// </summary>
        public ClaimsPrincipal SecurityPrincipal { get; set; }

        public static ClaimsPrincipal ConvertToClaimsPrincipal(IPrincipal principal)
        {
            if (principal == null)
                return null;

            if (principal is ClaimsPrincipal)
                return (ClaimsPrincipal)principal;

            return new ClaimsPrincipal(principal);
        }
        #endregion

        #region CompleteSet()
        /// <summary>
        /// This method signals the transmission as complete and is used to signal to comms technologies that can peek
        /// a message while processing is being completed.
        /// </summary>
        public void CompleteSet()
        {
            if (!ExecutionTime.HasValue)
                ExecutionTime = ConversionHelper.DeltaAsTimeSpan(TickCount);
        }

        #endregion
        #region ExecutionTime
        /// <summary>
        /// This is the overall execution time.
        /// </summary>
        public TimeSpan? ExecutionTime { get; private set; }
        #endregion

        #region Trace ...
        /// <summary>
        /// Gets or sets a value indicating whether trace is enabled for the request.
        /// </summary>
        public bool TraceEnabled { get; set; }

        /// <summary>
        /// Sets the trace configuration using a OR operator.
        /// </summary>
        /// <param name="enable">if set to true, trace is enable. If set to false, then will be enabled is already set.</param>
        public void TraceConfigure(bool enable)
        {
            TraceEnabled |= enable;
        }

        /// <summary>
        /// Gets the trace log collection.
        /// </summary>
        public List<TransmissionPayloadTraceEventArgs> TraceLog { get; private set; }

        private object mTraceObj = new object();
        /// <summary>
        /// Adds a trace event to the log.
        /// </summary>
        /// <param name="eventArgs">The <see cref="TransmissionPayloadTraceEventArgs"/> instance containing the event data.</param>
        public void TraceWrite(TransmissionPayloadTraceEventArgs eventArgs)
        {
            if (!TraceEnabled)
                return;

            lock (mTraceObj)
            {
                if (TraceLog == null)
                    TraceLog = new List<TransmissionPayloadTraceEventArgs>();

                TraceLog.Add(eventArgs);
            }
        }

        /// <summary>
        /// Traces the set.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The optional source parameter.</param>
        public void TraceWrite(string message, string source = null)
        {
            if (!TraceEnabled)
                return;

            TraceWrite(new TransmissionPayloadTraceEventArgs(TickCount, message, source));
        } 
        #endregion
    }
}
