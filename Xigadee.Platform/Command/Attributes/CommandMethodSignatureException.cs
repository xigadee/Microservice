using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is throw during the validation of a command method signature.
    /// </summary>
    public class CommandMethodSignatureException:Exception
    {
        public CommandMethodSignatureException(string message):base(message)
        {

        }
    }
}
