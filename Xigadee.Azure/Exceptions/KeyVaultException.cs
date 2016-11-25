using System;

namespace Xigadee
{
    public class KeyVaultException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the KeyVaultException class.
        /// </summary>
        public KeyVaultException() : base() { }
        /// <summary>
        /// Initializes a new instance of the KeyVaultException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public KeyVaultException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the KeyVaultException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public KeyVaultException(string message, Exception ex) : base(message, ex) { }
    }
}
