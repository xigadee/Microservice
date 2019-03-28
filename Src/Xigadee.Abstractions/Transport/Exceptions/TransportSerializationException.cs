using System;
namespace Xigadee
{
    public class TransportSerializationException : Exception
    {
        public TransportSerializationException(string message)
            : base(message)
        {
        }

        public TransportSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
