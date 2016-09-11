using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ChannelPartitionConfigExistsException: ChannelPartitionConfigBaseException
    {
        public ChannelPartitionConfigExistsException(string channelId, int channelPriority) : base(channelId)
        {
            ChannelPriority = channelPriority;
        }

        public int ChannelPriority { get; }
    }
}
