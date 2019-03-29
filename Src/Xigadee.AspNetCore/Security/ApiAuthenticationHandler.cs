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
    /// This class handler the authentication for the Microservice class, both JWT token, legacy V1 auth, and Client SSL certificate.
    /// </summary>
    public class ApiAuthenticationHandler : ApiAuthenticationHandlerBase<ApiAuthenticationSchemeOptions>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="clock">The clock.</param>
        public ApiAuthenticationHandler(IOptionsMonitor<ApiAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }
        #endregion

        #region GenerateHash(string data)
        /// <summary>
        /// Generates the string base SHA256 hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private string GenerateHash(string data)
        {
            string hashString;
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            using (SHA256Managed hashstring = new SHA256Managed())
            {
                byte[] hash = hashstring.ComputeHash(bytes);
                hashString = Convert.ToBase64String(hash);
            }
            //byte[] originaldata = Convert.FromBase64String(hashString);
            return hashString;
        }
        #endregion

        #region ResolveUserByJwtToken(string token)
        /// <summary>
        /// Resolves the user by an incoming JWT token.
        /// </summary>
        /// <param name="token">The token.</param>
        protected virtual async Task<User> ResolveUserByJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear();

            User user = null;
            var tokenValidationExceptions = new List<Exception>();

            foreach (var jwtBearerTokenOption in Options.JwtBearerTokenOptions)
            {
                if (!handler.CanReadToken(token))
                    continue;

                var tokenValidationParameters = jwtBearerTokenOption.TokenValidationParameters.Clone();

                ClaimsPrincipal claimsPrincipal;

                try
                {
                    claimsPrincipal = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                    // Check whether this token is due to expire soon
                    var daysUntilExpiry = validatedToken.ValidTo.Subtract(DateTime.UtcNow).TotalDays;
                    if (daysUntilExpiry > (jwtBearerTokenOption.LifetimeCriticalDays ?? 0) && daysUntilExpiry <= (jwtBearerTokenOption.LifetimeWarningDays ?? 0))
                        Logger?.LogWarning($"{claimsPrincipal.ExtractUserId()} bearer token due to expire on {validatedToken.ValidTo:O}");
                    else if (daysUntilExpiry <= (jwtBearerTokenOption.LifetimeCriticalDays ?? 0))
                        Logger?.LogCritical($"{claimsPrincipal.ExtractUserId()} bearer token due to expire within {jwtBearerTokenOption.LifetimeCriticalDays} days ({validatedToken.ValidTo:O}). Re-issue immediately");
                }
                catch (Exception exception)
                {
                    tokenValidationExceptions.Add(exception);

                    if (jwtBearerTokenOption == Options.JwtBearerTokenOptions.Last())
                        throw new AggregateException("Bearer token could not be validated against any supported issuer", tokenValidationExceptions);

                    continue;
                }

                user = await jwtBearerTokenOption.ClaimsPrincipalUserResolver.Resolve(claimsPrincipal);
                if (user == null)
                {
                    Logger?.LogWarning($"Unable to resolve user for {claimsPrincipal?.ExtractUserId()?.ToString() ?? claimsPrincipal?.ExtractSubject() }");
                }
                else
                {
                    break;
                }
            }

            return user;
        }
        #endregion

        private string GenerateCertificateHash(string certificateThumbprint)
        {
            return GenerateHash($"{Options.CertSalt}:{certificateThumbprint}");
        }

        #region ResolveUserByClientCertificate(string certificateThumbprint)
        /// <summary>
        /// Resolves the user by client certificate.
        /// </summary>
        /// <param name="certificateThumbprint">The certificate thumbprint to validate.</param>
        protected virtual async Task<User> ResolveUserByClientCertificate(string certificateThumbprint)
        {
            //If there isn't a cert, then return.
            if (string.IsNullOrWhiteSpace(certificateThumbprint))
                return null;

            try
            {
                string hash = GenerateCertificateHash(certificateThumbprint);

                var rs = await Options.RetrieveUserByReference(("thumbprint", hash));

                if (rs.success)
                    return rs.user;

            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error resolving user from JWT token.");
                throw;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// This method validates the user's incoming IP address against any IP restrictions set in the user account.
        /// </summary>
        /// <param name="uSec">The user.</param>
        /// <returns>Returns true if the validation is successful, or not set in the user account.</returns>
        protected bool ValidateUserAgainstIpRange(UserSecurity uSec)
        {
            if (!uSec.HasIpRestrictions())
                return true;

            var clientIpAddress = Options.ClientIpAddressResolver.Resolve(Context);

            if (clientIpAddress == null)
                return false;

            return uSec.ValidateIpRestriction(clientIpAddress);
        }

        protected async Task<bool> ValidateUserAgainstClientCertificate(UserSecurity uSec)
        {
            if (!uSec.HasClientCertificateRestrictions())
                return true;

            var clientCertificateThumbprint = await Options.ClientCertificateThumbprintResolver.Resolve(Context);

            if (string.IsNullOrWhiteSpace(clientCertificateThumbprint))
                return false;

            var hash = GenerateCertificateHash(clientCertificateThumbprint);

            return uSec.ValidateCertificateRestriction(hash);
        }

        #region UserGenerateTicket(User user, UserSecurity uSec)
        /// <summary>
        /// Generates a authentication ticket from the Users and UserSecurity objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="uSec">The user security. If this is null, no roles will be added to the ticket.</param>
        /// <returns>Returns the authentication ticket.</returns>
        protected AuthenticationTicket UserGenerateTicket(User user, UserSecurity uSec)
        {
            var identity = new ClaimsIdentity("MicroserviceAuth");

            identity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString("N")));

            var principal = new ClaimsPrincipal(identity);

            //TODO: set roles
            //uSec?.Roles.ForEach((c) => identity.AddClaim(new Claim(ClaimTypes.Role, c.Value)));

            var ticket = new AuthenticationTicket(principal, "MicroserviceAuth");

            ticket.Properties.IssuedUtc = DateTime.UtcNow;

            return ticket;
        }
        #endregion

        /// <summary>
        /// Resolves the user from credentials.
        /// </summary>
        /// <returns>Returns the user if reconciled from the incoming auth information.</returns>
        protected async Task<User> ResolveUserFromCredentials()
        {
            User callingParty = null;

            var result = ExtractAuth();

            //Yes, let's get the user from the scheme?
            if (result.success)
                switch (result.scheme.ToLowerInvariant())
                {
                    case "bearer":
                        callingParty = await ResolveUserByJwtToken(result.token);
                        break;
                }
            else
            {
                //No auth, but is there a client certificate?
                //If there is a client SSL cert, can we resolve this to a user?
                var clientCertificateThumbprint = await Options.ClientCertificateThumbprintResolver.Resolve(Context);

                if (!string.IsNullOrWhiteSpace(clientCertificateThumbprint))
                    callingParty = await ResolveUserByClientCertificate(clientCertificateThumbprint);
            }

            return callingParty;
        }

        /// <summary>
        /// The method is used to validate the incoming auth and return a security ticket against the user repository.
        /// </summary>
        /// <returns>
        /// Returns a authentication ticket, or an exception, or both as null if no result.
        /// </returns>
        protected override async Task<(AuthenticationTicket ticket, Exception ex)> ValidateAuth()
        {
            if (Options.HttpsOnly && !Context.Request.IsHttps)
                return (null, null);
            //AuthenticateResult result = AuthenticateResult.Fail(;

            User user;

            //OK, first resolve the user from authentication scheme or client certificate.
            try
            {
                user = await ResolveUserFromCredentials();
            }
            catch (Exception unCaughtEx)
            {
                Logger?.LogError(unCaughtEx, "Resolve user exception.");
                return (null, unCaughtEx);
            }

            if (user != null)
            {
                var rs = await Options.RetrieveUserSecurity(user.Id);

                //OK, if we have a user security object, do we have any restrictions that we need to validate
                if (rs.success)
                    try
                    {
                        //Are there any IP restrictions in place that should deny access?
                        //Are there any client certificates that need to be validated against the user?
                        //This will throw an exception on failure to pass more detail to the calling party.
                        if (ValidateUserAgainstIpRange(rs.uSec)
                            && await ValidateUserAgainstClientCertificate(rs.uSec))
                        {
                            var ticket = UserGenerateTicket(user, rs.uSec);

                            Logger?.LogDebug("Authentication success {TraceIdentifier}:{RemoteIpAddress} for {UserId}", Context.TraceIdentifier, Context.Connection.RemoteIpAddress?.ToString(), user.Id);
                            return (ticket, null);
                        }
                    }
                    catch (Exception validationEx)
                    {
                        Logger?.LogError(validationEx, "Validate restrictions exception.");

                        return (null, validationEx);
                    }
            }


            // If found a user but failed to authenticate them log a warning to help with resolving issue           
            if (user != null)
                Logger?.LogWarning("Authentication Failed {UserId}-{TraceIdentifier}@{RemoteIpAddress}", user.Id, Context.TraceIdentifier, Context.Connection.RemoteIpAddress?.ToString());

            //Fail safe and do nothing: user not resolved, so unauthenticated.
            return (null, null);
        }
    }
}
