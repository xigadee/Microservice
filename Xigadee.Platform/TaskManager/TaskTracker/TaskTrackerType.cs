#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the supported tracker types for the Microservice.
    /// </summary>
    public enum TaskTrackerType
    {
        Notset,
        ListenerPoll,
        Payload,
        Schedule,
        Overload
    }
}
