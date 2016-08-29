using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ChannelIdMismatchException: ChannelPartitionConfigBaseException
    {
        public ChannelIdMismatchException(string channelId, string mismatchId) : base(channelId)
        {
            MismatchChannelId = mismatchId;
        }

        public string MismatchChannelId { get; }
    }
}
