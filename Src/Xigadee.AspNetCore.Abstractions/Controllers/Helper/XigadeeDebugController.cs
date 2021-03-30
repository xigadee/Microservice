using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xigadee;
namespace Xigadee
{ 
    /// <summary>
    /// This controller handles incoming Stripe callback requests.
    /// </summary>
    public abstract class XigadeeDebugControllerBase : ApiControllerBase
    {
        StatisticsHolder _holder;
        ConfigHealthCheck _config;

        #region Constructor
        /// <summary>
        /// This is the default controller.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="holder">The statistics holder.</param>
        /// <param name="config">The health check configuration.</param>
        protected XigadeeDebugControllerBase(ILogger logger, StatisticsHolder holder, ConfigHealthCheck config) : base(logger)
        {
            _holder = holder;
            _config = config;
        }
        #endregion

        /// <summary>
        /// This method takes the request parameter, validates it against the health check and outputs the statistics.
        /// </summary>
        protected virtual Task<IActionResult> Output(Guid id)
        {
            if (!_config.Enabled
                || id != _config.Id
                || _holder?.Statistics == null)
            {
                _logger.LogError($"XigadeeDebug failure {id}");
                return Task.FromResult<IActionResult>(StatusCode(404));
            }

            _logger.LogInformation("XigadeeDebug success");
            return Task.FromResult<IActionResult>(StatusCode(200, _holder.Statistics));
        }
    }
}
