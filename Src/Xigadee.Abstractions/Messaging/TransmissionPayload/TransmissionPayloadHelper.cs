using System;

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
            //First clone the service message.
            var sm = inPayload.Message.Clone();

            var cloned = new TransmissionPayload(sm, release: signal, traceEnabled: traceEnabled ?? inPayload.TraceEnabled);

            cloned.TraceWrite($"Cloned from {inPayload.Id}", $"{nameof(TransmissionPayloadHelper)}/{nameof(Clone)}");
            inPayload.TraceWrite($"Cloned to {cloned.Id}", $"{nameof(TransmissionPayloadHelper)}/{nameof(Clone)}");

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
    }
}
