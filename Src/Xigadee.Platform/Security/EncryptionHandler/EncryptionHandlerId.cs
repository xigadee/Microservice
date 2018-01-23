namespace Xigadee
{
    /// <summary>
    /// This class encapsulates the encryption handler id.
    /// </summary>
    public class EncryptionHandlerId: SecurityHandlerIdBase
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="id">The security id name.</param>
        public EncryptionHandlerId(string id):base(id)
        {
        }

        /// <summary>
        /// Implicitly converts a string in to a the encryption handler class.
        /// </summary>
        /// <param name="id">The name of the security id.</param>
        public static implicit operator EncryptionHandlerId(string id)
        {
            return new EncryptionHandlerId(id);
        }
    }
}
