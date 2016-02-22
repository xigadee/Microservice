#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    [Serializable]
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
