using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This interface identifies that the module requires start and stop events to be fired when the Microservice starts and stops.
    /// </summary>
    public interface IApiModuleService
    {
        /// <summary>
        /// This method is used to trigger a load after auto-creation.
        /// </summary>
        /// <param name="context">The application context. An exception will be thrown if this is null.</param>
        void Load(IApiStartupContextBase context);

        /// <summary>
        /// This method connects the module to the relevant shared context services.
        /// </summary>
        /// <param name="context">The application context. An exception will be thrown if this is null.</param>
        /// <param name="logger">The application logger.</param>
        void Connect(IApiStartupContextBase context, ILogger logger);

        /// <summary>
        /// This method is called to start a service when it is registered as part of the Microservice..
        /// </summary>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        Task Stop(CancellationToken cancellationToken);
    }
}
