using System;
using System.Net.Http;
using System.Net.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract partial class ApiProviderBase
    {
        #region Class -> ConnectionContext
        /// <summary>
        /// This class holds the connection context.
        /// </summary>
        public class ConnectionContext
        {
            /// <summary>
            /// This is a list of auth handlers to be used to authorise the request.
            /// </summary>
            public List<IApiProviderAuthBase> AuthHandlers { get; set; }
            /// <summary>
            /// The manual cert validation function.
            /// </summary>
            public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ManualCertValidation { get; set; }
            /// <summary>
            /// This is the base Uri for the calls.
            /// </summary>
            public Uri Uri { get; set; }
            /// <summary>
            /// The http client handler this is used to add client based certificates.
            /// </summary>
            public HttpClientHandler Handler { get; set; }

            /// <summary>
            /// This is the primary transport used for sending requests.
            /// </summary>
            public string TransportOutDefault { get; set; }
            /// <summary>
            /// This is the collection of transports available for serialization.
            /// </summary>
            public Dictionary<string, TransportSerializer> TransportSerializers { get; set; }

            #region TransportSerializerDefault
            /// <summary>
            /// This is the default serializer.
            /// </summary>
            public TransportSerializer TransportSerializerDefault => TransportSerializers[TransportOutDefault];
            #endregion

            #region Client
            /// <summary>
            /// Get a new http client or uses the override.
            /// </summary>
            public HttpClient Client => ClientOverride ?? new HttpClient(Handler);
            #endregion
            #region ClientOverride
            /// <summary>
            /// Gets or sets the client override. This can be used for testing. It uses the HttpClient passed instead
            /// or creating a new client for each request.
            /// </summary>
            public HttpClient ClientOverride { get; set; }
            #endregion
            /// <summary>
            /// This is the User Agent for the context. If this is null, the header will be skipped.
            /// </summary>
            public string UserAgent { get; set; }
            /// <summary>
            /// This is the value set for the x-api-clientversion header. If this is null, the header will be skipped.
            /// </summary>
            public string AssemblyVersion { get; set; }
            /// <summary>
            /// This is the value set for the x-api-version header. If this is null, the header will be skipped.
            /// </summary>
            public string ApiVersion { get; set; }
            /// <summary>
            /// This is the default error object deserializer.
            /// </summary>
            public Func<HttpContent, Task<object>> DefaultErrorObjectDeserializer { get; set; } = ApiProviderHelper.ToErrorObject;
        }
        #endregion

        #region Context
        /// <summary>
        /// This is the Api connection context.
        /// </summary>
        public ConnectionContext Context { get; }
        #endregion

        #region ContextIsInherited
        /// <summary>
        /// This property is true if the context was inherited from a parent container.
        /// </summary>
        public bool ContextIsInherited { get; protected set; }
        #endregion

        #region Configure(Uri uri, string token ...
        /// <summary>
        /// This method configures the connection with the URI and the token.
        /// </summary>
        /// <param name="uri">The Api Uri.</param>
        /// <param name="jwtToken">The JWT token.</param>
        /// <param name="clientCert">The client certificate to connect to the remote party.</param>
        /// <param name="manualCertValidation">The certificate validation function.</param>
        /// <param name="transportOverride">The transport serializer collection.</param>
        /// <param name="defaultErrorObjectDeserializer">This is the optional deserializer used to format error responses from the remote API.</param>
        public virtual void Configure(Uri uri, string jwtToken
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            , IEnumerable<TransportSerializer> transportOverride = null, Func<HttpContent, Task<object>> defaultErrorObjectDeserializer = null) 
            => Configure(uri, new[] { new JwtAuthProvider(jwtToken) }, clientCert, manualCertValidation, transportOverride, defaultErrorObjectDeserializer);

        /// <summary>
        /// This method can be used to change the context parameters.
        /// </summary>
        /// <param name="uri">The root uri of the remote Api.</param>
        /// <param name="authHandlers">The auth handler collection.</param>
        /// <param name="clientCert">The SSL client certificate to use when connecting to the remote party.</param>
        /// <param name="manualCertValidation">The certificate validation function.</param>
        /// <param name="transportOverride">The transport serializer collection.</param>
        /// <param name="defaultErrorObjectDeserializer">This is the optional deserializer used to format error responses from the remote API.</param>
        public virtual void Configure(Uri uri
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            , IEnumerable<TransportSerializer> transportOverride = null
            , Func<HttpContent, Task<object>> defaultErrorObjectDeserializer = null
            )
        {
            if (ContextIsInherited)
                throw new ConnectionContextIsInheritedException();

            // Get the types assembly version to add to the request headers
            Context.AuthHandlers = authHandlers?.ToList() ?? new List<IApiProviderAuthBase>();

            Context.Uri = uri ?? throw new ArgumentNullException("uri");

            Context.Handler = new HttpClientHandler();
            Context.Handler.AllowAutoRedirect = false;

            if (manualCertValidation != null)
            {
                Context.ManualCertValidation = manualCertValidation;
                Context.Handler.ServerCertificateCustomValidationCallback = ValidateCerts;
            }

            if (clientCert != null)
            {
                Context.Handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                Context.Handler.ClientCertificates.Add(clientCert);
            }

            if (transportOverride == null || transportOverride.Count() == 0)
            {
                Context.TransportSerializers = new Dictionary<string, TransportSerializer>();
                var defaultTs = new JsonTransportSerializer();
                Context.TransportOutDefault = defaultTs.MediaType.ToLowerInvariant();
                Context.TransportSerializers[Context.TransportOutDefault] = defaultTs;
            }
            else
            {
                Context.TransportOutDefault = transportOverride.First().MediaType.ToLowerInvariant();
                Context.TransportSerializers = transportOverride.ToDictionary(t => t.MediaType.ToLowerInvariant(), t => t);
            }

            Context.UserAgent = UserAgentGet();
            Context.AssemblyVersion = AssemblyVersionGet();
            Context.ApiVersion = ApiVersionGet();

            if (defaultErrorObjectDeserializer != null)
                Context.DefaultErrorObjectDeserializer = defaultErrorObjectDeserializer;
        } 
        #endregion
    }
}
