using System;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    public partial class ServiceHandlerContainer
    {
        /// <summary>
        /// Gets the encryption collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerEncryption> Encryption { get; }

        #region Encrypt(EncryptionHandlerId handler, byte[] input)
        /// <summary>
        /// Encrypts the specified blob using the handler provided..
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="input">The input blob.</param>
        /// <returns>The encrypted output blob.</returns>
        public byte[] Encrypt(EncryptionHandlerId handler, byte[] input)
        {
            EncryptionValidate(handler);

            return Encryption[handler.Id].Encrypt(input);
        }
        #endregion
        #region Decrypt(EncryptionHandlerId handler, byte[] input)
        /// <summary>
        /// Decrypts the specified blob using the handler provided..
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="input">The input blob.</param>
        /// <returns>Returns the unencrypted blob.</returns>
        public byte[] Decrypt(EncryptionHandlerId handler, byte[] input)
        {
            EncryptionValidate(handler);

            return Encryption[handler.Id].Decrypt(input);
        }
        #endregion
        #region EncryptionValidate(EncryptionHandlerId handler, bool throwErrors = true)
        /// <summary>
        /// Validates that the required handler is supported.
        /// </summary>
        /// <param name="handler">The handler identifier.</param>
        /// <param name="throwErrors">if set to <c>true</c> [throw errors].</param>
        /// <returns>Returns true if the encryption handler is supported.</returns>
        /// <exception cref="EncryptionHandlerNotResolvedException">This exception is thrown if the handler is not present and throwErrors is set to true.</exception>
        public bool EncryptionValidate(EncryptionHandlerId handler, bool throwErrors = true)
        {
            if (!Encryption.Contains(handler.Id))
                if (throwErrors)
                    throw new EncryptionHandlerNotResolvedException(handler.Id);
                else
                    return false;

            return true;
        }
        #endregion

    }
}
