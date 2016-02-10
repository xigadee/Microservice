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
    public class CommandContainerStatistics: StatusBase
    {
        /// <summary>
        /// This is a list of the handlers active on the system and their status.
        /// </summary>
        public List<CommandHandlerStatistics> Handlers { get; set; }

        public List<StatusBase> Persistence { get; set; }

        public List<JobStatistics> Jobs { get; set; }

        public SharedServiceStatistics SharedServices { get; set; }

        public List<MessagingStatistics> Cache { get; set; }

        public List<StatusBase> PersistenceSharedService { get; set; }

        public List<StatusBase> MessageInitiators { get; set; }
    }
}
