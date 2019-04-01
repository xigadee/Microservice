using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class JwtBearerTokenOptions
    {
        public JwtBearerTokenOptions(IClaimsPrincipalUserReferenceResolver claimsPrincipalUserResolver, TokenValidationParameters tokenValidationParameters, int? lifetimeWarningDays = null, int? lifetimeCriticalDays = null)
        {
            ClaimsPrincipalUserReferenceResolver = claimsPrincipalUserResolver ?? throw new ArgumentNullException(nameof(claimsPrincipalUserResolver));
            TokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
            LifetimeWarningDays = lifetimeWarningDays;
            LifetimeCriticalDays = lifetimeCriticalDays;
        }

        public IClaimsPrincipalUserReferenceResolver ClaimsPrincipalUserReferenceResolver { get; }

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
