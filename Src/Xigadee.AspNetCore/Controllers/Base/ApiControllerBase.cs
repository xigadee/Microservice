using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This class is used to generic response processing.
    /// </summary>
    public abstract class ApiControllerBase: ControllerBase
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// This is the base method to allow for extended logging.
        /// </summary>
        /// <param name="logger"></param>
        protected ApiControllerBase(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This method wraps a request to a module or other methods and captures exceptions and processes them
        /// in a consistent manner.
        /// </summary>
        /// <param name="name">The name of the calling method. This is used for more detailed logging.</param>
        /// <param name="a">The function to execute.</param>
        /// <param name="extendErrors">Specifies whether the error should be appended to the response.</param>
        /// <returns></returns>
        protected async Task<IActionResult> ProcessRequest(Func<Task<IActionResult>> a
            , bool extendErrors = true
            , [CallerMemberName] string name = null
            )
        {
            try
            {
                return await a();
            }
            catch (HttpStatusOutputException hex)
            {
                _logger.LogWarning(hex, $"{GetType().Name}/{name} output exception - {hex.StatusCode}/{hex.StatusSubcode}/{hex.Message}");

                if (extendErrors)
                    return StatusCode(hex.StatusCode, ErrorFormat(hex));

                return StatusCode(hex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name}/{name} unhandled exception");

                if (extendErrors)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMessage { Code = 500 });

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// This method is used to format an error object to be returned to the client for an error.
        /// </summary>
        /// <param name="hex">The http exception.</param>
        /// <returns>Returns a detailed error message</returns>
        protected virtual ErrorMessage ErrorFormat(HttpStatusOutputException hex)
        {
            return new ErrorMessage { Code = hex.StatusCode, Subcode = hex.StatusSubcode };
        }

        /// <summary>
        /// This is the response message format for extended error logging.
        /// </summary>
        protected class ErrorMessage
        {
            /// <summary>
            /// The error code.
            /// </summary>
            public int Code { get; set; }
            /// <summary>
            /// The error subcode.
            /// </summary>
            public int? Subcode { get; set; }
            /// <summary>
            /// The error subcode.
            /// </summary>
            public string Description { get; set; }
        }
    }
}
