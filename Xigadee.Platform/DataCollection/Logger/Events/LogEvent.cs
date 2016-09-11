using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the root class for logging events for the Microservice framework.
    /// </summary>
    public class LogEvent
    {
        #region Constructors
        protected LogEvent() { }

        public LogEvent(Exception ex) : this(LoggingLevel.Error, null, null, ex)
        {
        }
        public LogEvent(string message, Exception ex) : this(LoggingLevel.Error, message, null, ex)
        {

        }
        public LogEvent(string message) : this(LoggingLevel.Info, message, null, null)
        {

        }
        public LogEvent(LoggingLevel level, string message) : this(level, message, null, null)
        {

        }
        public LogEvent(LoggingLevel level, string message, string category) : this(level, message, category, null)
        {

        }

        public LogEvent(LoggingLevel level, string message, string category, Exception ex)
        {
            Level = level;
            Message = message;
            Category = category;
            Ex = ex;

            AdditionalData = new Dictionary<string, string>();
        }
        #endregion

        public virtual LoggingLevel Level { get; set; }

        public virtual string Message { get; set; }

        public virtual string Category { get; set; }

        public virtual Exception Ex { get; set; }

        public virtual Dictionary<string, string> AdditionalData { get; private set; }
    }
}
