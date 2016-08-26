using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IMicroserviceChannel
    {
        ConfigurationPipeline Service { get; }

        Channel Channel { get; }
    }


    public interface IMicroserviceChannelIncoming: IMicroserviceChannel
    {

    }

    public interface IMicroserviceChannelOutgoing: IMicroserviceChannel
    {

    }

    public static class ChannelExtensionMethods
    {
        public static IMicroserviceChannelIncoming AddChannelIncoming(this ConfigurationPipeline service, string channelId)
        {
            return null;
        }

        public static IMicroserviceChannelOutgoing AddChannelOutgoing(this ConfigurationPipeline service, string channelId)
        {
            return null;
        }
    }
}
