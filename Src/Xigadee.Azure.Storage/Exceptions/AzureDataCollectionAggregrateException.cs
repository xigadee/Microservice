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
    public class AzureDataCollectionAggregrateException: AggregateException
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="message">the message.</param>
        /// <param name="innerExceptions">The inner exceptions.</param>
        public AzureDataCollectionAggregrateException(string message, IEnumerable<Exception> innerExceptions):base(message, innerExceptions)
        {

        }
    }
}
