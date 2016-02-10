using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the action holder for event source.
    /// </summary>
    public class EventSourceActionHolder: IEventSource
    {
        Func<string, EventSourceEntryBase, DateTime?, Task> mActionHolder;

        public EventSourceActionHolder(Func<string, EventSourceEntryBase, DateTime?, Task> actionHolder)
        {
            mActionHolder = actionHolder;
        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            try
            {
                await mActionHolder(originatorId, entry, utcTimeStamp);
            }
            catch
            {

            }
        }
    }
}
