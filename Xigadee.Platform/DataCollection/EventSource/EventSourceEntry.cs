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
    /// This class is used to log and event source entry.
    /// </summary>
    public class EventSourceEntry: EventSourceEntry<object, object>
    {

    }

    public class EventSourceEntry<K, E>: EventSourceEntryBase
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
                return mKeyMaker == null ? EntityKey.ToString() : mKeyMaker(EntityKey);
            }
        }
    }
}
