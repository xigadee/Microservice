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
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethods
    {
        /// <summary>
        /// This method can be used to log an event object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="logEvent">The log event.</param>
        public static async Task Log(this IDataCollection collector, LogEvent logEvent)
        {
            collector.Write(logEvent);
        }
        /// <summary>
        /// This method can be used to log an exception to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="ex">The exception to be logged.</param>
        public static void LogException(this IDataCollection collector, Exception ex)
        {
            collector.Write(new LogEvent(ex) { Level = LoggingLevel.Error });
        }
        /// <summary>
        /// This method can be used to log an exception to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="ex">The exception to be logged.</param>
        public static void LogException(this IDataCollection collector, string message, Exception ex)
        {
            collector.Write(new LogEvent(message, ex) { Level = LoggingLevel.Error });
        }
        /// <summary>
        /// This extension message can be used to log a message direct to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="message">The message.</param>
        public static void LogMessage(this IDataCollection collector, string message)
        {
            collector.Write(new LogEvent(message) { Level = LoggingLevel.Info });
        }
        /// <summary>
        /// This extension message can be used to log a message direct to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        public static void LogMessage(this IDataCollection collector, LoggingLevel level, string message)
        {
            collector.Write(new LogEvent(level, message));
        }
        /// <summary>
        /// This extension message can be used to log a message direct to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="level">The logging level.</param>
        /// <param name="message">The message.</param>
        /// <param name="category">The message category.</param>
        public static void LogMessage(this IDataCollection collector, LoggingLevel level, string message, string category)
        {
            collector.Write(new LogEvent(level, message, category));
        }
    }
}
