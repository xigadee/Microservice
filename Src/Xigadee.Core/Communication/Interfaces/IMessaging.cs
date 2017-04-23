using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IMessaging
    {
        /// <summary>
        /// This is a list of clients for the listener or sender.
        /// </summary>
        IEnumerable<ClientHolder> Clients { get; }

        /// <summary>
        /// This is the channel for the messaging agent.
        /// </summary>
        string ChannelId { get; }

        /// <summary>
        /// This is the boundary logger used by the service.
        /// </summary>
        IBoundaryLogger BoundaryLogger { get; set; }
    }
}
