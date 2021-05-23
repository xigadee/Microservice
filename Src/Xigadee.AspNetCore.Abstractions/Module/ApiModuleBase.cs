using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xigadee
{
    /// <summary>
    /// This method can be used to connect the module to the other application services.
    /// </summary>
    /// <typeparam name="C">The application context type.</typeparam>
    public abstract class ApiModuleBase<C>: ApiModuleBase
        where C:class
    {
        /// <summary>
        /// This is the application environment context.
        /// </summary>
        protected virtual C Context { get; set; }

        /// <summary>
        /// This method sets the specific context.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="services">The application service collection.</param>
        public override void Load(IApiStartupContextBase context, IServiceCollection services)
        {
            base.Load(context, services);

            if (context is C)
                Context ??= (C)context;
            else
                throw new ArgumentOutOfRangeException(nameof(context), $"{ErrString()} {nameof(context)} is not of type {typeof(C).Name}");
        }
        /// <summary>
        /// This method can be used to connect the module to the relevant application services.
        /// </summary>
        /// <param name="logger">The optional logger.</param>
        public override void Connect(ILogger logger)
        {
            base.Connect(logger);
        }
    }

    /// <summary>
    /// This is the base class for ApiModules
    /// </summary>
    public abstract class ApiModuleBase: IApiModuleService
    {
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// This is the base context definition.
        /// </summary>
        public IApiStartupContextBase ContextBase { get; set; }
        /// <summary>
        /// This is the application service collection.
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// This is the load method. This is called after the module has been automatically created.
        /// </summary>
        /// <param name="context">The application context. This will throw an exception if this is not set.</param>
        /// <param name="services">The application service collection.</param>
        public virtual void Load(IApiStartupContextBase context, IServiceCollection services)
        {
            ContextBase ??= context;
            Services ??= services;
        }

        /// <summary>
        /// This method can be used to configure the Microservice before it is started.
        /// </summary>
        public virtual void MicroserviceConfigure()
        {
        }

        /// <summary>
        /// This method can be used to connect the module to the relevant application services.
        /// </summary>
        /// <param name="logger">The optional logger.</param>
        public virtual void Connect(ILogger logger)
        {
            if (logger != null)
                Logger = logger;
        }

        /// <summary>
        /// This method is called to start a service when it is registered for a service call.
        /// </summary>
        public virtual Task Start(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        public virtual Task Stop(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// This helper method returns a short name for the module and the current line number.
        /// </summary>
        /// <param name="memberName">This is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">This is populated by the compiler.</param>
        /// <returns>Returns the debug string.</returns>
        public string ErrString(
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0) => $"{GetType().Name}/{memberName}@{sourceLineNumber}";
    }
}
