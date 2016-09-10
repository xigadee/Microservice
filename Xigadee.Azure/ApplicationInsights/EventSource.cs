using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ApplicationInsightsEventSource: IEventSource
    {
        public string Name
        {
            get
            {
                return "";
            }
        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            //throw new NotImplementedException();
        }
    }
}
