#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the exception class for the trasmit processing that identifies whether a payload has looped/
    /// </summary>
    public class TransitCountExceededException : DispatcherException
    {
        /// <summary>
        /// Initializes a new instance of the TransitCountExceededException class.
        /// </summary>
        public TransitCountExceededException() : base() { }
        /// <summary>
        /// Initializes a new instance of the TransitCountExceededException class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        public TransitCountExceededException(TransmissionPayload payload) : base(payload) { }
        /// <summary>
        /// Initializes a new instance of the DispatcherException class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="ex">The base exception.</param>
        public TransitCountExceededException(TransmissionPayload payload, Exception ex) : base(payload, ex) { }


    }
}
