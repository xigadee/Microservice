using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when the byte array is larger than the amount permitted. 
    /// This is used for messaging systems that have a specific limit.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SerializationBlobLimitExceeededException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationBlobLimitExceeededException"/> class.
        /// </summary>
        /// <param name="maxLength">The maximum byte array length.</param>
        /// <param name="length">The actual byte array length.</param>
        public SerializationBlobLimitExceeededException(int maxLength, int length)
            : base($"The byte array of length {length} has exceeded the permitted length of {maxLength}")
        {
        }
    }
}
