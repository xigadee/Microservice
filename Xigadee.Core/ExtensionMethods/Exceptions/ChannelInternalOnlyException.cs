using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ChannelInternalOnlyException: ChannelPartitionConfigBaseException
    {
        public ChannelInternalOnlyException(string channelId):base(channelId)
        {

        }
    }
}
