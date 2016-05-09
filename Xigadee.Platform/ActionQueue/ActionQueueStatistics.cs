#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
#endregion
namespace Xigadee
{
    public class ActionQueueCollectionStatistics: MessagingStatistics, ICollectionStatistics
    {
        public int ItemCount { get; set; }

        public int QueueLength { get; set; }

        public bool Overloaded { get; set; }

        public int OverloadProcessCount { get; set; }

        public int? OverloadThreshold { get; set; }

        public long OverloadProcessHits { get; set; }


        public List<object> Components { get; set; }

    }

    public class ActionQueueStatistics: MessagingStatistics
    {

    }
}
