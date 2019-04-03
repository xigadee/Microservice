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
    public class JwtAuthProvider : IApiProviderAuthBase
    {
        readonly string _jwtToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthProvider"/> class.
        /// </summary>
        /// <param name="jwtToken">The JWT token to store..</param>
        /// <exception cref="ArgumentNullException">jwtToken</exception>
        public JwtAuthProvider(string jwtToken)
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

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.String"/> to <see cref="JwtAuthProvider"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator JwtAuthProvider(string token)
        {
            return new JwtAuthProvider(token);
        }
    }
}
