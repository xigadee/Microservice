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

        public List<CommandStatistics> Commands { get; set; }

        public SharedServiceStatistics SharedServices { get; set; }

        public List<MessagingStatistics> Cache { get; set; }

   }
}
