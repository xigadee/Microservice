namespace Xigadee
{
    /// <summary>
    /// These method provide validates and decryption/encryption support when supported for incoming and outgoing messages.
    /// </summary>
    public interface ISecurityCommunication
    {
        /// <summary>
        /// This method verifies the security for the incoming message based on the channel it arrived on.
        /// </summary>
        /// <param name="channel">The channel the payload originated on.</param>
        /// <param name="payloadIn">The payload in.</param>
        void Verify(Channel channel, TransmissionPayload payloadIn);
        /// <summary>
        /// This method secures an outgoing message using the security defined for the channel.
        /// </summary>
        /// <param name="channel">The outgoing channel.</param>
        /// <param name="payloadOut">The outgoing payload.</param>
        void Secure(Channel channel, TransmissionPayload payloadOut);
    }
}
