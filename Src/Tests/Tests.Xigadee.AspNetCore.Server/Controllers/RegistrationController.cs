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
    public class RegistrationController : ApiControllerBase
    {
        readonly IApiUserSecurityModule _manager;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="config">The configuration.</param>
        public RegistrationController(ILogger<RegistrationController> logger
            , IApiUserSecurityModule manager) : base(logger)
        {
            _manager = manager;
        }
        #endregion

        #region Register([FromBody] RegisterModel info)
        /// <summary>
        /// This method is used to register a user.
        /// </summary>
        /// <param name="info">The model info.</param>
        [Route("register")]
        [HttpPost()]
        public virtual Task<IActionResult> Register([FromBody] RegistrationModel info) =>
             ProcessRequest(async () =>
             {
                 //Register and create a new user based on the info passed.
                 

                 return StatusCode(201);
             });

        #endregion

    }
}