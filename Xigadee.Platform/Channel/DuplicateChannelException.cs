using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class DuplicateChannelException:Exception
    {
        public DuplicateChannelException(string channelId, ChannelDirection direction):base(channelId)
        {
            Direction = direction;
        }

        public ChannelDirection Direction { get; }
    }
}
