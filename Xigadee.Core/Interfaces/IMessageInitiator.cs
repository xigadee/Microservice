using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by client connectors to access the request
    /// functionality of the Microservice architecture.
    /// </summary>
    public interface IMessageInitiator : IServiceInitiator, IMessageHandler, IRequireScheduler
    {
        /// <summary>
        /// This method is called to calculate expired requests.
        /// </summary>
        Task OutgoingRequestsProcessTimeouts(Schedule schedule, CancellationToken token);
    }
}
