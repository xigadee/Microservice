using System;
using System.Net.Http;
using System.Net.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class used for API client connectivity.
    /// </summary>
    public abstract partial class ApiProviderBase
    {
        #region Constructor
        /// <summary>
        /// This is the empty constructor. This allows for generic creation using the new() constraint.
        /// You will have to set the properties explicitly using the ContextSet parameters.
        /// </summary>
        public ApiProviderBase()
        {
            Context = new ConnectionContext();
        }
        /// <summary>
        /// This is the parent constructor. This is used when creating a child container that inherits the parent security settings.
        /// </summary>
        /// <param name="parent">The parent connection.</param>
        protected ApiProviderBase(ApiProviderBase parent) : this(parent.Context) { }
        /// <summary>
        /// This is the parent constructor. This is used when creating a child container that inherits the parent security settings.
        /// </summary>
        /// <param name="context">The parent connection context.</param>
        protected ApiProviderBase(ConnectionContext context)
        {
            Context = context;
            ContextIsInherited = true;
        }

        /// <summary>
        /// This is the main constructor that sets the connector properties.
        /// </summary>
        /// <param name="uri">The root uri of the remote Api.</param>
        /// <param name="authHandlers">The auth handler collection.</param>
        /// <param name="clientCert">The client certificate to connect to the remote party.</param>
        /// <param name="manualCertValidation">The certificate validation function.</param>
        /// <param name="transportOverride">The transport serializer collection.</param>
        /// <param name="defaultErrorObjectDeserializer">This is the optional deserializer used to format error responses from the remote API.</param>
        protected ApiProviderBase(Uri uri
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            , IEnumerable<TransportSerializer> transportOverride = null
            , Func<HttpContent, Task<object>> defaultErrorObjectDeserializer = null
            )
        {
            Context = new ConnectionContext();

            Configure(uri, authHandlers, clientCert, manualCertValidation, transportOverride, defaultErrorObjectDeserializer);

        }
        #endregion

        #region UserAgentGet()
        /// <summary>
        /// This method returns the user agent that is passed to the calling party. 
        /// </summary>
        /// <returns>Returns a string containing the user agent.</returns>
        protected virtual string UserAgentGet()
        {
            var type = GetType();
            return $"Xigadee/{type.Assembly.GetName().Version} ({Environment.OSVersion})";
        }
        #endregion
        #region AssemblyVersionGet()
        /// <summary>
        /// This method returns the assembly version that is passed to the calling party. You can override this
        /// method to change the version, or leave it as null to stop sending this header.
        /// </summary>
        /// <returns>Returns a string containing the assembly version.</returns>
        protected virtual string AssemblyVersionGet()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        #endregion
        #region ApiVersionGet()
        /// <summary>
        /// This method returns the api version that is expecting. Leave this as null the skip this header.
        /// </summary>
        /// <returns>Returns a string containing the api version.</returns>
        protected virtual string ApiVersionGet()
        {
            return "2016-08-01";
        }
        #endregion

        #region ValidateCerts ...
        /// <summary>
        /// Validates the certs.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        /// <param name="cert">The cert.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="errs">The errs.</param>
        /// <returns>Returns true when the cert should be passed.</returns>
        protected virtual bool ValidateCerts(HttpRequestMessage message
            , X509Certificate2 cert
            , X509Chain chain
            , SslPolicyErrors errs)
        {
            return Context.ManualCertValidation(message, cert, chain, errs);

            //return errs == SslPolicyErrors.None;
        }
        #endregion

        #region ClientOverride
        /// <summary>
        /// Gets or sets the client override. This can be used for testing. It uses the HttpClient passed instead
        /// or creating a new client for each request.
        /// </summary>
        public HttpClient ClientOverride { get => Context.ClientOverride; set => Context.ClientOverride = value; }
        #endregion

        #region ResponseHeadersAuth(HttpRequestMessage rq, HttpResponseMessage rs)
        /// <summary>
        /// This method sets the prefer request headers for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="rs">The http response object.</param>
        protected virtual void ResponseHeadersAuth(HttpRequestMessage rq, HttpResponseMessage rs)
        {
        }
        #endregion

        #region FormatExceptionChain(Exception exception, string message = null)
        /// <summary>
        /// Formats the exception message including all inner exceptions. Useful when we have a send error on the API
        /// which might be caused by a DNS or a certificate issue etc. 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string FormatExceptionChain(Exception exception, string message = null)
        {
            if (exception == null)
                return message;

            message = string.IsNullOrEmpty(message) ? exception.Message : string.Format("{0} {1}", message, exception.Message);

            return FormatExceptionChain(exception.InnerException, message);
        }
        #endregion

        #region UriBaseAppend(string part, string query = null)
        /// <summary>
        /// this method safely appends the part and the query to the base uri
        /// </summary>
        /// <param name="part">The query part.</param>
        /// <param name="query">The optional query string.</param>
        /// <returns>Returns the combined uri.</returns>
        protected virtual Uri UriBaseAppend(string part, string query = null)
        {
            var uri = Context.Uri;
            var end = string.Concat(uri.LocalPath, part).Replace("//", "/");
            var bd = new UriBuilder(uri.Scheme, uri.Host, uri.Port, end);
            bd.Query = query;
            return bd.Uri;
        }
        #endregion

        #region NotImplemented...
        /// <summary>
        /// This allows a Not Implemented response (501) for methods in the API that are not supported.
        /// </summary>
        /// <typeparam name="O">The entity response type.</typeparam>
        /// <param name="errorObject">This is the optional error object passed back.</param>
        /// <returns>Returns the response with a not implemented error code.</returns>
        protected virtual Task<ApiResponse<O>> NotImplemented<O>(object errorObject = null)
        {
            var result = new ApiResponse<O>() { ResponseCode = 501 };
            result.ErrorObject = errorObject;

            return Task.FromResult(result);
        }
        /// <summary>
        /// This allows a Not Implemented response (501) for methods in the API that are not supported.
        /// </summary>
        /// <param name="errorObject">This is the optional error object passed back.</param>
        /// <returns>Returns the response with a not implemented error code.</returns>
        protected virtual Task<ApiResponse> NotImplemented(object errorObject = null)
        {
            var result = new ApiResponse() { ResponseCode = 501 };
            result.ErrorObject = errorObject;

            return Task.FromResult(result);
        }
        #endregion
    }
}
