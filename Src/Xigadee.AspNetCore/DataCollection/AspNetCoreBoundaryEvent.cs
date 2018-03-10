using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Xigadee
{
    /// <summary>
    /// This is the base event class for the API Boundary Event logger.
    /// </summary>
    public class AspNetCoreBoundaryEvent: BoundaryEvent
    {
        private readonly HttpContext mContext;

        /// <summary>
        /// This is the base constructor.
        /// </summary>
        /// <param name="context">This is the API context.</param>
        /// <param name="level">This enumeration determines which part of the data should be logged.</param>
        public AspNetCoreBoundaryEvent(HttpContext context, ApiBoundaryLoggingFilterLevel level, Exception logEx)
        {
            Level = level;
            mContext = context;
            Direction = ChannelDirection.Incoming;
            Type = BoundaryEventType.Interface;
            Id = Guid.NewGuid();
            Ex = logEx;

            //if ((level & ApiBoundaryLoggingFilterLevel.Exception) > 0)
            //    Ex = context.Exception;

            //var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            if ((level & ApiBoundaryLoggingFilterLevel.Request) > 0)
                Request = new ApiHttpRequestWrapper(context);

            if ((level & ApiBoundaryLoggingFilterLevel.Response) > 0)
                Response = new ApiHttpResponseWrapper(context);

            //if ((level & ApiBoundaryLoggingFilterLevel.RequestContent) > 0)
            //    RequestBody = new ApiMimeContent(context.Request.);

            //if ((level & ApiBoundaryLoggingFilterLevel.ResponseContent) > 0)
            //    ResponseBody = new ApiMimeContent(context.Response);
        }

        public string TraceIdentifier => mContext.TraceIdentifier;

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
        public ApiHttpRequestWrapper Request { get; }
        /// <summary>
        /// This is the request body.
        /// </summary>
        public ApiMimeContent RequestBody { get; }
        /// <summary>
        /// This is the http response.
        /// </summary>
        public ApiHttpResponseWrapper Response { get; }
        /// <summary>
        /// This is the response body.
        /// </summary>
        public ApiMimeContent ResponseBody { get; }
        /// <summary>
        /// This is the correlation id.
        /// </summary>
        public string CorrelationId { get; set; }
    }



    #region Request Wrapper

    public abstract class ApiBoundaryWrapperBase
    {
        protected HttpContext mContext;

        protected ApiBoundaryWrapperBase(HttpContext context)
        {
            mContext = context;
        }
    }

    public class ApiHttpRequestWrapper: ApiBoundaryWrapperBase
    {
        public ApiHttpRequestWrapper(HttpContext context) : base(context) { }

        //public HttpRequestHeaders Headers => mContext.Request.Headers;

        //public HttpContentHeaders ContentHeaders => mContext.Request.

        public string Method => mContext.Request.Method;

        public string RequestUri => mContext.Request.GetEncodedUrl();

        public IIdentity Identity => mContext.User?.Identity;

        public string ClientIPAddress
        {
            get
            {
                //try
                //{
                //    if (mContext.Request.Properties.ContainsKey("MS_HttpContext"))
                //        return ((HttpContextWrapper)mContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;

                //    if (mContext.Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                //        return ((RemoteEndpointMessageProperty)mContext.Request.Properties[RemoteEndpointMessageProperty.Name]).Address;
                //}
                //catch (Exception)
                //{
                //    return null;
                //}

                return null;
            }
        }

    }

    #endregion

    #region Response Wrapper
    /// <summary>
    /// This is the HttpResponse wrapper class.
    /// </summary>
    public class ApiHttpResponseWrapper: ApiBoundaryWrapperBase
    {
        public ApiHttpResponseWrapper(HttpContext context) : base(context) { }

        //public HttpResponseHeaders Headers => mContext.Response.Headers;

        //public HttpContentHeaders ContentHeaders => mContext.Response.Content?.Headers;

        public int StatusCode => mContext.Response.StatusCode;
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
                // Do not cause the application to throw an exception due to logging failure
            }
        }
        /// <summary>
        /// This is the payload content type.
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// This is the binary payload body.
        /// </summary>
        public byte[] Body { get; }
    }
}
