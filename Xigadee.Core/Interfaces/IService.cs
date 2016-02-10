#region using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the standard service implementation.
    /// </summary>
    public interface IService 
    {
        event EventHandler<StatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Starts the service and returns immediately.
        /// </summary>
        void Start();

        /// <summary>
        /// This method stops a service.
        /// </summary>
        void Stop();

        /// <summary>
        /// This method allows the statitics to be retrieved as a generic format.
        /// </summary>
        /// <returns></returns>
        StatusBase StatisticsGet();
    }

    /// <summary>
    /// This class holds the status change.
    /// </summary>
    public class StatusChangedEventArgs: EventArgs
    {
        public ServiceStatus StatusOld { get; set; }

        public ServiceStatus StatusNew { get; set; }
    }

}
