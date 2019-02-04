namespace Xigadee
{
    /// <summary>
    /// This interface is used to send messages directly to the Microservice for processing.
    /// </summary>
    public interface IMicroserviceDispatch
    {
        /// <summary>
        /// This method injects a payload in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="payload">The transmission payload to execute.</param>
        void Process(TransmissionPayload payload);

        /// <summary>
        /// Gets the service handler container.
        /// </summary>
        IServiceHandlers ServiceHandlers { get; }
    }
}
