#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This method holds the action for the logger.
    /// </summary>
    public class LoggerActionHolder : ILogger
    {
        Func<LogEvent, Task> mLogAction;

        public LoggerActionHolder(Func<LogEvent,Task> logAction)
        {
            mLogAction = logAction;
        }

        public async Task Log(LogEvent logEvent)
        {
            try
            {
                await mLogAction(logEvent);
            }
            catch
            {
            }
        }
    }
}
