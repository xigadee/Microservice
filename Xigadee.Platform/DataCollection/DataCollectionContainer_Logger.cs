#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
            mContainerLogger = new ActionQueueCollection<LogEvent, ILogger>(items, mPolicy.Logger, ActionQueueEventProcessLog);
            ServiceStart(mContainerLogger);
        }
        /// <summary>
        /// This method stops the logger.
        /// </summary>
        protected virtual void StopLogger()
        {
            ServiceStop(mContainerLogger);
            mContainerLogger = null;
            mLoggers.ForEach((c) => ServiceStop(c));
        }
        #endregion
        #region ActionQueueEventProcessLog(LogEvent l , ILogger e)
        /// <summary>
        /// This is the method executed by the ActionQueueCollection
        /// </summary>
        /// <param name="l">The log event</param>
        /// <param name="e">The logger.</param>
        private void ActionQueueEventProcessLog(LogEvent l, ILogger e)
        {
            e.Log(l);
        } 
        #endregion

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

        //Extended logging methods
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
