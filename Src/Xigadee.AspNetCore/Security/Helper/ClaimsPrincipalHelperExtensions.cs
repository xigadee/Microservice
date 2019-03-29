using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Xigadee
{
    /// <summary>
    /// This class provides static extensions to the user account.
    /// </summary>
    public static class ClaimsPrincipalHelperExtensions
    {
        /// <summary>
        /// Extracts the user identifier from the claim.
        /// </summary>
        /// <param name="claimsPrincipal">The claims.</param>
        /// <returns>Returns the user id in the claim if it can be found.</returns>
        public static Guid? ExtractUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => ClaimTypes.Sid.Equals(c.Type, StringComparison.InvariantCultureIgnoreCase));

            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                return null;

            return userId;
        }

        /// <summary>
        /// Extracts the subject from the claim.
        /// </summary>
        /// <param name="claimsPrincipal">The claims.</param>
        /// <returns>Returns the subject it can be found.</returns>
        public static string ExtractSubject(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => JwtRegisteredClaimNames.Sub.Equals(c.Type, StringComparison.InvariantCultureIgnoreCase))?.Value;
        }

    }
}
