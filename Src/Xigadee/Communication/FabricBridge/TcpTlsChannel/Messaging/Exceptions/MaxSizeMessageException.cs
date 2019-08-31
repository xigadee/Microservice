using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown should the message exceed the maximum permitted size.
    /// </summary>
    public class MaxSizeMessageException : MessageException
    {
        /// <summary>
        /// Initializes a new instance of the MaxSizeMessageException class.
        /// </summary>
        public MaxSizeMessageException() : base() { }
        /// <summary>
        /// Initializes a new instance of the MaxSizeMessageException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MaxSizeMessageException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the MaxSizeMessageException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public MaxSizeMessageException(string message, Exception ex) : base(message, ex) { }

    }
}
