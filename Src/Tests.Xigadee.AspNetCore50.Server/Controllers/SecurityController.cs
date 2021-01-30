using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tests.Xigadee.AspNetCore50;
using Xigadee;
namespace Tests.Xigadee.AspNetCore50
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ApiControllerBase
    {
        #region Declarations
        IApiUserSecurityModule _manager;
        ConfigAuthenticationJwt _config;

        EmailAddressAttribute _emailChecker = new EmailAddressAttribute();
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="config">The configuration.</param>
        public SecurityController(ILogger<SecurityController> logger
            , IApiUserSecurityModule manager, ConfigAuthenticationJwt config):base(logger)
        {
            _manager = manager;
            _config = config;
        } 
        #endregion

        #region SessionCreate()
        [Route("sessioncreate")]
        [HttpPost()]
        public virtual async Task<IActionResult> SessionCreate()
        {
            var uSess = new UserSession();

            uSess.AddCustomClaim("paul", "wahey!");

            var rsC = await _manager.UserSessions.Create(uSess);

            if (!rsC.IsSuccess)
                return StatusCode(rsC.ResponseCode);


            var token = JwtTokenHelper.CreateSymmetricHmacSha256(_config, uSess.Id);

            return StatusCode(200, token);
        }
        #endregion

        #region ValidateUsername([FromBody] RegisterValidate toCheck)
        [Route("validateusername")]
        [HttpPost()]
        public virtual async Task<IActionResult> ValidateUsername([FromBody] ValidateId toCheck)
        {
            var name = toCheck.Value;

            //Ok, do we have a username?
            if (string.IsNullOrWhiteSpace(name) || name.Length<6)//TODO: add validation
                return StatusCode(400);

            name = name.Trim().ToLowerInvariant();

            var uRq1 = await _manager.Users.VersionByRef(TestUser.KeyUsername, name);
            if (uRq1.IsSuccess)
                return StatusCode(400);

            return StatusCode(204);
        }
        #endregion
        #region ValidateEmail([FromBody] RegisterValidate toCheck)
        [Route("validateemail")]
        [HttpPost()]
        public virtual async Task<IActionResult> ValidateEmail([FromBody] ValidateId toCheck)
        {
            var email = toCheck.Value;

            //Ok, do we have a username?
            if (string.IsNullOrWhiteSpace(email) || !_emailChecker.IsValid(email))
                return StatusCode(400);

            email = email.Trim().ToLowerInvariant();          

            var uRq1 = await _manager.Users.VersionByRef(TestUser.KeyEmail, email);

            if (uRq1.IsSuccess)
                return StatusCode(400);

            return StatusCode(204);
        }
        #endregion

        #region Logon([FromBody]SecurityInfo sInfo)
        [Route("logon")]
        [HttpPost()]
        public virtual async Task<IActionResult> Logon([FromBody]LogonModel sInfo)
        {
            var sid = User.ExtractUserSessionId();

            if (!sid.HasValue)
                return StatusCode(400);

            var rsC = await _manager.UserSessions.Read(sid.Value);

            if (!rsC.IsSuccess)
                return StatusCode(rsC.ResponseCode);
            UserSession uSess = rsC.Entity;

            User user = null;

            //Ok, do we have a username?
            if (!string.IsNullOrWhiteSpace(sInfo.Username))
            {
                var username = sInfo.Username.Trim().ToLowerInvariant();

                var uRq1 = await _manager.Users.ReadByRef(TestUser.KeyUsername, username);
                if (uRq1.IsSuccess)
                    user = uRq1.Entity;
            }

            //Do we have an email address?
            if (user == null && !string.IsNullOrWhiteSpace(sInfo.Email))
            {
                var email = sInfo.Email.Trim().ToLowerInvariant();
                var uRq2 = await _manager.Users.ReadByRef(TestUser.KeyUsername, email);
                if (uRq2.IsSuccess)
                    user = uRq2.Entity;
            }

            //Have we resolved this to a user?
            if (user == null)
                return StatusCode(404);

            //Ok, let's read the UserSecurity
            var usecRq = await _manager.UserSecurities.Read(user.Id);
            if (!usecRq.IsSuccess)
                return StatusCode(412);
            var uSec = usecRq.Entity;

            //Is the user marked as active.
            if (!uSec.IsActive)
                return StatusCode(404);

            //OK, let's verify the password - we don't use the username as that may not be used.
            var valid = usecRq.Entity.AuthenticationVerify("", sInfo.Password);

            if (!valid)
                return StatusCode(404);

            uSess.UserId = user.Id;

            var rsCu = await _manager.UserSessions.Update(uSess);

            if (rsCu.IsSuccess)
                return StatusCode(200);

            return StatusCode(503);
        } 
        #endregion

        #region Logoff()
        [Route("logoff")]
        [HttpPost()]
        [Authorize()]
        public virtual async Task<IActionResult> Logoff()
        {
            var sid = User.ExtractUserSessionId();

            if (!sid.HasValue)
                return StatusCode(400);

            var rsC = await _manager.UserSessions.Read(sid.Value);

            if (!rsC.IsSuccess)
                return StatusCode(rsC.ResponseCode);
            UserSession uSess = rsC.Entity;

            if (!uSess.UserId.HasValue)
                return StatusCode(200);

            uSess.UserId = null;

            var rsCu = await _manager.UserSessions.Update(uSess);

            if (rsCu.IsSuccess)
                return StatusCode(200);

            return StatusCode(503);

        } 
        #endregion


    }

    public class ValidateId
    {
        public string Value { get; set; }
    }


}