using System;
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class StorageThrottlingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the StorageThrottlingExcepion class.
        /// </summary>
        public StorageThrottlingException() { }
        /// <summary>
        /// Initializes a new instance of the StorageThrottlingExcepion class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public StorageThrottlingException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the StorageThrottlingExcepion class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public StorageThrottlingException(string message, Exception ex) : base(message, ex) { }
    }
}
