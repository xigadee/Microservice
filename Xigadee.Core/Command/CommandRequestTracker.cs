using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class CommandRequestTracker:RequestTracker
    {
        public CommandRequestTracker():base()
        {
            Tcs = new TaskCompletionSource<TransmissionPayload>();
        }

        public TaskCompletionSource<TransmissionPayload> Tcs { get; set; }
    }
}
