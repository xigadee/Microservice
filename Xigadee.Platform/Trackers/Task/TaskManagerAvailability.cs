#region using
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a transient class that shows the current task slot availability.
    /// </summary>
    public class TaskManagerAvailability
    {
        public TaskManagerAvailability()
        {
        }

        public int Level(int priority)
        {
            return 0;
        }
    }
}
