using System;
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class.
    /// </summary>
    public class EventSourceLogException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the EventSourceLogException class.
        /// </summary>
        public EventSourceLogException() : base() { }
        /// <summary>
        /// Initializes a new instance of the EventSourceLogException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public EventSourceLogException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the EventSourceLogException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public EventSourceLogException(string message, Exception ex) : base(message, ex) { }
    }
}
