using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50
{
    public class TestHostApiConnector : ApiProviderBase
    {
        #region Constructor
        /// <summary>
        /// This is the generics constructor.
        /// </summary>
        public TestHostApiConnector() : base()
        {
            DefaultErrorObjectDeserializer = ApiProviderHelper.ToErrorObject;
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="uri">The root uri of the remote Api.</param>
        /// <param name="authHandlers">The auth handler collection.</param>
        /// <param name="clientCert">The client certificate to connect to the remote party.</param>
        /// <param name="manualCertValidation">The certificate validation function.</param>
        /// <param name="transportOverride">The transport serializer collection.</param>
        public TestHostApiConnector(Uri uri
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            , IEnumerable<TransportSerializer> transportOverride = null)
            : base(uri, authHandlers ?? new List<IApiProviderAuthBase>(), clientCert, manualCertValidation, transportOverride)
        {
            DefaultErrorObjectDeserializer = ApiProviderHelper.ToErrorObject;
        }
        #endregion
    }
}
