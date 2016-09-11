using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IChannelService
    {
        IEnumerable<Channel> Channels { get; }

        void Add(Channel item);

        bool Remove(Channel item);

        bool Exists(string channelId, ChannelDirection direction);

        bool TryGet(string channelId, ChannelDirection direction, out Channel channel);

    }

    public interface IRequireChannelService
    {
        IChannelService ChannelService { get; set; }
    }
}
