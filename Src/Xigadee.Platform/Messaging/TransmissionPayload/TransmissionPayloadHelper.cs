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
    /// This class provides static extension methods for the transmission payload.
    /// </summary>
    public static class TransmissionPayloadHelper
    {
        #region Clone(this TransmissionPayload inPayload, Action<bool, Guid> signal, bool? traceEnabled = null)
        /// <summary>
        /// This method separates the payloads so that they are different objects.
        /// </summary>
        /// <param name="inPayload">The incoming payload.</param>
        /// <param name="signal">The optional signal action.</param>
        /// <param name="traceEnabled">Specifies whether trace is enabled. If omitted or set to null, the value inherits from the incoming payload.</param>
        /// <returns>Returns a new cloned payload.</returns>
        public static TransmissionPayload Clone(this TransmissionPayload inPayload, Action<bool, Guid> signal, bool? traceEnabled = null)
        {
            traceEnabled = traceEnabled ?? inPayload.TraceEnabled;
            //First clone the service message.
            var cloned = new TransmissionPayload(inPayload.Message.Clone(), release: signal, traceEnabled: traceEnabled.Value);

            cloned.TraceWrite("Cloned", "ManualCommunicationBridgeAgent/PayloadCopy");

            return cloned;
        } 
        #endregion

        #region ToResponse(this TransmissionPayload incoming)
        /// <summary>
        /// This helper method turns round an incoming payload request in to its corresponding response payload.
        /// </summary>
        /// <param name="incoming">The incoming payload.</param>
        /// <returns></returns>
        public static TransmissionPayload ToResponse(this TransmissionPayload incoming)
        {
            var m = incoming.Message;
            var rsMessage = m.ToResponse();

            rsMessage.ChannelId = m.ResponseChannelId;
            rsMessage.ChannelPriority = m.ResponseChannelPriority;
            rsMessage.MessageType = m.ResponseMessageType;
            rsMessage.ActionType = m.ResponseActionType;

            var outgoing = new TransmissionPayload(rsMessage, traceEnabled: incoming.TraceEnabled);

            if (incoming.TraceEnabled)
                outgoing.TraceWrite(new TransmissionPayloadTraceEventArgs(outgoing.TickCount, "Created from request", "ToResponse"));

            return outgoing;
        }
        #endregion
        #region CanRespond(this TransmissionPayload incoming)
        /// <summary>
        /// Determines whether the payload instance has a response channel set.
        /// </summary>
        public static bool CanRespond(this TransmissionPayload incoming)
        {
            var m = incoming.Message;

            return !string.IsNullOrEmpty(m.ResponseChannelId);
        }
        #endregion

        /// <summary>
        /// Transmissions the payload trace set.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <param name="eventArgs">The <see cref="TransmissionPayloadTraceEventArgs"/> instance containing the event data.</param>
        public static void TransmissionPayloadTraceWrite(this TaskTracker tracker, TransmissionPayloadTraceEventArgs eventArgs)
        {
            tracker.ToTransmissionPayload()?.TraceWrite(eventArgs);
        }

        /// <summary>
        /// Transmissions the payload trace set.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <param name="message">The message.</param>
        /// <param name="source">The optional source parameter.</param>
        public static void TransmissionPayloadTraceWrite(this TaskTracker tracker, string message, string source = null)
        {
            tracker.ToTransmissionPayload()?.TraceWrite(message, source);
        }

        /// <summary>
        /// Identifies whether the tracker context is a payload.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <returns>Return true if the context is a payload.</returns>
        public static bool IsTransmissionPayload(this TaskTracker tracker)
        {
            return tracker.Context is TransmissionPayload;
        }

        /// <summary>
        /// Converts the context to the transmission payload.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <returns>Returns the payload or null.</returns>
        public static TransmissionPayload ToTransmissionPayload(this TaskTracker tracker)
        {
            return tracker.Context as TransmissionPayload;
        }
    }
}
