using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base abstract class for command based exceptions.
    /// </summary>
    public abstract class CommandException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        protected CommandException() : base() { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected CommandException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the XimuraException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The base exception.</param>
        protected CommandException(string message, Exception ex) : base(message, ex) { }
    }
}
