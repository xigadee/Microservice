using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enumeration is used to specify the type of data collection supported by the component.
    /// </summary>
    [Flags]
    public enum DataCollectionSupport:int
    {
        Logger = 1,
        EventSource = 2,
        BoundaryLogger = 4,
        Telemetry = 8,
        All = 15
    }
}
