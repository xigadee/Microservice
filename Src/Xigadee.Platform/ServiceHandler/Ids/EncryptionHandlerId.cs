namespace Xigadee
{
    /// <summary>
    /// This class encapsulates the encryption handler id.
    /// </summary>
    public class EncryptionHandlerId: ServiceHandlerIdBase
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="id">The security id name.</param>
        public EncryptionHandlerId(string id):base(id)
        {
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
