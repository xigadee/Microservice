#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Linq;
#endregion

namespace Xigadee
{
    /// <summary>
    /// This is the base event class for the Api Boundary Event logger.
    /// </summary>
    public class ApiBoundaryEvent: BoundaryEvent
    {
        private readonly HttpActionExecutedContext mContext;

        /// <summary>
        /// This is the base constructor.
        /// </summary>
        /// <param name="context">This is the api context.</param>
        /// <param name="direction">The event direction.</param>
        /// <param name="level">This enumeration determines which part of the data should be logged.</param>
        public ApiBoundaryEvent(HttpActionExecutedContext context, ChannelDirection direction, ApiBoundaryLoggingFilterLevel level)
        {
            Level = level;
            mContext = context;
            Direction = direction;
            Type = BoundaryEventType.Boundary;
            Id = Guid.NewGuid();

            if ((level & ApiBoundaryLoggingFilterLevel.Exception) > 0)
                Ex = context.Exception;

            if ((level & ApiBoundaryLoggingFilterLevel.Request) > 0)
                Request = new HttpRequestWrapper(context);

            if ((level & ApiBoundaryLoggingFilterLevel.Response) > 0)
                Response = new HttpResponseWrapper(context);

            if ((level & ApiBoundaryLoggingFilterLevel.RequestContent) > 0)
                RequestBody = new ApiMimeContent(context.Request.Content);

            if ((level & ApiBoundaryLoggingFilterLevel.ResponseContent) > 0)
                ResponseBody = new ApiMimeContent(context.Response.Content);
        }

        /// <summary>
        /// This is the log level set.
        /// </summary>
        public ApiBoundaryLoggingFilterLevel Level { get; }

        /// <summary>
        /// This is the UTC timestamp for the event.
        /// </summary>
        public DateTime TimeStamp => DateTime.UtcNow;

        /// <summary>
        /// This is the http request.
        /// </summary>
        public HttpRequestWrapper Request { get; }
        /// <summary>
        /// This is the request body.
        /// </summary>
        public ApiMimeContent RequestBody { get; }
        /// <summary>
        /// This is the http response.
        /// </summary>
        public HttpResponseWrapper Response { get; }
        /// <summary>
        /// This is the response body.
        /// </summary>
        public ApiMimeContent ResponseBody { get; }
        /// <summary>
        /// This is the correlation id.
        /// </summary>
        public string CorrelationId { get; set; }

    }

    #region LoggingFilterLevel
    /// <summary>
    /// This is the logging level.
    /// </summary>
    [Flags]
    public enum ApiBoundaryLoggingFilterLevel
    {
        /// <summary>
        /// No logging of any information.
        /// </summary>
        None = 0,

        Exception = 1,
        Request = 2,
        Response = 4,
        RequestContent = 8,
        ResponseContent = 16,

        All = 31
    }
    #endregion

    #region Request Wrapper

    public class WrapperBase
    {
        protected HttpActionExecutedContext mContext;

        public WrapperBase(HttpActionExecutedContext context)
        {
            mContext = context;
        }
    }

    public class HttpRequestWrapper: WrapperBase
    {
        public HttpRequestWrapper(HttpActionExecutedContext context):base(context){}

        public HttpRequestHeaders Headers => mContext.Request.Headers;

        public HttpContentHeaders ContentHeaders => mContext.Request.Content?.Headers;

        public HttpMethod Method => mContext.Request.Method;

        public Uri RequestUri => mContext.Request.RequestUri;

        public IIdentity Identity => mContext.ActionContext.RequestContext?.Principal?.Identity;

        public string ClientIPAddress
        {
            get
            {
                try
                {
                    if (mContext.Request.Properties.ContainsKey("MS_HttpContext"))
                        return ((HttpContextWrapper)mContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;

                    if (mContext.Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                        return ((RemoteEndpointMessageProperty)mContext.Request.Properties[RemoteEndpointMessageProperty.Name]).Address;
                }
                catch (Exception)
                {
                    return null;
                }

                return null;
            }
        }

    }

    #endregion

    #region Response Wrapper

    public class HttpResponseWrapper: WrapperBase
    {
        public HttpResponseWrapper(HttpActionExecutedContext context):base(context){}

        public HttpResponseHeaders Headers => mContext.Response.Headers;

        public HttpContentHeaders ContentHeaders => mContext.Response.Content?.Headers;

        public HttpStatusCode StatusCode => mContext.Response.StatusCode;
    }

    #endregion

    /// <summary>
    /// This class holds the incoming and outgoing content.
    /// </summary>
    public class ApiMimeContent
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="content">The http content.</param>
        public ApiMimeContent(HttpContent content)
        {
            if (content == null || content.Headers.ContentLength == 0)
                return;

            IEnumerable<string> contentTypes;
            if (content.Headers.TryGetValues("Content-Type", out contentTypes))
                ContentType = contentTypes.FirstOrDefault();

            try
            {
                Body = content.ReadAsByteArrayAsync().Result;
            }
            catch (Exception)
            {
                // Do not cause application to throw an exception due to logging failure
            }
        }
        /// <summary>
        /// This is the payload content type.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// This is the payload body.
        /// </summary>
        public byte[] Body { get; }
    }
}
