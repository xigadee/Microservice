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
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    /// <summary>
    /// This helper provides support for convering entities in to their binary representation.
    /// </summary>
    public static partial class BinaryContainerHelper
    {

        /// <summary>
        /// This method serializes incoming objects in to standard JSON format encoded as UTF8.
        /// </summary>
        /// <param name="e">The incoming EventHolder class.</param>
        /// <param name="id">The Microservice identifier</param>
        /// <returns>Returns the byte array.</returns>
        public static BinaryContainer DefaultJsonBinarySerializer(EventHolder e, MicroserviceId id)
        {
            var jObj = JObject.FromObject(e.Data);
            var body = jObj.ToString();
            return new BinaryContainer { Blob = Encoding.UTF8.GetBytes(body) };
        }

        //Statistics        
        /// <summary>
        /// Creates the appropriate storage identifier for the event holder.
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>Returns a file name for the event.</returns>
        public static string StatisticsMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as Microservice.Statistics;
            return $"{e.StorageId}.json";
        }
        /// <summary>
        /// This static method makes a folder path for the entity.
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>Returns a folder path.</returns>
        public static string StatisticsMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as Microservice.Statistics;
            return string.Format("{0}/{1:yyyy-MM-dd}/{1:HH}", msId.Name, DateTime.UtcNow);
        }
        //Logger        
        /// <summary>
        /// Loggers the make identifier.
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns></returns>
        public static string LoggerMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as LogEvent;           
            return $"{e.TraceId}.json";
        }
        /// <summary>
        /// Returns a folder for the EventHolder.
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>Returns the folder as a string.</returns>
        public static string LoggerMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as LogEvent;
            string level = Enum.GetName(typeof(LoggingLevel), e.Level);
            return string.Format("{0}/{1}/{2:yyyy-MM-dd}/{2:HH}", msId.Name, level, DateTime.UtcNow);

            //if (e is ILogStoreName)
            //    return ((ILogStoreName)logEvent).StorageId;

            //// If there is a category specified and it contains valid digits or characters then make it part of the log name to make it easier to filter log events
            //if (!string.IsNullOrEmpty(logEvent.Category) && logEvent.Category.Any(char.IsLetterOrDigit))
            //    return string.Format("{0}_{1}_{2}", logEvent.GetType().Name, new string(logEvent.Category.Where(char.IsLetterOrDigit).ToArray()), Guid.NewGuid().ToString("N"));

            //return string.Format("{0}_{1}", logEvent.GetType().Name, Guid.NewGuid().ToString("N"));
        }

        //Event Source
        /// <summary>
        /// Creates an file identifier for the EventHolder.
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>A file name string.</returns>
        public static string EventSourceMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as EventSourceEvent;
            return $"{string.Join("_", e.Entry.Key.Split(Path.GetInvalidFileNameChars()))}.json";
        }
        /// <summary>
        /// Creates an folder for the EventHolder.
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>A folder string.</returns>
        public static string EventSourceMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as EventSourceEvent;
            return $"{msId.Name}/{e.UtcTimeStamp:yyyy-MM-dd}/{e.Entry.EntityType}";
        }

        /// <summary>
        /// Make an Id for a boundary event
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>Returns a file name.</returns>
        public static string BoundaryMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as BoundaryEvent;
            string id = $"{e.Payload?.Message?.ProcessCorrelationKey ?? "NoCorrId"}_{e.Id}";
            return $"{string.Join("_", id.Split(Path.GetInvalidFileNameChars()))}.json";
        }

        /// <summary>
        /// Makes a folder for a boundary event
        /// </summary>
        /// <param name="ev">The event holder.</param>
        /// <param name="msId">The Microservice identifier.</param>
        /// <returns>Returns a folder name.</returns>
        public static string BoundaryMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as BoundaryEvent;
            return string.Format("{0}/{2:yyyy-MM-dd}/{2:HH}", msId.Name, DateTime.UtcNow);
        }
    }
}
