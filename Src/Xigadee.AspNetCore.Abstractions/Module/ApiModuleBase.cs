using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for module that support standard module configuration.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    /// <typeparam name="CNF">The application configuration type.</typeparam>
    public abstract class ApiModuleBase<CTX,CNF> : ApiModuleBase<CTX>
        where CTX : class
        where CNF: IApiModuleConfigBase
    {
        /// <summary>
        /// The connector configuration.
        /// </summary>
        protected abstract CNF Config { get; }

        /// <summary>
        /// Specifies whether the service is enabled. The default is false.
        /// </summary>
        public virtual bool Enabled => Config?.Enabled ?? false;

        #region Service overrides
        /// <summary>
        /// This method sets the specific context.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="services">The application service collection.</param>
        public override void Load(IApiStartupContextBase context, IServiceCollection services)
        {
            base.Load(context, services);

            if (Enabled)
                LoadInternal(Context);
        }
        /// <summary>
        /// This method can be used to connect the module to the relevant application services.
        /// </summary>
        /// <param name="logger">The optional logger.</param>
        public override void Connect(ILogger logger)
        {
            base.Connect(logger);

            if (Enabled)
                ConnectInternal(Context);
        }
        /// <summary>
        /// This method can be used to configure the Microservice before it is started.
        /// </summary>
        public override void MicroserviceConfigure()
        {
            base.MicroserviceConfigure();

            if (Enabled)
                MicroserviceConfigureInternal();
        }
        /// <summary>
        /// This method is called to start a service when it is registered for a service call.
        /// </summary>
        public override async Task Start(CancellationToken cancellationToken)
        {
            if (Enabled)
            {
                await base.Start(cancellationToken);

                await StartInternal(cancellationToken);
            }
        }
        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        public override async Task Stop(CancellationToken cancellationToken)
        {
            if (Enabled)
            {
                await StopInternal(cancellationToken);

                await base.Stop(cancellationToken);
            }
        }
        #endregion

        /// <summary>
        /// This method is called to Load the service if it marked as enabled.
        /// </summary>
        /// <param name="context">The main context.</param>
        protected virtual void LoadInternal(CTX context) { }

        /// <summary>
        /// This method is called to connect the service to the other services if enabled.
        /// </summary>
        /// <param name="context">The main context.</param>
        protected virtual void ConnectInternal(CTX context) { }
        /// <summary>
        /// This override is called if the service is enabled.
        /// </summary>
        protected virtual void MicroserviceConfigureInternal() { }

        /// <summary>
        /// This method is called to start the service if it is enabled though config.
        /// </summary>
        /// <param name="cancellationToken">The token to cancel the process.</param>
        protected virtual Task StartInternal(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// This is used to stop the service based on the config getting..
        /// </summary>
        /// <param name="cancellationToken">The token to cancel the process.</param>
        protected virtual Task StopInternal(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    /// <summary>
    /// This method can be used to connect the module to the other application services.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    public abstract class ApiModuleBase<CTX>: ApiModuleBase
        where CTX:class
    {
        /// <summary>
        /// This is the application environment context.
        /// </summary>
        protected virtual CTX Context { get; set; }

        /// <summary>
        /// This method sets the specific context.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="services">The application service collection.</param>
        public override void Load(IApiStartupContextBase context, IServiceCollection services)
        {
            base.Load(context, services);

            if (context is CTX)
                Context ??= (CTX)context;
            else
                throw new ArgumentOutOfRangeException(nameof(context), $"{ErrString()} {nameof(context)} is not of type {typeof(CTX).Name}");
        }
    }

    /// <summary>
    /// This event class is fired when a CommandHarnessRequest object is generated.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [DebuggerDisplay("{IsActive} {Message}")]
    public class ApiModuleActiveEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarnessEventArgs"/> class.
        /// </summary>
        /// <param name="active">The active status of the connection.</param>
        /// <param name="reason">The status change reason.</param>
        public ApiModuleActiveEventArgs(bool active, string reason = null)
        {
            Active = active;
            Reason = reason;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public bool Active { get; }
        /// <summary>
        /// The status change reason.
        /// </summary>
        public string Reason { get; }

    }

    /// <summary>
    /// This is the base class for ApiModules
    /// </summary>
    public abstract class ApiModuleBase: CommandBase, IApiModuleService
    {
        /// <summary>
        /// This event fires when the active status of a module changes.
        /// </summary>
        public event EventHandler<ApiModuleActiveEventArgs> OnActiveChange;

        /// <summary>
        /// This method is used to change the active status.
        /// </summary>
        /// <param name="active">The boolean status.</param>
        /// <param name="reason">The status change reason.</param>
        protected virtual void ActiveChange(bool active, string reason = null)
        {
            var changed = _isActive ^ active;

            _isActive = active;

            try
            {
                if (changed)
                    OnActiveChange?.Invoke(this, new ApiModuleActiveEventArgs(active, reason));
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"{ErrString()} uncaught event exception.");
            }
        }

        /// <summary>
        /// This is the internal flag that marks the module as active.
        /// </summary>
        private bool _isActive { get; set; }

        /// <summary>
        /// Specifies that the module is active.
        /// </summary>
        public virtual bool IsActive => _isActive;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public virtual ILogger Logger { get; set; }
        /// <summary>
        /// This is the base context definition.
        /// </summary>
        public virtual IApiStartupContextBase ContextBase { get; set; }
        /// <summary>
        /// This is the application service collection.
        /// </summary>
        public virtual IServiceCollection Services { get; set; }

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
        public virtual void MicroserviceConfigure() => MicroserviceConnectCommand();

        /// <summary>
        /// This is the channel to attach the command.
        /// </summary>
        protected virtual string CommandChannelId => "command";
        /// <summary>
        /// This is the description for command attachment.
        /// </summary>
        protected virtual string CommandChannelDescription => $"Application Module {GetType().Name}";
        /// <summary>
        /// This method sepcifically sets the cmmand based on command properties.
        /// </summary>
        protected virtual void MicroserviceConnectCommand() => MicroserviceConnectCommand(CommandChannelId, CommandChannelDescription);
        /// <summary>
        /// THis method is used to connect the command to the relevant channel.
        /// </summary>
        /// <param name="channelId">THe incoming command channel id.</param>
        /// <param name="channelDescription">The option description.</param>
        protected virtual void MicroserviceConnectCommand(string channelId, string channelDescription = null)
        {
            var comms = ContextBase.MicroservicePipeline?.ToMicroservice().Communication;
            if (comms == null)
            {
                Logger.LogWarning($"{ErrString()} - could not attach command to Microservice as pipeline is not defined.");
                return;
            }

            IPipelineChannelIncoming<MicroservicePipeline> pipe;
            //If the channel does not exist, add the command to a new channel.
            if (!comms.TryGet(channelId, ChannelDirection.Incoming, out var channel))
                pipe = ContextBase.MicroservicePipeline.AddChannelIncoming(channelId, channelDescription ?? "This is the system channel used to hold system processes");
            else
                pipe = new ChannelPipelineIncoming<MicroservicePipeline>(ContextBase.MicroservicePipeline, channel);

            pipe.AttachCommand(this);
        }

        /// <summary>
        /// This method can be used to connect the module to the relevant application services.
        /// </summary>
        /// <param name="logger">The optional logger.</param>
        public virtual void Connect(ILogger logger)
        {
            Logger ??= logger;
        }

        /// <summary>
        /// This method is called to start a service when it is registered for a service call.
        /// </summary>
        public virtual Task Start(CancellationToken cancellationToken)
        {
            ActiveChange(true);
            return Task.CompletedTask;
        }
        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        public virtual Task Stop(CancellationToken cancellationToken)
        {
            ActiveChange(false);
            return Task.CompletedTask;
        }

        /// <summary>
        /// This helper method returns a short name for the module and the current line number.
        /// </summary>
        /// <param name="memberName">This is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">This is populated by the compiler.</param>
        /// <returns>Returns the debug string.</returns>
        public virtual string ErrString(
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0) => $"{GetType().Name}/{memberName}@{sourceLineNumber}";
    }
}
