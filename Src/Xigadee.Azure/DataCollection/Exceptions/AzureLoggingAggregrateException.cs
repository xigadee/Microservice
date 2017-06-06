using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is used to contain multiple azure storage exceptions.
    /// </summary>
    public class AzureLoggingAggregrateException: AggregateException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public AzureLoggingAggregrateException(string message, IEnumerable<Exception> innerExceptions):base(message, innerExceptions)
        {

        }
    }
}
