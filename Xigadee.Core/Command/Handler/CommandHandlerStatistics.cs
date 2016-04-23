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
    /// This is the statistics class for the Handler.
    /// </summary>
    public class CommandHandlerStatistics: MessagingStatistics
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
        /// <summary>
        /// This is the time the handler was last accessed.
        /// </summary>
        public DateTime? LastAccessed { get; set; }
    }
}
