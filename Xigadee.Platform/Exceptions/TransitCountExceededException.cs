#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base exception class for the CSV enumerator.
    /// </summary>
    public class TransitCountExceededException : TransmissionPayloadException
    {
        /// <summary>
        /// Initializes a new instance of the TransitCountExceededException class.
        /// </summary>
        public TransitCountExceededException() : base() { }
        /// <summary>
        /// Initializes a new instance of the TransitCountExceededException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransitCountExceededException(TransmissionPayload payload) : base(payload) { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        public TransitCountExceededException(TransmissionPayload payload, Exception ex) : base(payload, ex) { }


    }
}
