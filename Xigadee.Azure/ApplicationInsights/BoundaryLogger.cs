using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ApplicationInsightsBoundaryLogger: IBoundaryLogger
    {

        public Guid BatchPoll(int requested, int actual, string channelId)
        {
            return Guid.NewGuid();
        }

        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {
            //throw new NotImplementedException();
        }
    }
}
