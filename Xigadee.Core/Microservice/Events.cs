using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ProcessRequestUnresolvedEventArgs: MicroserviceEventArgs
    {
        public TransmissionPayload Payload { get; set; }
    }

    public class ProcessRequestErrorEventArgs: ProcessRequestUnresolvedEventArgs
    {
        public Exception Ex { get; set; }
    }

    /// <summary>
    /// This is the base event for microservice events.
    /// </summary>
    public class MicroserviceEventArgs: EventArgs
    {

    }
}
