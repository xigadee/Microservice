using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class CommandOutgoingRequestTracker:RequestTracker
    {
        public CommandOutgoingRequestTracker():base()
        {
            Tcs = new TaskCompletionSource<TransmissionPayload>();
        }

        public TaskCompletionSource<TransmissionPayload> Tcs { get; set; }
    }
}
