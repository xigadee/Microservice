#region using

using System;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;

#endregion
namespace Xigadee
{
    public class SecurityControllerAsyncBase: ApiController
    {

        public void SignOn()
        {
            //var tokenHandler = new JwtSecurityTokenHandler();

            //SecurityKey key = new System.IdentityModel.Tokens.X509AsymmetricSecurityKey();

            //SigningCredentials creds = new SigningCredentials(key, "hello");;

            //var jwt = new JwtSecurityToken(
            //    token.Issuer,
            //    token.Audience,
            //    token.Claims,
            //    new Lifetime(DateTime.UtcNow, DateTime.UtcNow.AddSeconds(token.Lifetime)),
            //    credentials);

            
        }
    }
}
