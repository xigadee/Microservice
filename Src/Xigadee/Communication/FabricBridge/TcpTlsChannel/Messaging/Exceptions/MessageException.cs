using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base exception for the Message class.
    /// </summary>
    public class MessageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MessageException() : base() { }
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public MessageException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public MessageException(string message, Exception ex) : base(message, ex) { }


    }
}
