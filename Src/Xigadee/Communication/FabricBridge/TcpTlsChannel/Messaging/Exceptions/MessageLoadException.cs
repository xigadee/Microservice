using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown should the message exceed the maximum permitted size.
    /// </summary>
    public class MessageLoadException : MessageException
    {
        /// <summary>
        /// Initializes a new instance of the MessageLoadException class.
        /// </summary>
        public MessageLoadException() : base() { }
        /// <summary>
        /// Initializes a new instance of the MessageLoadException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MessageLoadException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the MessageLoadException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public MessageLoadException(string message, Exception ex) : base(message, ex) { }

    }
}
