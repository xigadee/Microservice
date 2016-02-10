using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TraceEventLogger : ILogger
    {

        public async Task Log(LogEvent logEvent)
        {
            try
            {
                //string message = string.Format(, logEvent.Message);
                Trace.WriteLine(logEvent.Message);
            }
            catch (Exception)
            {

            }
        }
    }
}
