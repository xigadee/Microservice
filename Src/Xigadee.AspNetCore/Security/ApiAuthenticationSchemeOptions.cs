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
        public ICertificateThumbprintResolver ClientCertificateThumbprintResolver { get; set; }

        /// <summary>
        /// Gets or sets the client IP address resolver.
        /// </summary>
        public IIpAddressResolver ClientIpAddressResolver { get; set; }

        /// <summary>
        /// Gets or sets the JWT bearer token options.
        /// </summary>
        public IEnumerable<JwtBearerTokenOptions> JwtBearerTokenOptions { get; set; }

        /// <summary>
        /// Gets or sets the retrieve user function.
        /// </summary>
        public Func<Guid, Task<(bool success, User user)>> RetrieveUser { get; set; }
        /// <summary>
        /// Gets or sets the retrieve user by reference function.
        /// </summary>
        public Func<(string type, string value), Task<(bool success, User user)>> RetrieveUserByReference { get; set; }
        /// <summary>
        /// Gets or sets the retrieve user security function.
        /// </summary>
        public Func<Guid, Task<(bool success, UserSecurity uSec)>> RetrieveUserSecurity { get; set; }

        /// <summary>
        /// Gets or sets a value to only validate connections on a HTTPS secure channel.
        /// </summary>
        public bool HttpsOnly { get; set; } = false;

        public string CertSalt { get; set; }
    }

}
