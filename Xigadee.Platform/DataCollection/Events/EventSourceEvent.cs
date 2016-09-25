using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class EventSourceEvent
    {
        public string OriginatorId { get; set; }
        public EventSourceEntryBase Entry { get; set; }
        public DateTime? UtcTimeStamp { get; set; }
        public bool Sync { get; set; }
    }
}
