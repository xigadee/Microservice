﻿using System;
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class PayloadDeserializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PayloadDeserializationException() : base() { }
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public PayloadDeserializationException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public PayloadDeserializationException(string message, Exception ex) : base(message, ex) { }


    }
}
