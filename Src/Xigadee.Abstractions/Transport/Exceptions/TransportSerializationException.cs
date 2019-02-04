#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
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
