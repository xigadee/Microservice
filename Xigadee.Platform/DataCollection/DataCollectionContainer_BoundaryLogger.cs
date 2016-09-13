using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class DataCollectionContainer
    {

        public void Log(ChannelDirection direction, TransmissionPayload payload, Exception ex = null, Guid? batchId = default(Guid?))
        {

        }

        public Guid BatchPoll(int requested, int actual, string channelId)
        {
            var id = Guid.NewGuid();

            return id;
        }
    }
}
