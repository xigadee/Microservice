using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Test.Xigadee;
using Xigadee;

namespace Tests.Xigadee
{
    public class ApiProviderMondayMorningBlues : ApiProviderAsyncV2<Guid, MondayMorningBlues>
    {
        public ApiProviderMondayMorningBlues(Uri uri
            , RepositoryKeyManager<Guid> keyMapper = null
            , TransportUriMapper<Guid> transportUriMapper = null
            , IEnumerable<TransportSerializer> transportOverride = null
            , IApiProviderAuthBase authHandler = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            ) : base(uri, keyMapper, transportUriMapper, transportOverride, authHandler, clientCert, manualCertValidation)
        {
        }

        public async Task<bool> SessionStart()
        {
            var httpRq = new HttpRequestMessage(
                HttpMethod.Post, mUriMapper.Server + "/security/sessioncreate");

            var result = await Context.Client.SendAsync(httpRq);

            if (result.IsSuccessStatusCode)
            {
                var tokenB = await result.Content.ReadAsByteArrayAsync();

                var token = Encoding.UTF8.GetString(tokenB);

                if (!string.IsNullOrEmpty(token))
                {
                    Context.AuthHandlers.Clear();
                    Context.AuthHandlers.Add(new JwtAuthProvider(token));
                }

                return true;
            }

            return false;
        }
        public async Task<bool> SessionLogoff()
        {
            var httpRq = new HttpRequestMessage(HttpMethod.Post, mUriMapper.Server + "/security/logoff");

            //Set the headers
            RequestHeadersSet(httpRq);

            //Sets the supported transport mechanisms
            RequestHeadersSetTransport(httpRq);

            //Sets the authentication.
            RequestHeadersAuth(httpRq);

            HttpResponseMessage result = await Context.Client.SendAsync(httpRq);

            return result.IsSuccessStatusCode;
        }


        public async Task<bool> SessionLogon(string username, string password)
        {
            var httpRq = new HttpRequestMessage(HttpMethod.Post, mUriMapper.Server + "/security/logon");

            //Set the headers
            RequestHeadersSet(httpRq);

            //Sets the supported transport mechanisms
            RequestHeadersSetTransport(httpRq);

            //Sets the authentication.
            RequestHeadersAuth(httpRq);

            HttpResponseMessage result = null;

            using (var content = EntitySerialize(new SecurityInfo { Username = username, Password = password }))
            {
                httpRq.Content = content;

                result = await Context.Client.SendAsync(httpRq);
            }

            return result?.IsSuccessStatusCode ?? false;
        }

        /// <summary>
        /// This is the security information passed to a sign-on method.
        /// </summary>
        public class SecurityInfo
        {
            /// <summary>
            /// Gets or sets the username.
            /// </summary>
            public string Username { get; set; }
            /// <summary>
            /// Gets or sets the email.
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// Gets or sets the associated password.
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// Gets or sets the not a robot confirmation code.
            /// </summary>
            public string NotARobot { get; set; }
        }
    }
}
