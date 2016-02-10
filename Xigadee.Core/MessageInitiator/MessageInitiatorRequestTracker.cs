using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the request while it is being processed by the remote party.
    /// </summary>
    public class MessageInitiatorRequestTracker : RequestTracker
    {
        public MessageInitiatorRequestTracker():base()
        {
            Tcs = new TaskCompletionSource<TransmissionPayload>();
        }

        public TaskCompletionSource<TransmissionPayload> Tcs { get; set; }

    }
}
