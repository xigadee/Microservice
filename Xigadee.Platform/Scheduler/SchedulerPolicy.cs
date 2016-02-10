using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class SchedulerPolicy:PolicyBase
    {
        public SchedulerPolicy()
        {
            DefaultPollInMs = 100;
        }

        public virtual int DefaultPollInMs { get; set; }
    }
}
