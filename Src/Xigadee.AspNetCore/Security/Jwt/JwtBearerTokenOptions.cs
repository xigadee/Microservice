using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class contains the bearer token options.
    /// </summary>
    public class JwtBearerTokenOptions
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="claimsPrincipalUserResolver">The principal claims resolver.</param>
        /// <param name="tokenValidationParameters">The token validation parameters</param>
        /// <param name="lifetimeWarningDays">The warning window for the lifetime of the token.</param>
        /// <param name="lifetimeCriticalDays">The critical warning period for the token.</param>
        public JwtBearerTokenOptions(IClaimsPrincipalUserReferenceResolver claimsPrincipalUserResolver
            , TokenValidationParameters tokenValidationParameters
            , int? lifetimeWarningDays = null
            , int? lifetimeCriticalDays = null)
        {
            ClaimsPrincipalUserReferenceResolver = claimsPrincipalUserResolver ?? throw new ArgumentNullException(nameof(claimsPrincipalUserResolver));
            TokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
            LifetimeWarningDays = lifetimeWarningDays;
            LifetimeCriticalDays = lifetimeCriticalDays;
        }

        /// <summary>
        /// The principal claims resolver.
        /// </summary>
        public IClaimsPrincipalUserReferenceResolver ClaimsPrincipalUserReferenceResolver { get; }

        /// <summary>
        /// The token validation parameters.
        /// </summary>
        public TokenValidationParameters TokenValidationParameters { get; }

        /// <summary>
        /// Number of days before expiry where a warning will be raised
        /// </summary>
        public int? LifetimeWarningDays { get; }

        /// <summary>
        /// Number of days before expiry where a critical error will be raised
        /// </summary>
        public int? LifetimeCriticalDays { get; }
    }
}
