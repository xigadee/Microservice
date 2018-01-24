using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    public static partial class AzureStorageHelper
    {

        /// <summary>
        /// This method serializes incoming objects in to standard JSON format encoded as UTF8.
        /// </summary>
        /// <param name="e">The incoming EventBase.</param>
        /// <param name="id">The Microservice Id</param>
        /// <returns>Returns the byte array.</returns>
        public static AzureStorageBinary DefaultJsonBinarySerializer(EventHolder e, MicroserviceId id)
        {
            var jObj = JObject.FromObject(e.Data);
            var body = jObj.ToString();
            return new AzureStorageBinary { Blob = Encoding.UTF8.GetBytes(body) };
        }

        //Statistics
        public static string StatisticsMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as Microservice.Statistics;
            return $"{e.StorageId}.json";
        }
        public static string StatisticsMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as Microservice.Statistics;
            return string.Format("{0}/{1:yyyy-MM-dd}/{1:HH}", msId.Name, DateTime.UtcNow);
        }
        //Logger
        public static string LoggerMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as LogEvent;           
            return $"{e.TraceId}.json";
        }
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
        public static string EventSourceMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as EventSourceEvent;
            return $"{string.Join("_", e.Entry.Key.Split(Path.GetInvalidFileNameChars()))}.json";
        }

        public static string EventSourceMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as EventSourceEvent;
            return $"{msId.Name}/{e.UtcTimeStamp:yyyy-MM-dd}/{e.Entry.EntityType}";
        }

        /// <summary>
        /// Make an Id for a boundary event
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="msId"></param>
        /// <returns></returns>
        public static string BoundaryMakeId(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as BoundaryEvent;
            string id = $"{e.Payload?.Message?.ProcessCorrelationKey ?? "NoCorrId"}_{e.Id}";
            return $"{string.Join("_", id.Split(Path.GetInvalidFileNameChars()))}.json";
        }

        /// <summary>
        /// Makes a folder for a boundary event
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="msId"></param>
        /// <returns></returns>
        public static string BoundaryMakeFolder(EventHolder ev, MicroserviceId msId)
        {
            var e = ev.Data as BoundaryEvent;
            return string.Format("{0}/{2:yyyy-MM-dd}/{2:HH}", msId.Name, DateTime.UtcNow);
        }
    }
}
