using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the boundary event base class. It is used to allow for other boundary event logging mechanisms to be used.
    /// </summary>
    public abstract class BoundaryEventBase : EventBase
    {
        public BoundaryEventType Type { get; set; }

        public ChannelDirection Direction { get; set; }

        public Guid? Id { get; set; }

        public Exception Ex { get; set; }
    }
}
