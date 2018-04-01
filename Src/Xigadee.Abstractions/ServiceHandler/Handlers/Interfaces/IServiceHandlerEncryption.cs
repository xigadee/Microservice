namespace Xigadee
{
    /// <summary>
    /// Provide symmetric i.e. Encrypt / Decrypt Functionality
    /// </summary>
    public interface IServiceHandlerEncryption: IServiceHandler
    {
        /// <summary>
        /// Encrypt a byte array
        /// </summary>
        /// <param name="input">The byte array to encrypt.</param>
        /// <returns>Returns the encrypted byte array.</returns>
        byte[] Encrypt(byte[] input);

        /// <summary>
        /// Decrypt a byte array
        /// </summary>
        /// <param name="input">The byte array to decrypt.</param>
        /// <returns>Returns the decrypted byte array.</returns>
        byte[] Decrypt(byte[] input);
    }
}
