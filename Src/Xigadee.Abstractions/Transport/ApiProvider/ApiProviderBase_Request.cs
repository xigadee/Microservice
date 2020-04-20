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
        protected virtual void RequestHeadersSet(HttpRequestMessage rq)
        {
            if (!string.IsNullOrEmpty(Context.UserAgent))
                rq.Headers.Add("User-Agent", Context.UserAgent);
            if (!string.IsNullOrEmpty(Context.AssemblyVersion))
                rq.Headers.Add("x-api-clientversion", Context.AssemblyVersion);
            if (!string.IsNullOrEmpty(Context.ApiVersion))
                rq.Headers.Add("x-api-version", Context.ApiVersion);
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
            Context.AuthHandlers?.ForEach((a) => a.ProcessRequest(rq));
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
    }
}
