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
        /// This is a list of clients for the sender.
        /// </summary>
        IEnumerable<ClientHolder> Clients { get; }
    }
}
