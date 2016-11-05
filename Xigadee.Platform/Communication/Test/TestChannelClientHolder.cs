using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class TestChannelClientHolder: ClientHolder<TestChannelConnection, TestChannelMessage>
    {
        public override void MessageComplete(TransmissionPayload payload)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            throw new NotImplementedException();
        }

        public override Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            throw new NotImplementedException();
        }
    }
}
