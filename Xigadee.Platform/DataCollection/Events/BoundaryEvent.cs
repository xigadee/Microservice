using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class BoundaryEvent: EventBase
    {
        public BoundaryEventType Type { get; set; }
        
        public ChannelDirection? Direction { get; set; }

        public TransmissionPayload Payload { get; set; }

        public Exception Ex { get; set; }

        public Guid? Id { get; set; }

        public int Requested { get; set; }

        public int Actual { get; set; }

        public string ChannelId { get; set; }
    }
}
