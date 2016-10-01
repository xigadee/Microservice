using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    [DebuggerDisplay("{Entry.EntityType}/{Entry.EventType} [{Entry.Key}/{Entry.EntityVersion}] @ {OriginatorId}")]
    public class EventSourceEvent: EventBase
    {
        public string OriginatorId { get; set; }

        public EventSourceEntryBase Entry { get; set; }

        public DateTime? UtcTimeStamp { get; set; }
    }
}
