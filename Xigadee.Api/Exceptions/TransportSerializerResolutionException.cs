#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee.Api
{
    [Serializable]
    public class TransportSerializerResolutionException : Exception
    {
        public TransportSerializerResolutionException(string message)
            : base(message)
        {
        }

        public TransportSerializerResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
