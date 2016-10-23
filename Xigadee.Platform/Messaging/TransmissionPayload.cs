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
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This call wraps the incoming message and provides the ability to signal to the underlying
    /// listener that the message can be released.
    /// </summary>
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
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="release">The optional release method that can be used to signal to the underlying transport the message has completed.</param>
        /// <param name="options">The optional process options. By default this is internal or external</param>
        /// <param name="isDeadLetterMessage">A boolean flag indicating whether this is a deadletter message. By default this is false.</param>
        public TransmissionPayload(string channelId, string messageType, string actionType
            , Action<bool, Guid> release = null
            , Action<RateLimitSignal, Guid> rateLimitNotification = null
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , bool isDeadLetterMessage = false)
            : this (new ServiceMessage {ChannelId = channelId, MessageType = messageType, ActionType = actionType }, release, options, isDeadLetterMessage)
        {
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="release">The optional release method that can be used to signal to the underlying transport the message has completed.</param>
        /// <param name="options">The optional process options. By default this is internal or external</param>
        /// <param name="isDeadLetterMessage">A boolean flag indicating whether this is a deadletter message. By default this is false.</param>
        public TransmissionPayload(ServiceMessage message
            , Action<bool, Guid> release = null
            , ProcessOptions options = ProcessOptions.RouteInternal | ProcessOptions.RouteExternal
            , bool isDeadLetterMessage = false)
        {
            Message = message;
            mListenerSignalRelease = release;
            Options = options;
            DispatcherCanSignal = true;
            Id = Guid.NewGuid();
        }
        #endregion

        #region Extent
        /// <summary>
        /// This is the time that the payload has been in existence.
        /// </summary>
        public TimeSpan Extent
        {
            get { return DateTime.UtcNow - mCreateTime; }
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
        public static TransmissionPayload Create()
        {
            var message = new ServiceMessage();
            return new TransmissionPayload(message);
        }

        public static TransmissionPayload Create<T>() where T : IMessageContract
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

            return new TransmissionPayload(message);
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

        #region MessageObject
        /// <summary>
        /// This is the message object to be processed. It can be used to speed up
        /// internal transmission without serializing and deserializing objects between
        /// internal commands.
        /// </summary>
        public object MessageObject { get; set; } 
        #endregion

        #region IsDeadLetterMessage
        /// <summary>
        /// This identifies whether the incoming message is a deadletter message.
        /// </summary>
        public bool IsDeadLetterMessage { get; private set; } 
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
        /// This readonly property identifies whether the message can be singalled complete.
        /// </summary>
        public bool MessageCanSignal { get { return mListenerSignalRelease != null; } } 
        #endregion

        #region DispatcherCanSignal
        /// <summary>
        /// This property signals to the Dispatcher that it can signal to the underlying listener the message
        /// has completed. You may want to turn off this default action in specific scenarios. The default action
        /// is to signal (true).
        /// </summary>
        public bool DispatcherCanSignal { get; set; }
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
            var release = Interlocked.Exchange<Action<bool, Guid>>(ref mListenerSignalRelease, null);

            if (release != null)
                try
                {
                    release(success, Id);
                }
                catch (Exception ex)
                {
                    //We are not interested in exceptions from here.
                }
        } 
        #endregion
    }
}
