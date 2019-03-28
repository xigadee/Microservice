#region using
using System;
#endregion
namespace Xigadee
{
    public class TransportDeserializationException : Exception
    {
        public TransportDeserializationException(string message)
            : base(message)
        {
        }

        public TransportDeserializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
