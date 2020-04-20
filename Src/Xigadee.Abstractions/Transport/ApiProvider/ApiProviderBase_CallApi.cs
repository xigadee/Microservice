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
    public abstract partial class ApiProviderBase
    {
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

                await ProcessResponse(response, httpRs, responsemodelDeserializer, errorObjectDeserializer ?? DefaultErrorObjectDeserializer);

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
        protected virtual async Task<R> CallApiClient<O, R>(
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

                await ProcessResponse(response, httpRs, responsemodelDeserializer, errorObjectDeserializer ?? DefaultErrorObjectDeserializer);

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
            where O : class
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
            var httpRs = await Context.Client.SendAsync(httpRq);

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
        protected static async Task ProcessResponse<O, R>(R response
            , HttpResponseMessage httpRs
            , Func<HttpContent, Task<O>> objectDeserializer
            , Func<HttpContent, Task<object>> errorObjectDeserializer = null)
            where O : class
            where R : ApiResponse<O>, new()
        {
            //Set the HTTP Response code.
            response.ResponseCode = (int)httpRs.StatusCode;
            response.ResponseMessage = httpRs.ReasonPhrase;

            if (httpRs.HasContent())
            {
                if ((response.IsException || response.IsFailure)) //Error path
                {
                    if (errorObjectDeserializer != null)
                        response.ErrorObject = await errorObjectDeserializer(httpRs.Content);
                    else
                    {
                        var err = new ApiResponse.ErrorObjectDefault();
                        err.Blob = await httpRs.Content.ReadAsByteArrayAsync();
                        response.ErrorObject = err;
                    }
                }
                else //Success path
                {
                    if (objectDeserializer == null)
                        objectDeserializer = (c) => c.FromJsonUTF8<O>();

                    response.Entity = await objectDeserializer(httpRs.Content);
                }
            }
        }
        #endregion
    }
}
