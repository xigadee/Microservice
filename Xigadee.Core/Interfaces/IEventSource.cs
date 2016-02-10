#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This interface is used to provide generic support for event sources for components
    /// </summary>
    public interface IEventSource
    {
        Task Write<K,E>(string originatorId, EventSourceEntry<K,E> entry, DateTime? utcTimeStamp = null, bool sync = false);
    }

    /// <summary>
    /// This class is used to log and event source entry.
    /// </summary>
    public class EventSourceEntry: EventSourceEntry<object, object>
    {

    }

    public class EventSourceEntry<K,E>: EventSourceEntryBase
    {
        private Func<K, string> mKeyMaker;

        public EventSourceEntry()
        {
            mKeyMaker = null;
        }
        public EventSourceEntry(Func<K, string> keyMaker = null)
        {
            mKeyMaker = keyMaker;
        }

        public K EntityKey { get; set; }

        public E Entity { get; set; }

        public override string Key
        {
            get
            {
                return mKeyMaker == null?EntityKey.ToString(): mKeyMaker(EntityKey);
            }
        }
    }

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
    }
}
