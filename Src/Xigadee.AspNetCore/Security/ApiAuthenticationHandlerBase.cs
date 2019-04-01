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
    /// This is the base AuthenticationHandler used to implement security for the Microservice.
    /// See https://www.codeproject.com/Articles/1221846/ASP-NET-Core-Security-Overview
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    public abstract class ApiAuthenticationHandlerBase<T> : AuthenticationHandler<T>
        where T : AuthenticationSchemeOptions, new()
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiAuthenticationHandlerBase{T}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="uSec">The user security module.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="clock">The clock.</param>
        protected ApiAuthenticationHandlerBase(
            IOptionsMonitor<T> options
            , IApiUserSecurityModule uSec
            , ILoggerFactory logger
            , UrlEncoder encoder
            , ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            UserSecurityModule = uSec;
        }
        #endregion
        #region UserSecurityModule
        /// <summary>
        /// Gets the user security module which contains the security entities.
        /// </summary>
        protected IApiUserSecurityModule UserSecurityModule { get; }
        #endregion


        #region ExtractAuth()
        /// <summary>
        /// Extracts the specified scheme and token for authorization.
        /// </summary>
        /// <returns>the scheme and the token</returns>
        protected (bool success, string scheme, string token) ExtractAuth()
        {
            string authorization = Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorization))
            {
                var parts = authorization.TrimStart().Split(' ');
                if (parts.Length > 1)
                    return (true, parts[0], parts[1]);
            }

            return (false, null, null);
        }
        #endregion
        #region TryExtractToken(string key, out string token)
        /// <summary>
        /// Extracts the token.
        /// </summary>
        /// <param name="key">The token key type.</param>
        /// <param name="token">The token as an output parameter..</param>
        /// <returns>Returns true if the token is found</returns>
        protected bool TryExtractToken(string key, out string token)
        {
            token = null;

            string authorization = Request.Headers["Authorization"];

            // If no authorization header found, nothing to process further
            if (!string.IsNullOrEmpty(authorization) &&
                authorization.StartsWith($"{key} ", StringComparison.OrdinalIgnoreCase))
            {
                token = authorization.Substring($"{key} ".Length).Trim();
            }

            return !string.IsNullOrEmpty(token);
        }
        #endregion

        #region GenerateHash(string data)
        /// <summary>
        /// Generates the string base SHA256 hash.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected string GenerateHash(string data)
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

        /// <summary>
        /// The abstract method is used to validate the incoming auth and return a security ticket.
        /// </summary>
        /// <returns>Returns a authentication ticket, an exception, or both as null if no result.</returns>
        protected abstract Task<(AuthenticationTicket ticket, Exception ex)> ValidateAuth();

        #region HandleAuthenticateAsync()
        /// <summary>
        /// Handles the authenticate asynchronous.
        /// </summary>
        /// <returns>Returns the AuthenticateResult</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var result = await ValidateAuth();

                if (result.ex != null)
                    return AuthenticateResult.Fail(result.ex);
                else if (result.ticket != null)
                    return AuthenticateResult.Success(result.ticket);

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Unexpected Auth failure");

                throw;
            }
        }
        #endregion
    }
}
