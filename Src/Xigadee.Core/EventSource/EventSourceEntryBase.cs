#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public abstract class EventSourceEntryBase
    {
        public EventSourceEntryBase()
        {
            UTCTimeStamp = DateTime.UtcNow;
        }

        public DateTime UTCTimeStamp { get; set; }

        public string BatchId { get; set; }

        public string CorrelationId { get; set; }

        public string EventType { get; set; }

        public string EntityType { get; set; }

        public string EntityVersion { get; set; }

        public string EntityVersionOld { get; set; }

        public abstract string Key { get; }

        public string EntitySource { get; set; }

        public string EntitySourceId { get; set; }

        public string EntitySourceName { get; set; }
    }

}
