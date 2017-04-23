using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holds a record of the event source changes. The number of records held is based on
    /// the parameters passed through the constructor.
    /// </summary>
    public class MemoryEventSource: IEventSource
    {
        public MemoryEventSource(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {

        }
    }
}
