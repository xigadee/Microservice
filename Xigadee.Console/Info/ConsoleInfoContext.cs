using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This context holds the logging information for the console.
    /// </summary>
    public class ConsoleInfoContext
    {
        /// <summary>
        /// This is the list of info messages.
        /// </summary>
        public ConcurrentBag<ErrorInfo> InfoMessages { get; } = new ConcurrentBag<ErrorInfo>();

        public bool InfoDecrement()
        {
            if (InfoCurrent == 0)
                return false;

            InfoCurrent--;

            return true;
        }

        public bool InfoIncrement()
        {
            if (InfoCurrent == InfoMax - 1)
                return false;

            InfoCurrent++;

            return true;
        }

        public int InfoCurrent { get; set; }

        public int InfoMax
        {
            get
            {
                return InfoMessages.Count;
            }
        }

        public bool Refresh { get; set; }
    }
}
