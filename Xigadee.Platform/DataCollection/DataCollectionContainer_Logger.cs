using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class DataCollectionContainer
    {
        #region Declarations
        /// <summary>
        /// This collection holds the loggers for the Microservice.
        /// </summary>
        protected ActionQueueCollection<LogEvent, ILogger> mContainerLogger;
        #endregion
        #region Start/Stop
        /// <summary>
        /// This method starts the telemetry.
        /// </summary>
        protected virtual void StartLogger()
        {
            mLoggers.ForEach((c) => ServiceStart(c));
            var items = mCollectors.Where((c) => c.IsSupported(DataCollectionSupport.Logger)).Cast<ILogger>().Union(mLoggers).ToList();
            mContainerLogger = new ActionQueueCollection<LogEvent, ILogger>(items, mPolicy.Logger, EventProcessLog);
            mContainerLogger.Start();
        }
        /// <summary>
        /// This method stops the logger.
        /// </summary>
        protected virtual void StopLogger()
        {
            mContainerLogger.Stop();
            mContainerLogger = null;
            mLoggers.ForEach((c) => ServiceStop(c));
        }
        #endregion

        private void EventProcessLog(LogEvent l , ILogger e)
        {
            e.Log(l);
        }

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method logs the event. 
        /// </summary>
        /// <param name="logEvent">The incoming event</param>
        public async Task Log(LogEvent logEvent)
        {
            mContainerLogger.EventSubmit(logEvent, logEvent.Level != LoggingLevel.Status);
        } 
        #endregion

        #region LogException...
        public void LogException(Exception ex)
        {
            Log(new LogEvent(ex));
        }

        public void LogException(string message, Exception ex)
        {
            Log(new LogEvent(message, ex));
        }
        #endregion
        #region LogMessage...
        public void LogMessage(string message)
        {
            Log(new LogEvent(message));
        }

        public void LogMessage(LoggingLevel logLevel, string message)
        {
            Log(new LogEvent(logLevel, message));
        }

        public void LogMessage(LoggingLevel logLevel, string message, string category)
        {
            Log(new LogEvent(logLevel, message, category));
        }
        #endregion

    }
}
