using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the conceptual base class for event and information logging.
    /// </summary>
    public abstract class EventBase
    {
        /// <summary>
        /// This is the unique id used to identify the EventBase object.
        /// </summary>
        public string TraceId { get; } = Guid.NewGuid().ToString("N").ToUpperInvariant();
    }
}
