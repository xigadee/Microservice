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
    /// <summary>
    /// This policy is primarily concerned with defining what happens when the ActionQueue becomes overloaded.
    /// </summary>
    public class ActionQueuePolicy: PolicyBase
    {
        /// <summary>
        /// This is the maximum time that an overload process should run.
        /// </summary>
        public int OverloadProcessTimeInMs { get; set; } = 10000; //10s
        /// <summary>
        /// This is the maximum number of overload tasks that should be run concurrently.
        /// </summary>
        public int OverloadMaxTasks { get; set; } = 2;
        /// <summary>
        /// This is the threshold at which point the overload tasks will be triggered.
        /// </summary>
        public int? OverloadThreshold { get; set; } = 1000;

        /// <summary>
        /// This is the name used for debugging.
        /// </summary>
        public virtual string Name
        {
            get;set;
        }
    }
}
