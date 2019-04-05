using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Xigadee
{
    /// <summary>
    /// This module is used to validate the incoming token and assign a user ticket using the UserSecurityModule.
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

        #region HandleAuthenticateAsync()
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
                    return AuthenticateResult.Fail("Token is valid, but session could not be resolved.");

                //Ok, we have a valid session, let's see if this relates to a user.
                var ticket = await SessionGenerateUserTicket(validatedToken, uSess);

                if (ticket != null)
                    return AuthenticateResult.Success(ticket);

                return AuthenticateResult.Fail("Token is valid, but a user ticket was not be generated");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Unexpected Auth failure");

                return AuthenticateResult.Fail(ex);
            }
        } 
        #endregion
        #region UserGenerateTicket(UserSession uSess, User user, UserSecurity uSec, UserRoles ur)
        /// <summary>
        /// Generates a authentication ticket from the security objects.
        /// </summary>
        /// <param name="validatedToken">The validated token.</param>
        /// <param name="uSess">The user session.</param>
        /// <returns>Returns the authentication ticket.</returns>
        protected virtual async Task<AuthenticationTicket> SessionGenerateUserTicket(SecurityToken validatedToken, UserSession uSess)
        {
            //This fixes the IsAuthenticated issue when an empty string can cause it to fail.
            string authtype = Options?.InternalAuthType;
            if (string.IsNullOrEmpty(authtype))
                authtype = nameof(JwtApiAuthenticationHandler);

            bool IsAuthenticated = false;
            UserSecurity userSecurity = null;
            UserRoles userRoles = null;

            //Ok, we will add the user identifier and any claims if the account is active.
            if (uSess.UserId.HasValue)
            {
                var userId = uSess.UserId.Value;

                var usecRs = await UserSecurityModule.UserSecurities.Read(userId);
                if (usecRs.IsSuccess)
                {
                    userSecurity = usecRs.Entity;
                    IsAuthenticated = userSecurity.IsActive;
                    if (IsAuthenticated)
                    {
                        //OK, let's see what roles the user has assigned, if any?
                        var urRs = await UserSecurityModule.UserRoles.Read(userId);
                        if (urRs.IsSuccess)
                            userRoles = urRs.Entity;
                    }
                    else
                        Logger.LogWarning($"User {userId} is not active.");
                }
                else
                    Logger.LogWarning($"User {userId} for Session {uSess.Id} security cannot be read: {usecRs.ResponseCode}/{usecRs.ResponseMessage}");
            }

            var identity = new ClaimsIdentity(IsAuthenticated?authtype:"");

            //Add the session id.
            identity.AddClaim(new Claim(ClaimTypes.Sid, uSess.Id.ToString("N")));

            if (userSecurity != null && userSecurity.IsActive)
                //Add the associated user id, if present.
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, userSecurity.Id.ToString("N")));

            userRoles?.Roles?.ForEach((c) => identity.AddClaim(new Claim(ClaimTypes.Role, c)));

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
