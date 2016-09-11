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
}
