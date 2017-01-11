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
        public static async Task Log(this IDataCollection collector, LogEvent logEvent)
        {
            collector.Write(logEvent);
        }

        public static void LogException(this IDataCollection collector, Exception ex)
        {
            collector.Write(new LogEvent(ex) { Level = LoggingLevel.Error });
        }

        public static void LogException(this IDataCollection collector, string message, Exception ex)
        {
            collector.Write(new LogEvent(message, ex) { Level = LoggingLevel.Error });
        }

        public static void LogMessage(this IDataCollection collector, string message)
        {
            collector.Write(new LogEvent(message) { Level = LoggingLevel.Info });
        }

        public static void LogMessage(this IDataCollection collector, LoggingLevel level, string message)
        {
            collector.Write(new LogEvent(level, message));
        }

        public static void LogMessage(this IDataCollection collector, LoggingLevel level, string message, string category)
        {
            collector.Write(new LogEvent(level, message, category));
        }
    }
}
