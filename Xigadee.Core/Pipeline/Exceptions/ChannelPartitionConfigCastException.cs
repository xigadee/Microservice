using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ChannelPartitionConfigCastException: ChannelPartitionConfigBaseException
    {
        public ChannelPartitionConfigCastException(string channelId):base(channelId)
        {

        }
    }
}
