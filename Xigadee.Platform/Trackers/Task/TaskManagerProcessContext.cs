using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class holders a registered process that will be polled as part of the thread loop.
    /// </summary>
    public class TaskManagerProcessContext
    {
        public TaskManagerProcessContext(string name)
        {
            Name = name;
        }
        /// <summary>
        /// The process priority.
        /// </summary>
        public int Ordinal { get; set; }
        /// <summary>
        /// The execute action.
        /// </summary>
        public ITaskManagerProcess Process { get; set; }
        /// <summary>
        /// The unique readonly process name.
        /// </summary>
        public string Name { get; }
    }

    public class TaskManagerProcessContext<C>: TaskManagerProcessContext
    {
        public TaskManagerProcessContext(string name) : base(name)
        {
            Context = default(C);
        }

        public C Context { get; set; }
    }
}
