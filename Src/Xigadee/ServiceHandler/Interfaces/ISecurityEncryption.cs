namespace Xigadee
{
    /// <summary>
    /// THis interface is implemented by components that provide encryption services.
    /// </summary>
    public interface ISecurityEncryption
    {
        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="id">The encryption handler identifier.</param>
        /// <param name="input">The byte input.</param>
        /// <returns>Returns the encrypted byte input.</returns>
        byte[] Encrypt(EncryptionHandlerId id, byte[] input);
        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="id">The encryption handler identifier.</param>
        /// <param name="input">The byte input.</param>
        /// <returns>Returns the decrypted byte input.</returns>
        byte[] Decrypt(EncryptionHandlerId id, byte[] input);
        /// <summary>
        /// Validates that the required handler is supported.
        /// </summary>
        /// <param name="id">The encryption handler identifier.</param>
        /// <param name="throwErrors">if set to <c>true</c> [throw errors].</param>
        /// <returns>Returns true if the encryption handler is supported.</returns>
        /// <exception cref="EncryptionHandlerNotResolvedException">This exception is thrown if the handler is not present and throwErrors is set to true.</exception>
        bool EncryptionValidate(EncryptionHandlerId id, bool throwErrors = true);
    }
}
