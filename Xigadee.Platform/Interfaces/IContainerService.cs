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
    /// This interface is implemented by containers that contains services 
    /// inside them. This allows the contained services to be started and stopped independently.
    /// </summary>
    public interface IContainerService
    {
        /// <summary>
        /// The services to start and stop.
        /// </summary>
        IEnumerable<IService> Services { get; }
    }
}
