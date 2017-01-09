using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This class contains and validates the Jwt Token.
    /// This class currently only supports simple HMAC-based verification.
    /// Thanks to http://kjur.github.io/jsjws/tool_jwt.html for verification.
    /// </summary>
    public static partial class JwtTokenExtensionMethods
    {
        public static IEnumerable<Claim> ToClaims(this JwtToken token)
        {
            return null;
        }
    }
}
