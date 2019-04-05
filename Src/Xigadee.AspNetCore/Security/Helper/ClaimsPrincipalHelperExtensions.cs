using System;
using System.Linq;
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
        public static Guid? ExtractUserSessionId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Extract(ClaimTypes.Sid);

            if (claim == null || !Guid.TryParse(claim, out var id))
                return null;

            return id;
        }

        /// <summary>
        /// Extracts the user identifier from the claim.
        /// </summary>
        /// <param name="claimsPrincipal">The claims.</param>
        /// <returns>Returns the user id in the claim if it can be found.</returns>
        public static Guid? ExtractUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.Extract(ClaimTypes.PrimarySid);

            if (claim == null || !Guid.TryParse(claim, out var id))
                return null;

            return id;
        }

        /// <summary>
        /// Extracts the subject from the claim.
        /// </summary>
        /// <param name="claimsPrincipal">The claims.</param>
        /// <param name="claimId">The claim parameter id.</param>
        /// <returns>Returns the subject it can be found.</returns>
        public static string Extract(this ClaimsPrincipal claimsPrincipal, string claimId)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => claimId.Equals(c.Type, StringComparison.InvariantCultureIgnoreCase))?.Value;
        }

    }
}
