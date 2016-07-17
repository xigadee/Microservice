using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Xigadee;

namespace Test.Xigadee.Api.Server.Controllers
{
    public class SecurityController : SecurityControllerAsyncBase
    {

        // POST api/Account/Register
        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> Register()
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            //IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            return Ok();
        }
    }
}
