using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when the schedule recalculation fails.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ScheduleRecalculateException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleRecalculateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerEx">The inner ex.</param>
        public ScheduleRecalculateException(string message, Exception innerEx):base(message, innerEx)
        {

        }
    }
}
