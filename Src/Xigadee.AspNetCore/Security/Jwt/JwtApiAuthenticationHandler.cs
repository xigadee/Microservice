using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Xigadee
{
    /// <summary>
    /// This module is used to validate the incoming token and assign a user ticket.
    /// </summary>
    public class JwtApiAuthenticationHandler : AuthenticationHandler<JwtApiAuthenticationSchemeOptions>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtApiAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="uSec">The user security module.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="clock">The clock.</param>
        public JwtApiAuthenticationHandler(IOptionsMonitor<JwtApiAuthenticationSchemeOptions> options
            , IApiUserSecurityModule uSec
            , ILoggerFactory logger
            , UrlEncoder encoder
            , ISystemClock clock) : base(options, logger, encoder, clock)
        {
            UserSecurityModule = uSec;
        } 
        #endregion
        #region UserSecurityModule
        /// <summary>
        /// Gets the user security module.
        /// </summary>
        protected IApiUserSecurityModule UserSecurityModule { get; }
        #endregion

        /// <summary>
        /// Handles the authentication.
        /// </summary>
        /// <returns>Returns the result.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                //OK, extract the auth scheme and token
                if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue authHeader))
                    return AuthenticateResult.NoResult();

                //Check whether this is authentication using JWT bearer token.
                if (!(authHeader?.Scheme.Equals("bearer", StringComparison.InvariantCultureIgnoreCase) ?? false))
                    return AuthenticateResult.NoResult();

                //OK, let's parse the JWT token
                var handler = new JwtSecurityTokenHandler();
                handler.InboundClaimTypeMap.Clear();

                if (!handler.CanReadToken(authHeader.Parameter))
                    return AuthenticateResult.Fail("Authorization bearer token cannot be read.");

                //Ok, let's validate the token. The auth key is set in the validation parameters.
                var vParams = GetTokenValidationParameters();
                var claimsPrincipal = handler.ValidateToken(authHeader.Parameter, vParams, out SecurityToken validatedToken);

                //OK, we have a valid token. Does this have to be a Https request?
                if (Options.Configuration.HttpsOnly && !Context.Request.IsHttps)
                    return AuthenticateResult.Fail("Https required error.");

                //Finally extract the Sid value from the token, and get the UserSession
                var sid = SidResolve(claimsPrincipal);

                //This is the user session object that we use to reference to other security entities.
                UserSession uSess = null;
                if (sid.HasValue)
                {
                    var rqUs = await UserSecurityModule.UserSessions.Read(sid.Value);
                    if (rqUs.IsSuccess)
                        uSess = rqUs.Entity;
                }

                //So we either have nothing in the database or the sid is invalid.
                if (uSess == null)
                    return AuthenticateResult.Fail("Session could not be resolved.");

                var ticket = await UserGenerateTicket(validatedToken, uSess);

                if (ticket != null)
                    return AuthenticateResult.Success(ticket);

                return AuthenticateResult.Fail("Ticket was not be generated");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Unexpected Auth failure");

                return AuthenticateResult.Fail(ex);
            }
        }


        #region UserGenerateTicket(UserSession uSess, User user, UserSecurity uSec, UserRoles ur)
        /// <summary>
        /// Generates a authentication ticket from the security objects.
        /// </summary>
        /// <param name="validatedToken">The validated token.</param>
        /// <param name="uSess">The user session.</param>
        /// <returns>Returns the authentication ticket.</returns>
        protected virtual async Task<AuthenticationTicket> UserGenerateTicket(SecurityToken validatedToken, UserSession uSess)
        {
            User user = null;
            UserRoles uRoles = null;

            string authtype = Options?.InternalAuthType ?? "XigadeeAuth";

            var identity = new ClaimsIdentity(authtype);

            //Add the session id.
            if (uSess != null)
                identity.AddClaim(new Claim(ClaimTypes.Sid, uSess.Id.ToString("N")));

            if (uSess.UserId.HasValue)
            {
                var uRs = await UserSecurityModule.Users.Read(uSess.UserId.Value);

                if (!uRs.IsSuccess)
                    throw new ArgumentOutOfRangeException($"UserId {uSess.UserId.Value} for Session {uSess.Id} cannot be read: {uRs.ResponseCode}/{uRs.ResponseMessage}");
                user = uRs.Entity;

                //Add the associated user id, if present.
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, user.Id.ToString("N")));

                var urRs = await UserSecurityModule.UserRoles.Read(user.Id);

                if (urRs.IsSuccess)
                    uRoles = urRs.Entity;

                //Add the user roles.
                uRoles?.Roles?.ForEach((c) => identity.AddClaim(new Claim(ClaimTypes.Role, c)));
            }
            identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, authtype);

            ticket.Properties.IssuedUtc = DateTime.UtcNow;

            return ticket;
        }
        #endregion
        #region GetTokenValidationParameters()
        /// <summary>
        /// Gets the token validation parameters for the Options configuration
        /// </summary>
        /// <returns>Returns the token validation parameters.</returns>
        protected virtual TokenValidationParameters GetTokenValidationParameters()
        {
            ConfigAuthenticationJwt auth = Options.Configuration;

            SecurityKey key = Options.JwtKey;

            if (key == null)
                key = new SymmetricSecurityKey(Convert.FromBase64String(auth.Key));

            return new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = auth.Audience,
                ValidateIssuer = true,
                ValidIssuer = auth.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = auth.ValidateTokenExpiry.GetValueOrDefault(true),
                ClockSkew = auth.GetClockSkew()
            };
        }
        #endregion
        #region SidResolve(ClaimsPrincipal claimsPrincipal)
        /// <summary>
        /// Resolves the sid id from the claims principal .
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        /// <returns>Returns the user id.</returns>
        protected Guid? SidResolve(ClaimsPrincipal claimsPrincipal)
        {
            var sid = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Sid, StringComparison.InvariantCultureIgnoreCase));

            if (Guid.TryParse(sid?.Value, out var userId))
                return userId;

            return null;
        } 
        #endregion
    }
}
