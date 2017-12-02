#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the command container statistics.
    /// </summary>
    public class CommandContainerStatistics: StatusBase
    {
        /// <summary>
        /// The command list.
        /// </summary>
        public List<CommandStatistics> Commands { get; set; }
        /// <summary>
        /// The shared services.
        /// </summary>
        public SharedServiceStatistics SharedServices { get; set; }

   }
}
