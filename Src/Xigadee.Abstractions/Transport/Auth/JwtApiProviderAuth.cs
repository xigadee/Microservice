using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This auth method provides the token for the connection.
    /// </summary>
    /// <seealso cref="Xigadee.IApiProviderAuthBase" />
    public class JwtApiProviderAuth : IApiProviderAuthBase
    {
        readonly string _jwtToken;
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtApiProviderAuth"/> class.
        /// </summary>
        /// <param name="jwtToken">The JWT token to store..</param>
        /// <exception cref="ArgumentNullException">jwtToken</exception>
        public JwtApiProviderAuth(string jwtToken)
        {
            _jwtToken = jwtToken ?? throw new ArgumentNullException("jwtToken");
        }
        /// <summary>
        /// Processes the request and appends the token.
        /// </summary>
        /// <param name="rq">The request.</param>
        public void ProcessRequest(HttpRequestMessage rq)
        {
            rq.Headers.Authorization = new AuthenticationHeaderValue("bearer", $"{_jwtToken}");
        }

        /// <summary>
        /// Processes the response. This does not do anything.
        /// </summary>
        /// <param name="rq">The response.</param>
        public void ProcessResponse(HttpResponseMessage rs)
        {
        }
    }
}
