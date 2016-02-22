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
    public class KeyMapperResolutionException : Exception
    {
        public KeyMapperResolutionException(string message)
            : base(message)
        {
        }

        public KeyMapperResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
