using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
        /// <param name="services">The service collection.</param>
        void Load(IApiStartupContextBase context, IServiceCollection services);

        /// <summary>
        /// This method is used to configure the Microservice before it is started.
        /// </summary>
        void MicroserviceConfigure();

        /// <summary>
        /// This method connects the module to the relevant shared context services.
        /// </summary>
        /// <param name="logger">The application logger for the module.</param>
        void Connect(ILogger logger);

        /// <summary>
        /// This method is called to start a service when it is registered as part of the Microservice..
        /// </summary>
        Task Start(CancellationToken cancellationToken);

        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        Task Stop(CancellationToken cancellationToken);

        /// <summary>
        /// Specifies whether the module is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// This event is fired when the module active status is changed.
        /// </summary>
        event EventHandler<ApiModuleActiveEventArgs> OnActiveChange;
    }
}
