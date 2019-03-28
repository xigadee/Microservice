#region using
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Newtonsoft.Json;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This abstract base class is used for Api client connectivity.
    /// </summary>
    public abstract class ApiProviderBase
    {
        #region Declarations
        /// <summary>
        /// This is the assembly version
        /// </summary>
        protected readonly string mAssemblyVersion;
        /// <summary>
        /// This is a list of auth handlers to be used to authorise the request.
        /// </summary>
        protected readonly List<IApiProviderAuthBase> mAuthHandlers;
        /// <summary>
        /// The manual cert validation function.
        /// </summary>
        protected readonly Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> mManualCertValidation;
        /// <summary>
        /// This is the base Uri for the calls.
        /// </summary>
        protected readonly Uri mUri;
        /// <summary>
        /// The http client handler this is used to add client based certificates.
        /// </summary>
        protected readonly HttpClientHandler mHandler;
        /// <summary>
        /// This is the user agent that will be passed in the request header
        /// </summary>
        protected readonly string mUserAgent;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        protected ApiProviderBase(Uri uri
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null)
        {
            // Get the types assembly version to add to the request headers
            mAssemblyVersion = AssemblyVersionGet();
            mAuthHandlers = authHandlers?.ToList();

            mUri = uri ?? throw new ArgumentNullException("uri");
            // Get the types assembly version to add to the request headers
            mUserAgent = UserAgentGet();

            mHandler = new HttpClientHandler();
            mHandler.AllowAutoRedirect = false;

            if (manualCertValidation != null)
            {
                mManualCertValidation = manualCertValidation;
                mHandler.ServerCertificateCustomValidationCallback = ValidateCerts;
            }

            if (clientCert != null)
            {
                mHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                mHandler.ClientCertificates.Add(clientCert);
            }
        }
        #endregion
        #region AssemblyVersionGet()
        /// <summary>
        /// This method returns the user agent that is passed to the calling party. 
        /// </summary>
        /// <returns>Returns a string containing the user agent.</returns>
        protected virtual string UserAgentGet()
        {
            var type = GetType();
            return $"{GetType().Name}/{type.Assembly.GetName().Version}";
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
            return mManualCertValidation(message, cert, chain, errs);

            //return errs == SslPolicyErrors.None;
        }
        #endregion

        #region ClientOverride
        /// <summary>
        /// Gets or sets the client override. This can be used for testing. It uses the HttpClient passed instead
        /// or creating a new client for each request.
        /// </summary>
        public HttpClient ClientOverride { get; set; } 
        #endregion

        #region AssemblyVersionGet()
        /// <summary>
        /// This method returns the assembly version that is passed to the calling party. You can override this
        /// method to change the version.
        /// </summary>
        /// <returns>Returns a string containing the assembly version.</returns>
        protected virtual string AssemblyVersionGet()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
        #endregion

        #region Request(HttpMethod verb, Uri uri)
        /// <summary>
        /// This method creates the default request message.
        /// </summary>
        /// <param name="verb">The HTTP verb.</param>
        /// <param name="uri">The Uri request.</param>
        /// <returns>Returns the message with the full domain request.</returns>
        protected virtual HttpRequestMessage Request(HttpMethod verb, Uri uri)
        {
            HttpRequestMessage rq = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = uri
            };

            return rq;
        }
        #endregion
        #region RequestHeadersSet(HttpRequestMessage rq)
        /// <summary>
        /// This virtual method sets the necessary headers for the request.
        /// </summary>
        /// <param name="rq">The http request.</param>
        protected virtual void RequestHeadersSet(HttpRequestMessage rq)
        {
            rq.Headers.Add("User-Agent", mUserAgent);
            rq.Headers.Add("x-api-clientversion", mAssemblyVersion);
            rq.Headers.Add("x-api-version", "2016-08-01");
        }
        #endregion
        #region RequestHeadersPreferSet(HttpRequestMessage rq, Dictionary<string, string> Prefer)
        /// <summary>
        /// This method sets the prefer request header directives for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="Prefer">The prefer collection.</param>
        protected virtual void RequestHeadersPreferSet(HttpRequestMessage rq, Dictionary<string, string> Prefer)
        {
            if (Prefer != null && Prefer.Count > 0)
                rq.Headers.Add("Prefer", Prefer.Select((k) => string.Format("{0}={1}", k.Key, k.Value)));
        }
        #endregion

        #region RequestHeadersAuthSet(HttpRequestMessage rq)
        /// <summary>
        /// This method sets the prefer request headers for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        protected virtual void RequestHeadersAuth(HttpRequestMessage rq)
        {
            mAuthHandlers?.ForEach((a) => a.ProcessRequest(rq));
        }
        #endregion
        #region ResponseHeadersAuth(HttpRequestMessage rq, HttpResponseMessage rs)
        /// <summary>
        /// This method sets the prefer request headers for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="Prefer">The prefer collection.</param>
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

        #region JsonBody...
        /// <summary>
        /// Converts the object in to a Json body.
        /// </summary>
        /// <param name="body">The body object.</param>
        /// <returns>Returns a Http content object.</returns>
        protected HttpContent JsonBody(object body)
        {
            string json = JsonConvert.SerializeObject(body);
            return JsonBody(json);
        }
        /// <summary>
        /// Converts the text in to a Json body.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        protected HttpContent JsonBody(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            var content = new ByteArrayContent(data);
            content.Headers.Add("Content-Type", "application/json; charset=utf-8");
            return content;
        }
        #endregion
        #region TextBody(string text)
        /// <summary>
        /// Converts the texts in to a http body.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        protected HttpContent TextBody(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            var content = new ByteArrayContent(data);
            content.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            return content;
        }
        #endregion

        #region RequestHeadersSetTransport(HttpRequestMessage rq)
        /// <summary>
        /// This method sets the media quality type for the entity transfer.
        /// </summary>
        /// <param name="rq">The http request.</param>
        protected virtual void RequestHeadersSetTransport(HttpRequestMessage rq)
        {
            rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region CallClient<KT,ET>...
        /// <summary>
        /// This method calls the client using HTTP and returns the response along with the entity in the response if supplied.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="uri">The request Uri.</param>
        /// <param name="options">The repository settings passed from the caller.</param>
        /// <param name="content">The HttpContent to send to the API.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="deserializer">Deserialize the response content into the entity</param>
        /// <returns>Returns the repository holder.</returns>
        protected virtual Task<RepositoryHolder<KT, ET>> CallClient<KT, ET>(
              KeyValuePair<HttpMethod, Uri> uri
            , RepositorySettings options
            , HttpContent content = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpResponseMessage, RepositoryHolder<KT, ET>> mapOut = null
            , Action<HttpResponseMessage, byte[]
            , RepositoryHolder<KT, ET>> deserializer = null) =>
            CallClient(uri.Key, uri.Value, options, content, adjustIn, mapOut, deserializer);

        /// <summary>
        /// This method calls the client using HTTP and returns the response along with the entity in the response if supplied.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The request Uri.</param>
        /// <param name="options">The repository settings passed from the caller.</param>
        /// <param name="content">The HttpContent to send to the API.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="deserializer">Deserialize the response content into the entity</param>
        /// <returns>Returns the repository holder.</returns>
        protected virtual async Task<RepositoryHolder<KT, ET>> CallClient<KT, ET>(
              HttpMethod method, Uri uri
            , RepositorySettings options
            , HttpContent content = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpResponseMessage, RepositoryHolder<KT, ET>> mapOut = null
            , Action<HttpResponseMessage, byte[], RepositoryHolder<KT, ET>> deserializer = null)
        {
            var response = new RepositoryHolder<KT, ET>();

            try
            {
                //Create the message
                HttpRequestMessage httpRq = Request(method, uri);
                //Set the headers
                RequestHeadersSet(httpRq);
                //Sets the supported transport mechanisms
                RequestHeadersSetTransport(httpRq);
                //Sets the prefer headers
                RequestHeadersPreferSet(httpRq, options?.Prefer);
                //Sets the authentication.
                RequestHeadersAuth(httpRq);
                //Any manual adjustments.
                adjustIn?.Invoke(httpRq);

                //Sets the binary content to the request.
                if (content != null)
                    httpRq.Content = content;
                //Set the Http client or override.
                var client = ClientOverride ?? new HttpClient(mHandler);

                //Executes the request to the remote header.
                var httpRs = await client.SendAsync(httpRq);

                //Processes any response headers.
                ResponseHeadersAuth(httpRq, httpRs);

                //OK, set the response content if set
                if (httpRs.Content != null && httpRs.Content.Headers.ContentLength > 0)
                {
                    byte[] httpRsContent = await httpRs.Content.ReadAsByteArrayAsync();

                    if (httpRs.IsSuccessStatusCode)
                        deserializer?.Invoke(httpRs, httpRsContent, response);
                    else
                        // So that we can see error messages such as schema validation fail
                        response.ResponseMessage = Encoding.UTF8.GetString(httpRsContent);
                }

                //Get any outgoing trace headers and set them in to the response.
                //IEnumerable<string> trace;
                //if (httpRs.Headers.TryGetValues(ApimConstants.AzureTraceHeaderLocation, out trace))
                //    response.Settings.Prefer.Add(ApimConstants.AzureTraceHeaderLocation, trace.First());

                //Set the HTTP Response code.
                response.ResponseCode = (int)httpRs.StatusCode;

                //Maps any additional properties to the response.
                mapOut?.Invoke(httpRs, response);
            }
            catch (Exception ex)
            {
                response.ResponseMessage = FormatExceptionChain(ex);
                response.ResponseCode = 503;
            }

            return response;
        }
        #endregion

    }
}
