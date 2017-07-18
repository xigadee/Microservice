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
        /// <summary>
        /// This is the default constructo.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The optional exception.</param>
        public CommandMethodSignatureException(string message, Exception ex = null):base(message, ex)
        {

        }
    }
}
