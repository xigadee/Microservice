using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown by the Command Harness. See the error message for a description of what caused the error.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class CommandHarnessException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarnessException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandHarnessException(string message):base (message)
        {

        }
    }
}
