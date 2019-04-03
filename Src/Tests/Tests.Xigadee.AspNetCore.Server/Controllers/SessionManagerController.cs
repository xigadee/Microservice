using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xigadee;
namespace Tests.Xigadee.AspNetCore.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionManagerController : ControllerBase
    {
        ILogger _logger;
        IApiUserSecurityModule _manager;
        ConfigAuthenticationJwt _config;

        public SessionManagerController(ILogger<SessionManagerController> logger
            , IApiUserSecurityModule manager, ConfigAuthenticationJwt config)
        {
            _logger = logger;
            _manager = manager;
            _config = config;
        }

        [Route("create")]
        [HttpGet()]
        public virtual async Task<IActionResult> SessionCreate()
        {
            var uSess = new UserSession();

            var rsC = await _manager.UserSessions.Create(uSess);

            if (!rsC.IsSuccess)
                return StatusCode(rsC.ResponseCode);

            
            var token = JwtTokenHelper.CreateSymmetricHmacSha256(_config, uSess.Id); 

            return StatusCode(200, token);
        }
    }
}