using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when the resolver is unable to resolve a token.
    /// </summary>
    public class ConfigKeyVaultException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the KeyVaultException class.
        /// </summary>
        public ConfigKeyVaultException() : base() { }
        /// <summary>
        /// Initializes a new instance of the KeyVaultException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ConfigKeyVaultException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the KeyVaultException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public ConfigKeyVaultException(string message, Exception ex) : base(message, ex) { }
    }
}
