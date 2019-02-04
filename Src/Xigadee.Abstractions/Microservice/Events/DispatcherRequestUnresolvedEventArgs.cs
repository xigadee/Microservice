namespace Xigadee
{
    /// <summary>
    /// This event argument class is used to notify when a message cannot be resolved.
    /// </summary>
    /// <seealso cref="Xigadee.MicroserviceEventArgs" />
    public class DispatcherRequestUnresolvedEventArgs: MicroserviceEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherRequestUnresolvedEventArgs"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="reason">The failure reason.</param>
        public DispatcherRequestUnresolvedEventArgs(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason, DispatcherUnhandledAction policy)
        {
            Payload = payload;
            Reason = reason;
            Policy = policy;
        }
        /// <summary>
        /// The message payload.
        /// </summary>
        public TransmissionPayload Payload { get; }
        /// <summary>
        /// THe failure reason.
        /// </summary>
        public DispatcherRequestUnresolvedReason Reason { get; }
        /// <summary>
        /// This is the policy. You can change this in the event if you so wish.
        /// </summary>
        public DispatcherUnhandledAction Policy { get; set; }
    }

    /// <summary>
    /// This enumeration specifies the reason a message cannot be processed.
    /// </summary>
    public enum DispatcherRequestUnresolvedReason
    {
        /// <summary>
        /// The message handler not found, and the message was for internal processing only.
        /// </summary>
        MessageHandlerNotFound,
        /// <summary>
        /// The message could be processed externally but the outgoing channel could not be resolved.
        /// </summary>
        ChannelOutgoingNotFound
    }
}
