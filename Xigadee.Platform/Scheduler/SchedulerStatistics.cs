#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    public class SchedulerStatistics: CollectionStatistics
    {
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }

        public override int ItemCount
        {
            get
            {
                return base.ItemCount;
            }

            set
            {
                base.ItemCount = value;
            }
        }

        public int DefaultPollInMs { get; set; }

        public List<Schedule> Schedules { get; set; }
    }
}
