using System;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the core message used for tracking outgoing messages.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class OutgoingRequest
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="payload">The incoming payload.</param>
        /// <param name="ttl">The time to live for the account.</param>
        /// <param name="start">The timestamp for start.</param>
        public OutgoingRequest(TransmissionPayload payload, TimeSpan ttl, int? start = null)
        {
            Id = payload.Message.OriginatorKey.ToUpperInvariant();
            Payload = payload;
            Start = start ?? Environment.TickCount;
            MaxTTL = ttl;

            ResponseMessage = new ServiceMessageHeader(
                  payload.Message.ResponseChannelId
                , payload.Message.ResponseMessageType
                , payload.Message.ResponseActionType);
        }

        /// <summary>
        /// This is the request Id.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// This incoming payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// This is the start timestamp.
        /// </summary>
        public int Start { get; }
        /// <summary>
        /// The maximum time to live for the message.
        /// </summary>
        public TimeSpan MaxTTL { get; }
        /// <summary>
        /// The current timespan since the message began.
        /// </summary>
        public TimeSpan Extent { get { return ExtentNow(); } }
        /// <summary>
        /// The extent since the submitted time or the current time if not set.
        /// </summary>
        /// <param name="now">The optional time to calculate the extent from.</param>
        /// <returns>The timespan.</returns>
        private TimeSpan ExtentNow(int? now = null)
        {
            return ConversionHelper.DeltaAsTimeSpan(Start, now ?? Environment.TickCount).Value;
        }
        /// <summary>
        /// Returns a boolean value indicating whether the message has expired.
        /// </summary>
        /// <param name="now">The time to Timestamp to check from.</param>
        /// <returns>Returns true if the message has expired.</returns>
        public bool HasExpired(int? now = null)
        {
            var extent = ExtentNow();
            return extent > MaxTTL;
        }
        /// <summary>
        /// The address to send the response message.
        /// </summary>
        public ServiceMessageHeader ResponseMessage { get; }
        /// <summary>
        /// The debug string.
        /// </summary>
        public string Debug
        {
            get
            {
                return $"{Id} TTL: {(MaxTTL - Extent).ToFriendlyString()} HasExpired: {(HasExpired() ? "Yes" : "No")}";
            }
        }


    }
}
