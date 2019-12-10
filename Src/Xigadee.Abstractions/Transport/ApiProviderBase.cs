using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class used for API client connectivity.
    /// </summary>
    public abstract class ApiProviderBase
    {
        #region Declarations
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
        /// This is the primary transport used for sending requests.
        /// </summary>
        protected string mTransportOutDefault;
        /// <summary>
        /// This is the collection of transports available for serialization.
        /// </summary>
        protected readonly Dictionary<string, TransportSerializer> mTransportSerializers;
        #endregion
        #region Constructor

        protected ApiProviderBase(ApiProviderBase parent)
        {


        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        protected ApiProviderBase(Uri uri
            , IEnumerable<IApiProviderAuthBase> authHandlers = null
            , X509Certificate clientCert = null
            , Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> manualCertValidation = null
            , IEnumerable<TransportSerializer> transportOverride = null
            )
        {
            // Get the types assembly version to add to the request headers
            mAuthHandlers = authHandlers?.ToList() ?? new List<IApiProviderAuthBase>();

            mUri = uri ?? throw new ArgumentNullException("uri");

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

            if (transportOverride == null || transportOverride.Count() == 0)
            {
                mTransportSerializers = new Dictionary<string, TransportSerializer>();
                var defaultTs = new JsonTransportSerializer();
                mTransportOutDefault = defaultTs.MediaType.ToLowerInvariant();
                mTransportSerializers[mTransportOutDefault] = defaultTs;
            }
            else
            {
                mTransportOutDefault = transportOverride.First().MediaType.ToLowerInvariant();
                mTransportSerializers = transportOverride.ToDictionary(t => t.MediaType.ToLowerInvariant(), t => t);
            }
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
        /// method to change the version.
        /// </summary>
        /// <returns>Returns a string containing the assembly version.</returns>
        protected virtual string AssemblyVersionGet()
        {
            return GetType().Assembly.GetName().Version.ToString();
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
        #region TransportSerializerDefault
        /// <summary>
        /// This is the default serializer.
        /// </summary>
        protected TransportSerializer TransportSerializerDefault => mTransportSerializers[mTransportOutDefault]; 
        #endregion

        #region Client
        /// <summary>
        /// Get a new http client or uses the override.
        /// </summary>
        protected internal HttpClient Client => ClientOverride ?? new HttpClient(mHandler); 
        #endregion
        #region ClientOverride
        /// <summary>
        /// Gets or sets the client override. This can be used for testing. It uses the HttpClient passed instead
        /// or creating a new client for each request.
        /// </summary>
        public HttpClient ClientOverride { get; set; }
        #endregion

        #region Request(HttpMethod verb, Uri uri, HttpContent content = null)
        /// <summary>
        /// This method creates the default request message.
        /// </summary>
        /// <param name="verb">The HTTP verb.</param>
        /// <param name="uri">The Uri request.</param>
        /// <param name="content">The optional Http content request body.</param>
        /// <returns>Returns the message with the full domain request.</returns>
        protected virtual HttpRequestMessage Request(HttpMethod verb, Uri uri, HttpContent content = null)
        {
            HttpRequestMessage rq = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = uri
            };

            //Sets the binary content to the request.
            if (content != null)
                rq.Content = content;

            return rq;
        }
        #endregion
        #region RequestHeadersSet(HttpRequestMessage rq)
        /// <summary>
        /// This virtual method sets the necessary headers for the request.
        /// </summary>
        /// <param name="rq">The http request.</param>
        protected internal virtual void RequestHeadersSet(HttpRequestMessage rq)
        {
            rq.Headers.Add("User-Agent", UserAgentGet());
            rq.Headers.Add("x-api-clientversion", AssemblyVersionGet());
            rq.Headers.Add("x-api-version", "2016-08-01");
        }
        #endregion
        #region RequestHeadersPreferSet(HttpRequestMessage rq, Dictionary<string, string> Prefer)
        /// <summary>
        /// This method sets the prefer request header directives for the Api call.
        /// </summary>
        /// <param name="rq">The http request object.</param>
        /// <param name="Prefer">The prefer collection.</param>
        protected internal virtual void RequestHeadersPreferSet(HttpRequestMessage rq, Dictionary<string, string> Prefer)
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
        protected internal virtual void RequestHeadersAuth(HttpRequestMessage rq)
        {
            mAuthHandlers?.ForEach((a) => a.ProcessRequest(rq));
        }
        #endregion
        #region RequestHeadersSetTransport(HttpRequestMessage rq)
        /// <summary>
        /// This method sets the media quality type for the entity transfer.
        /// </summary>
        /// <param name="rq">The http request.</param>
        protected internal virtual void RequestHeadersSetTransport(HttpRequestMessage rq)
        {
            rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
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
            var end = string.Concat(mUri.LocalPath, part).Replace("//", "/");
            var bd = new UriBuilder(mUri.Scheme, mUri.Host, mUri.Port, end);
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

        #region CallApiClient<O, R>
        /// <summary>
        /// This method calls the client using HTTP and returns the response along with the entity in the response if supplied.
        /// </summary>
        /// <typeparam name="O">The response object type.</typeparam>
        /// <typeparam name="R">The response object type.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The request Uri.</param>
        /// <param name="model">The optional model.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="adjustOut">Any message adjustment.</param>
        /// <param name="errorObjectDeserializer">Deserialize the response content into the error object if content is present on failure.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="requestmodelSerializer">The optional model serializer.</param>
        /// <param name="responsemodelDeserializer">The optional deserializer for the response.</param>
        /// <returns>Returns the response wrapper.</returns>
        protected virtual async Task<R> CallApiClient<O, R>(
              HttpMethod method
            , Uri uri
            , object model = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            , Func<HttpContent, Task<object>> errorObjectDeserializer = null
            , Action<HttpResponseMessage, R> mapOut = null
            , Func<object, HttpContent> requestmodelSerializer = null
            , Func<HttpContent, Task<O>> responsemodelDeserializer = null
            )
            where O : class
            where R : ApiResponse<O>, new()
        {
            var response = new R();

            try
            {
                var httpRs = await CallApiClientBase(method, uri, model, adjustIn, adjustOut, requestmodelSerializer);

                await ProcessResponse(response, httpRs, responsemodelDeserializer, errorObjectDeserializer);

                //Maps any additional properties or adjustments to the response.
                mapOut?.Invoke(httpRs, response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 503;
                response.ResponseMessage = ex.AppendInnerExceptions();
            }

            return response;
        }
        /// <summary>
        /// This method calls the client using HTTP and returns the response along with the entity in the response if supplied.
        /// </summary>
        /// <typeparam name="O">The response object type.</typeparam>
        /// <typeparam name="R">The response object type.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The request Uri.</param>
        /// <param name="content">The HttpContent to send to the API.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="adjustOut">Any message adjustment.</param>
        /// <param name="errorObjectDeserializer">Deserialize the response content into the error object if content is present on failure.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="responsemodelDeserializer">The optional deserializer for the response.</param>
        /// <returns>Returns the repository holder.</returns>
        protected virtual async Task<R> CallApiClient<O,R>(
              HttpMethod method
            , Uri uri
            , HttpContent content = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            , Func<HttpContent, Task<object>> errorObjectDeserializer = null
            , Action<HttpResponseMessage, R> mapOut = null
            , Func<HttpContent, Task<O>> responsemodelDeserializer = null
            )
            where O : class
            where R : ApiResponse<O>, new()
        {
            var response = new R();

            try
            {
                var httpRs = await CallApiClientBase(method, uri, content, adjustIn, adjustOut);

                await ProcessResponse(response, httpRs, responsemodelDeserializer, errorObjectDeserializer);

                //Maps any additional properties to the response.
                mapOut?.Invoke(httpRs, response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = 503;
                response.ResponseMessage = ex.AppendInnerExceptions();
            }

            return response;
        }
        #endregion
        #region CallApiClient <O> ...
        /// <summary>
        /// This method calls the client using HTTP and returns the response.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The request Uri.</param>
        /// <param name="model">The optional model.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="adjustOut">Any message adjustment.</param>
        /// <param name="errorObjectDeserializer">Deserialize the response content into the error object if content is present on failure.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="requestmodelSerializer">The optional model serializer.</param>
        /// <returns>Returns an API response wrapper.</returns>
        protected virtual Task<ApiResponse> CallApiClient(
              HttpMethod method
            , Uri uri
            , object model = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            , Func<HttpContent, Task<object>> errorObjectDeserializer = null
            , Action<HttpResponseMessage, ApiResponse> mapOut = null
            , Func<object, HttpContent> requestmodelSerializer = null
            ) 
            =>
            CallApiClient<object, ApiResponse>(method, uri, model, adjustIn, adjustOut, errorObjectDeserializer, mapOut, requestmodelSerializer);

        /// <summary>
        /// This method calls the client using HTTP and returns the response object.
        /// </summary>
        /// <typeparam name="O">The return object type.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="uri">The request Uri.</param>
        /// <param name="model">The optional model.</param>
        /// <param name="adjustIn">Any message adjustment.</param>
        /// <param name="adjustOut">Any message adjustment.</param>
        /// <param name="errorObjectDeserializer">Deserialize the response content into the error object if content is present on failure.</param>
        /// <param name="mapOut">Any response adjustment before returning to the caller.</param>
        /// <param name="requestmodelSerializer">The optional model serializer.</param>
        /// <param name="responsemodelDeserializer"></param>
        /// <returns>Returns an API response wrapper with an entity payload.</returns>
        protected virtual Task<ApiResponse<O>> CallApiClient<O>(
              HttpMethod method
            , Uri uri
            , object model = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            , Func<HttpContent, Task<object>> errorObjectDeserializer = null
            , Action<HttpResponseMessage, ApiResponse<O>> mapOut = null
            , Func<object, HttpContent> requestmodelSerializer = null
            , Func<HttpContent, Task<O>> responsemodelDeserializer = null
            )
            where O:class
            =>
            CallApiClient<O, ApiResponse<O>>(method, uri, model, adjustIn, adjustOut, errorObjectDeserializer, mapOut, requestmodelSerializer, responsemodelDeserializer);


        #endregion

        #region CallApiClientBase ...
        /// <summary>
        /// This is the raw HTTP request method.
        /// </summary>
        /// <param name="httpRq">The http request.</param>
        /// <param name="adjustIn">The optional adjustment input action.</param>
        /// <param name="adjustOut">The optional adjustment output action.</param>
        /// <returns>Returns the Http response.</returns>
        protected virtual async Task<HttpResponseMessage> CallApiClientBase(HttpRequestMessage httpRq
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            )
        {
            //Set the headers
            RequestHeadersSet(httpRq);
            //Sets the supported transport mechanisms
            RequestHeadersSetTransport(httpRq);
            //Sets the authentication.
            RequestHeadersAuth(httpRq);

            //Any manual adjustments.
            adjustIn?.Invoke(httpRq);

            //Executes the request to the remote header.
            var httpRs = await Client.SendAsync(httpRq);

            //Processes any response headers.
            ResponseHeadersAuth(httpRq, httpRs);

            //Process any output extensions.
            adjustOut?.Invoke(httpRq, httpRs);

            return httpRs;
        }

        /// <summary>
        /// This is the raw HTTP request method.
        /// </summary>
        /// <param name="method">The Http method.</param>
        /// <param name="uri">The request uri.</param>
        /// <param name="model">The optional model object to serialize.</param>
        /// <param name="adjustIn">The optional adjustment input action.</param>
        /// <param name="adjustOut">The optional adjustment output action.</param>
        /// <param name="requestModelSerializer">The optional model serializer.</param>
        /// <returns>Returns the Http response.</returns>
        protected virtual Task<HttpResponseMessage> CallApiClientBase(HttpMethod method
            , Uri uri
            , object model = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            , Func<object, HttpContent> requestModelSerializer = null
            ) => CallApiClientBase(method, uri, HttpContentHelper.Convert(model, requestModelSerializer), adjustIn, adjustOut);


        /// <summary>
        /// This is the raw HTTP request method.
        /// </summary>
        /// <param name="method">The Http method.</param>
        /// <param name="uri">The request uri.</param>
        /// <param name="content">The Http content.</param>
        /// <param name="adjustIn">The optional adjustment input action.</param>
        /// <param name="adjustOut">The optional adjustment output action.</param>
        /// <returns>Returns the Http response.</returns>
        protected virtual Task<HttpResponseMessage> CallApiClientBase(HttpMethod method
            , Uri uri
            , HttpContent content = null
            , Action<HttpRequestMessage> adjustIn = null
            , Action<HttpRequestMessage, HttpResponseMessage> adjustOut = null
            ) => CallApiClientBase(Request(method, uri, content), adjustIn, adjustOut);

        #endregion
        #region ProcessResponse ...
        /// <summary>
        /// This method processes the response and sets the payload object.
        /// </summary>
        /// <typeparam name="O">The response type.</typeparam>
        /// <typeparam name="R">The API response holder type.</typeparam>
        /// <param name="response">The response wrapper</param>
        /// <param name="httpRs">The Http Response object.</param>
        /// <param name="objectDeserializer">The optional object entity deserializer.</param>
        /// <param name="errorObjectDeserializer">The optional error object deserializer.</param>
        protected static async Task ProcessResponse<O,R>(R response
            , HttpResponseMessage httpRs
            , Func<HttpContent, Task<O>> objectDeserializer
            , Func<HttpContent, Task<object>> errorObjectDeserializer = null)
            where O : class
            where R: ApiResponse<O>, new()
        {
            //Set the HTTP Response code.
            response.ResponseCode = (int)httpRs.StatusCode;

            if (httpRs.HasContent()) 
            {
                if ((response.IsException || response.IsFailure)) //Error path
                {
                    if (errorObjectDeserializer != null)
                        response.ErrorObject = await errorObjectDeserializer(httpRs.Content);
                    else
                    {
                        byte[] httpRsContent = await httpRs.Content.ReadAsByteArrayAsync();

                        var err = new ApiResponse.ErrorObjectDefault();

                        err.Blob = httpRsContent;

                        response.ErrorObject = err;
                    }
                }
                else //Success path
                {
                    if (objectDeserializer == null)
                        objectDeserializer = (c) => c.FromJsonUTF8<O>();

                    if (response.IsSuccess && httpRs.HasContent())
                        response.Entity = await objectDeserializer(httpRs.Content);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// This class is used to implement specific Api specific sections, while inheriting the main settings from the parent.
    /// </summary>
    public abstract class ApiProviderChildBase: ApiProviderBase
    {
        protected ApiProviderBase _parent;

        public ApiProviderChildBase(ApiProviderBase parent):base(parent)
        {
            _parent = parent;
        }

    }
}
