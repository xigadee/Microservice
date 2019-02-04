using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethods
    {
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The log event.</param>
        /// <param name="sync">Specifies whether this method should be written synchronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, LogEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Logger, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The boundary event.</param>
        /// <param name="sync">Specifies whether this method should be written synchronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, BoundaryEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Boundary, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The event source.</param>
        /// <param name="sync">Specifies whether this method should be written synchronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, EventSourceEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.EventSource, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The resource event.</param>
        /// <param name="sync">Specifies whether this method should be written synchronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, ResourceEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Resource, sync);
        }
        /// <summary>
        /// This extension method writes the raw object to the data collector.
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="myEvent">The telemetry event.</param>
        /// <param name="sync">Specifies whether this method should be written synchronously or on to the async queue for offline processing.</param>
        public static void Write(this IDataCollection collector, TelemetryEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Telemetry, sync);
        }
    }
}
