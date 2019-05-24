using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
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
    }
}
