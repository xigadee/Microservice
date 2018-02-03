namespace Xigadee
{
    /// <summary>
    /// This class encapsulates the encryption handler id.
    /// </summary>
    public class EncryptionHandlerId: ServiceHandlerIdBase, ISecurityService
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="id">The security id name.</param>
        public EncryptionHandlerId(string id):base(id)
        {
        }

        public byte[] Decrypt(EncryptionHandlerId id, byte[] input)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encrypt(EncryptionHandlerId id, byte[] input)
        {
            throw new System.NotImplementedException();
        }

        public bool EncryptionValidate(EncryptionHandlerId id, bool throwErrors = true)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Processes the incoming identifier in to a standard format..
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The processed identifier.
        /// </returns>
        public override string ProcessIdentifier(string id)
        {
            return base.ProcessIdentifier(id.Trim().ToLowerInvariant());
        }

        public void Secure(Channel channel, TransmissionPayload payloadOut)
        {
            throw new System.NotImplementedException();
        }

        public void Verify(Channel channel, TransmissionPayload payloadIn)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Implicitly converts a string in to a the encryption handler class.
        /// </summary>
        /// <param name="id">The name of the security id.</param>
        public static implicit operator EncryptionHandlerId(string id)
        {
            return new EncryptionHandlerId(id);
        }


        /// <summary>
        /// Implicitly converts a handler id to a string.
        /// </summary>
        /// <param name="handlerId">The handler id.</param>
        public static implicit operator string(EncryptionHandlerId handlerId)
        {
            return handlerId.Id;
        }
    }
}
