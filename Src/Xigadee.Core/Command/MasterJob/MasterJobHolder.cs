#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    public class MasterJobHolder
    {
        public MasterJobHolder(string name
            , Schedule Schedule
            , Func<Schedule, Task> Action
            , Action<Schedule> Initialise = null
            , Action<Schedule> Cleanup = null
            )
        {
            Name = name;
            this.Schedule = Schedule;
            this.Action = Action;
            this.Initialise = Initialise;
            this.Cleanup = Cleanup;
        }

        public string Name { get; }

        public Schedule Schedule { get; set; }

        public Func<Schedule, Task> Action { get; set; }

        public Action<Schedule> Initialise { get; set; }

        public Action<Schedule> Cleanup { get; set; }
    }
}
