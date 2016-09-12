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

#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This internal collection contains the Microservice loggers.
    /// </summary>
    public class LoggerContainer : ActionQueueCollectionBase<LogEvent, ILogger, LoggerContainerStatistics, LoggerPolicy>
        , ILoggerExtended
    {
        public LoggerContainer(IEnumerable<ILogger> components, LoggerPolicy policy=null) : base(components, policy)
        {

        }

        protected override void StatisticsRecalculate(LoggerContainerStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            //stats.
        }

        protected override void Process(LogEvent data, ILogger logger)
        {
            logger.Log(data);
        }

        protected string FormatMessageData(DispatcherLoggerDirection direction, ServiceMessage message)
        {
            return string.Format(@"{0}:{1}/{2}/{3} @[{4}Z-{5}] [ID-{6}<-CID-{7}] Status:({8})-{9} [O={10}] ",
                direction, //0
                message.ChannelId, message.MessageType, message.ActionType, //1,2,3
                DateTime.UtcNow, message.OriginatorUTC, //4,5
                message.OriginatorKey, message.CorrelationKey, //6,7
                message.OriginatorServiceId, //8
                message.Status, message.StatusDescription //9,10
                );
        }

        #region LogMessageException(DispatcherLoggerDirection direction, ServiceMessage message, Exception ex)
        /// <summary>
        /// This method logs the appropriate exception with the underlying loggers.
        /// </summary>
        /// <param name="payloadRq"></param>
        /// <param name="ex">The exception raised.</param>
        public void LogMessageException(TransmissionPayload payload, Exception ex)
        {
            LogException(FormatMessageData(DispatcherLoggerDirection.Incoming, payload.Message), ex);
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

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method ensures that status updates are written out immediately and
        /// not at the end of a long queue during busy times.
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        public async Task Log(LogEvent logEvent)
        {
            if (logEvent.Level == LoggingLevel.Status)
                WriteEvent(logEvent);
            else
                Enqueue(logEvent);
        }
        #endregion

    }
}
