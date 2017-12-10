using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// Exception is throw when the holder ContentType does not match the expected value.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SerializationHolderContentTypeException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationHolderContentTypeException"/> class.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <param name="expectedType">The expected content type.</param>
        public SerializationHolderContentTypeException(SerializationHolder holder, string expectedType)
        {
            ContentTypeExpected = expectedType;
            ContentTypeActual = holder?.ContentType;
        }
        /// <summary>
        /// Gets the expected content type.
        /// </summary>
        public string ContentTypeExpected { get; }
        /// <summary>
        /// Gets the actual content type.
        /// </summary>
        public string ContentTypeActual { get; }
    }
}
