using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This controller is used to create an API user session and set the necessary cookie and JwtToken
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    public abstract class SessionControllerBase : ControllerBase
    {
        protected SessionControllerBase(ILogger logger, IApiUserSecurityModule security)
        {

        }
    }
}
