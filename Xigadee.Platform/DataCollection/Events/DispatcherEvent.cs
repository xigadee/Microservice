using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class DispatcherEvent: EventBase
    {
        public PayloadEventType Type { get; set; }

        public TransmissionPayload Payload { get; set; }

        public DispatcherRequestUnresolvedReason? Reason { get; set; }

        public Exception Ex { get; set; }

        public int Delta { get; set; }

        public bool IsSuccess { get; set; }
    }

    public enum PayloadEventType
    {
        Complete,
        Exception,
        Incoming,
        Unresolved
    }
}
