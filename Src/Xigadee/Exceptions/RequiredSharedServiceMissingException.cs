using System;
namespace Xigadee
{
    public class RequiredSharedServiceMissingException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the EventSourceLogException class.
        /// </summary>
        public RequiredSharedServiceMissingException() : base() { }
        /// <summary>
        /// Initializes a new instance of the EventSourceLogException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RequiredSharedServiceMissingException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the EventSourceLogException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public RequiredSharedServiceMissingException(string message, Exception ex) : base(message, ex) { }
    }
}


