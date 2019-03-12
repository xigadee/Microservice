using System;
namespace Xigadee
{
    /// <summary>
    /// This class is used to record the standby partner for a master job.
    /// </summary>
    public class MasterJobPartner
    {
        /// <summary>
        /// The constructor is passed the partner service id.
        /// </summary>
        /// <param name="Id">The remote service id.</param>
        /// <param name="isStandby">Specifies whether the remote partner is in standby mode.</param>
        public MasterJobPartner(string Id, bool isStandby = true)
        {
            LastNotification = DateTime.UtcNow;
            ServiceId = Id;
            IsStandby = isStandby;
        }
        /// <summary>
        /// The time of last notification.
        /// </summary>
        public DateTime LastNotification { get; }
        /// <summary>
        /// The microservice is.
        /// </summary>
        public string ServiceId { get; }
        /// <summary>
        /// Specifies whether this partner is in standby.
        /// </summary>
        public bool IsStandby { get; }
    }
}
