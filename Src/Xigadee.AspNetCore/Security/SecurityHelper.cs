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
    /// This class is used to help set up the security.
    /// </summary>
    public static partial class SecurityHelper
    {
        /// <summary>
        /// Set the User and UserSecurity repository read settings.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="repo">The user/user security repository function.</param>
        public static void UserRepositorySet(this ApiAuthenticationSchemeOptions options, Func<IApiUserSecurityModule> repo)
        {
            options.RetrieveUser = (id) => repo().RetrieveUser(id);
            options.RetrieveUserSecurity = (id) => repo().RetrieveUserSecurity(id);
            options.RetrieveUserByReference = (id) => repo().RetrieveUser(id.type, id.value);
        }

        /// <summary>
        /// Adds the default microservice authentication.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="auth">The authentication settings.</param>
        /// <param name="repo">The User security repository.</param>
        /// <returns>Returns the service collection.</returns>
        public static IServiceCollection AddMicroserviceAuthentication(this IServiceCollection services
            , ConfigAuthenticationJwt auth
            , Func<IApiUserSecurityModule> repo = null)
        {
            repo = repo ?? (() => services.ServiceExtract<IApiUserSecurityModule>());

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = auth.Name;
                    options.DefaultAuthenticateScheme = auth.Name;
                })
                .AddScheme<ApiAuthenticationSchemeOptions, ApiAuthenticationHandler>(
                    auth.Name, "Default V1 Basic and V2 JWT authentication", options =>
                    {
                        //options.ClientCertificateThumbprintResolver = new ConditionalCertificateThumbprintResolver(
                        //    apimRequestIndicator,
                        //    apimMode,
                        //    new ApimHttpHeaderCertificateThumbprintResolver(),
                        //    new HttpConnectionCertificateThumbprintResolver());

                        //options.ClientIpAddressResolver = new ConditionalIpAddressResolver(
                        //    apimRequestIndicator,
                        //    apimMode,
                        //    new ApimHttpHeaderIpAddressResolver(),
                        //    new HttpConnectionIpAddressResolver());

                        options.HttpsOnly = auth.HttpsOnly;

                        options.UserRepositorySet(repo);

                        options.JwtBearerTokenOptions = BuildJwtBearerTokenOptions(repo(), auth);
                    }
                )
                ;

            return services;
        }

        private static IEnumerable<JwtBearerTokenOptions> BuildJwtBearerTokenOptions(IApiUserSecurityModule userSecurityModule, ConfigAuthenticationJwt auth)
        {
            var options = new List<JwtBearerTokenOptions>
            {
                new JwtBearerTokenOptions(
                    new CustomClaimsPrincipalUserResolver(userSecurityModule),
                    new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = auth.Audience,
                        ValidateIssuer = true,
                        ValidIssuer = auth.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(auth.Key)),
                        ValidateLifetime = auth.ValidateTokenExpiry.GetValueOrDefault(true),
                        ClockSkew = auth.GetClockSkew()
                    },
                    auth.LifetimeWarningDays,
                    auth.LifetimeCriticalDays)
            };

            return options;
        }
    }

    public class CustomClaimsPrincipalUserResolver : IClaimsPrincipalUserResolver
    {
        private readonly IApiUserSecurityModule _userSecurityModule;

        public CustomClaimsPrincipalUserResolver(IApiUserSecurityModule userSecurityModule)
        {
            _userSecurityModule = userSecurityModule;
        }

        public async Task<User> Resolve(ClaimsPrincipal claimsPrincipal)
        {
            var sid = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Sid, StringComparison.InvariantCultureIgnoreCase));

            if (!Guid.TryParse(sid?.Value, out var userId))
                return null;

            var (success, user) = await _userSecurityModule.RetrieveUser(userId);

            return success ? user : null;
        }
    }

    public interface IClaimsPrincipalUserResolver
    {
        Task<User> Resolve(ClaimsPrincipal claimsPrincipal);
    }

    public class JwtBearerTokenOptions
    {
        public JwtBearerTokenOptions(IClaimsPrincipalUserResolver claimsPrincipalUserResolver, TokenValidationParameters tokenValidationParameters, int? lifetimeWarningDays = null, int? lifetimeCriticalDays = null)
        {
            ClaimsPrincipalUserResolver = claimsPrincipalUserResolver ?? throw new ArgumentNullException(nameof(claimsPrincipalUserResolver));
            TokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
            LifetimeWarningDays = lifetimeWarningDays;
            LifetimeCriticalDays = lifetimeCriticalDays;
        }

        public IClaimsPrincipalUserResolver ClaimsPrincipalUserResolver { get; }

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

    public interface ICertificateThumbprintResolver
    {
        Task<string> Resolve(HttpContext httpContext);
    }

    public interface IIpAddressResolver
    {
        IPAddress Resolve(HttpContext httpContext);
    }





}
