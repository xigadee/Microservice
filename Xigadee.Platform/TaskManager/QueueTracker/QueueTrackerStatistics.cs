using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class QueueTrackerStatistics: StatusBase
    {
        public List<MessagingStatistics> Queues { get; set; }
    }
}
