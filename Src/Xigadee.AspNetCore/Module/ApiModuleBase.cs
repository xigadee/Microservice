using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
namespace Xigadee
{
    /// <summary>
    /// This method can be used to connect the module to the other application services.
    /// </summary>
    /// <typeparam name="C">The application context type.</typeparam>
    public abstract class ApiModuleBase<C>: ApiModuleBase
    {
        /// <summary>
        /// This is the application context.
        /// </summary>
        protected C _context;

        /// <summary>
        /// This method can be used to connect the module to the relevant application services.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="logger">The optional logger.</param>
        public virtual void Connect(C context, ILogger logger = null)
        {
            _context = context;
            if (logger != null)
                Logger = logger;
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
        /// This method is called to start a service when it is registered for a service call.
        /// </summary>
        public virtual Task Start(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is called to stop a registered service.
        /// </summary>
        public virtual Task Stop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// This helper method returns a short name for the module and the current line number.
        /// </summary>
        /// <param name="memberName">This is filled in by the compiler.</param>
        /// <param name="sourceLineNumber">This is populated by the compiler.</param>
        /// <returns>Returns the debug string.</returns>
        public string ErrString(
            [CallerMemberName] string memberName = "",
            [CallerLineNumber] int sourceLineNumber = 0) =>
            $"{GetType().Name}/{memberName}@{sourceLineNumber}";
    }
}
