using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class ChannelPartitionConfigBaseException:Exception
    {
        public ChannelPartitionConfigBaseException(string channelId)
        {
            ChannelId = channelId;
        }

        public string ChannelId { get; }

    }
}
