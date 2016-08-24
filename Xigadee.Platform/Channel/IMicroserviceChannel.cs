using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IMicroserviceChannel
    {
        Microservice Service { get; set; }

        string ChannelId { get; set; }

        ChannelDirection Direction { get; }
    }


    public interface IMicroserviceChannelIncoming: IMicroserviceChannel
    {

    }

    public interface IMicroserviceChannelOutgoing: IMicroserviceChannel
    {

    }
}
