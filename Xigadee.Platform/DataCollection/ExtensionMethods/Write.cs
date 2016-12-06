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
    public static partial class DataCollectionExtensionMethodsWrite
    {
        public static void Write(this IDataCollection collector, LogEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Logger, sync);
        }

        public static void Write(this IDataCollection collector, BoundaryEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.BoundaryLogger, sync);
        }

        public static void Write(this IDataCollection collector, EventSourceEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.EventSource, sync);
        }

        public static void Write(this IDataCollection collector, ResourceEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Resource, sync);
        }

        public static void Write(this IDataCollection collector, TelemetryEvent myEvent, bool sync = false)
        {
            collector.Write(myEvent, DataCollectionSupport.Telemetry, sync);
        }
    }
}
