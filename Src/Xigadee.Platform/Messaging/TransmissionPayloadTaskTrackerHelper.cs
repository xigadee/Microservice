namespace Xigadee
{
    public static class TransmissionPayloadTaskTrackerHelper
    {
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
        public static bool HasTransmissionPayload(this TaskTracker tracker)
        {
            return tracker.Context is TransmissionPayload;
        }

        /// <summary>
        /// Identifies whether the tracker context is a schedule.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <returns>Return true if the context is a schedule.</returns>
        public static bool HasSchedule(this TaskTracker tracker)
        {
            return tracker.Context is Schedule;
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


        /// <summary>
        /// Converts the context to a schedule.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <returns>Returns the schedule or null.</returns>
        public static Schedule ToSchedule(this TaskTracker tracker)
        {
            return tracker.Context as Schedule;
        }
    }

}
