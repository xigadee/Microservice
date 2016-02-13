#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to record the standby partner for a master job.
    /// </summary>
    public class StandbyPartner
    {
        /// <summary>
        /// The constructor is passed the partner service id.
        /// </summary>
        /// <param name="Id"></param>
        public StandbyPartner(string Id)
        {
            LastNotification = DateTime.UtcNow;
            ServiceId = Id;
        }
        /// <summary>
        /// The time of last notification.
        /// </summary>
        public readonly DateTime LastNotification;
        /// <summary>
        /// The microservice is.
        /// </summary>
        public readonly string ServiceId;
    }
}
