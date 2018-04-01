using System;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This enumeration provides additional information to the dispatcher on how to process the incoming
    /// payload request.
    /// </summary>
    [Flags]
    public enum ProcessOptions: int
    {
        RouteInternal = 1,
        RouteExternal = 2
    }
}
