using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when the DefaultCreator function is unable to create the Command object due to the constructor not being supported.
    /// You either need to have an empty constructor, or all the parameters must be optional.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class CommandHarnessInvalidConstructorException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarnessInvalidConstructorException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandHarnessInvalidConstructorException(string message):base (message)
        {

        }
    }
}
