using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
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

        protected override Task InitializeHandlerAsync()
        {
            return base.InitializeHandlerAsync();
        }

        protected override Task InitializeEventsAsync()
        {
            return base.InitializeEventsAsync();
        }

        /// <summary>
        /// Handles the authentication.
        /// </summary>
        /// <returns>Returns the result.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (Options.Configuration.HttpsOnly && !Context.Request.IsHttps)
                    return AuthenticateResult.Fail("Https required error.");

                var result = AuthorizationSchemeExtract();

                if (!result.HasValue)
                    return AuthenticateResult.Fail("Authorization header not set.");

                if (result.Value.scheme != "bearer")
                    return AuthenticateResult.Fail($"Authorization scheme not supported: {result.Value.scheme}");

                var handler = new JwtSecurityTokenHandler();
                handler.InboundClaimTypeMap.Clear();

                //First, check we can parse the token
                if (!handler.CanReadToken(result.Value.token))
                    return AuthenticateResult.Fail("Authorization bearer token cannot be read.");

                //Ok, let's validate the token.
                var claimsPrincipal
                    = handler.ValidateToken(result.Value.token, GetTokenValidationParameters()
                        , out SecurityToken validatedToken);

                //This is the user session object that we use to reference to other security entities.
                UserSession uSess = null;

                //Finally extract the Sid value from the token, and get the UserSession
                var sid = SidResolve(claimsPrincipal);

                if (sid.HasValue)
                {
                    var rqUs = await UserSecurityModule.UserSessions.Read(sid.Value);
                    if (rqUs.IsSuccess)
                        uSess = rqUs.Entity;
                }

                //So we either have nothing in the database or the sid is invalid.
                if (uSess == null)
                {
                    uSess = new UserSession() { Source = GetType().Name };
                    var rsC = UserSecurityModule.UserSessions.Create(uSess);
                    sid = uSess.Id;
                }

                //if (sid.HasValue

                //var result = await ValidateAuth();

                    //if (result.ex != null)
                    //    return AuthenticateResult.Fail(result.ex);
                    //else if (result.ticket != null)
                    //    return AuthenticateResult.Success(result.ticket);

                    return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Unexpected Auth failure");

                return AuthenticateResult.Fail(ex);
            }
        }


        #region AuthorizationSchemeExtract()
        /// <summary>
        /// Extracts the specified scheme and token for authorization.
        /// </summary>
        /// <returns>the scheme and the token</returns>
        protected (string scheme, string token)? AuthorizationSchemeExtract()
        {
            string authorization = Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorization))
            {
                var parts = authorization.TrimStart().Split(' ');
                if (parts.Length > 1)
                    return (parts[0].Trim().ToLowerInvariant(), parts[1]);
            }

            return null;
        }
        #endregion
        #region UserGenerateTicket(UserSession uSess, User user, UserSecurity uSec, UserRoles ur)
        /// <summary>
        /// Generates a authentication ticket from the security objects.
        /// </summary>
        /// <param name="uSess">The user session.</param>
        /// <param name="user">The user.</param>
        /// <param name="uSec">The user security.</param>
        /// <param name="uRoles">The user roles. If this is null, no roles will be added to the ticket.</param>
        /// <returns>Returns the authentication ticket.</returns>
        protected virtual AuthenticationTicket UserGenerateTicket(UserSession uSess, User user, UserSecurity uSec, UserRoles uRoles)
        {
            string authtype = Options?.InternalAuthType ?? "XigadeeAuth";

            var identity = new ClaimsIdentity(authtype);

            //Add the session id.
            if (uSess != null)
                identity.AddClaim(new Claim(ClaimTypes.Sid, uSess.Id.ToString("N")));

            //Add the associated user id, if present.
            if (user != null)
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, user.Id.ToString("N")));
            else if (uSess?.UserId.HasValue ?? false)
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, uSess.UserId.Value.ToString("N")));

            //Add the user roles.
            uRoles?.Roles?.ForEach((c) => identity.AddClaim(new Claim(ClaimTypes.Role, c)));

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
    }
}
