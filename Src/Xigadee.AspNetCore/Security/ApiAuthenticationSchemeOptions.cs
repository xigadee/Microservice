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
    /// This class contains the authentication options.
    /// </summary>
    public class ApiAuthenticationSchemeOptions : AuthenticationSchemeOptions
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
        /// Gets or sets the JWT bearer token options.
        /// </summary>
        public IEnumerable<JwtBearerTokenOptions> JwtBearerTokenOptions { get; set; }

        /// <summary>
        /// Gets or sets a value to only validate connections on a HTTPS secure channel.
        /// </summary>
        public bool HttpsOnly { get; set; } = false;

        /// <summary>
        /// Gets or sets the cert salt.
        /// </summary>
        public string CertSalt { get; set; }
    }

}
