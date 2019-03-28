using System;
namespace Xigadee
{
    /// <summary>
    /// This exception is throw when the media type header cannot be resolved to the TransportSerializer.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class TransportSerializerResolutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransportSerializerResolutionException"/> class.
        /// </summary>
        /// <param name="message">The unresolved media type.</param>
        public TransportSerializerResolutionException(string message)
            : base(message)
        {
        }
    }
}
