using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ChannelIdMismatchException: ChannelPartitionConfigBaseException
    {
        public ChannelIdMismatchException(string channelId, ChannelDirection direction, string mismatchId) : base(channelId)
        {
            MismatchChannelId = mismatchId;
            Direction = direction;
        }

        public string MismatchChannelId { get; }

        public ChannelDirection Direction { get; }
    }
}
