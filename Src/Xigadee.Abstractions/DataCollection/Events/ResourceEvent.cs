using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This event is used to signal resource based event types, such as a dependency slow down.
    /// </summary>
    public class ResourceEvent:EventBase
    {
        /// <summary>
        /// This is the type of resource event raised.
        /// </summary>
        public ResourceStatisticsEventType Type { get; set; }
        /// <summary>
        /// This is the for the resource event.
        /// </summary>
        public string Name { get; set; }
    }
}
