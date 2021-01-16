using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the JWT default Authentication Scheme Options.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions" />
    public class JwtApiAuthenticationSchemeOptions: AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the client certificate thumbprint resolver.
        /// </summary>
        public IResolveCertificateThumbprint RequestCertificateThumbprintResolver { get; set; }

        /// <summary>
        /// Gets or sets the client IP address resolver.
        /// </summary>
        public IResolveIpAddress RequestIpAddressResolver { get; set; }

        /// <summary>
        /// Gets or sets the client IP address resolver.
        /// </summary>
        public IResolveJwtToken RequestJwtTokenResolver { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public ConfigAuthenticationJwt Configuration { get; set; }

        /// <summary>
        /// Gets or sets the JWT key used to verify the token.
        /// </summary>
        public SecurityKey JwtKey { get; set; }

        /// <summary>
        /// Gets or sets the name of internal authentication.
        /// </summary>
        public string InternalAuthType { get; set; } = "";
    }
}
