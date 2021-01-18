using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface identifies that the module requires start and stop events to be fired when the Microservice starts and stops.
    /// </summary>
    public interface IApiModuleService
    {
        /// <summary>
        /// This method is called to start a service when it is registered for a service call.
        /// </summary>
        Task Start(CancellationToken cancellationToken);
        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        Task Stop(CancellationToken cancellationToken);
    }
}
