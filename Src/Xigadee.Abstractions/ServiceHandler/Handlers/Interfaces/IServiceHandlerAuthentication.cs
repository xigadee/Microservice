namespace Xigadee
{
    /// <summary>
    /// This interface is used by classes that provide authentication for incoming and outgoing messages.
    /// </summary>
    public interface IServiceHandlerAuthentication: IServiceHandler
        , IRequireServiceOriginator, IRequireDataCollector
    {
        /// <summary>
        /// This method signs the outgoing payload.
        /// </summary>
        /// <param name="payload">The payload to sign.</param>
        void Sign(TransmissionPayload payload);
        /// <summary>
        /// This method verifies the incoming payload and sets the ClaimsPrincipal on the payload.
        /// </summary>
        /// <param name="payload">The payload to verify.</param>
        void Verify(TransmissionPayload payload);
    }
}
