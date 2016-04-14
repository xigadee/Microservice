using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TaskAvailabilityStatistics:StatusBase
    {
        public int TasksMaxConcurrent { get; set; }

        public int SlotsAvailable { get; set; }

        public string[] Levels { get; set; }

        public int Killed { get; set; }

        public long KilledDidReturn { get; set; }

        public int Active { get; set; }


        public string[] Running { get; set; }
    }
}
