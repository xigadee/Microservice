#region using
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http.Filters;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the base event class for the API Boundary Event logger.
    /// </summary>
    public class ApiBoundaryEvent: BoundaryEvent
    {
        private readonly HttpActionExecutedContext mContext;

        /// <summary>
        /// This is the base constructor.
        /// </summary>
        /// <param name="context">This is the API context.</param>
        /// <param name="level">This enumeration determines which part of the data should be logged.</param>
        public ApiBoundaryEvent(HttpActionExecutedContext context, ApiBoundaryLoggingFilterLevel level)
        {
            Level = level;
            mContext = context;
            Direction = ChannelDirection.Incoming;
            Type = BoundaryEventType.Interface;
            Id = Guid.NewGuid();

            if ((level & ApiBoundaryLoggingFilterLevel.Exception) > 0)
                Ex = context.Exception;

            if ((level & ApiBoundaryLoggingFilterLevel.Request) > 0)
                Request = new ApiHttpRequestWrapper(context);

            if ((level & ApiBoundaryLoggingFilterLevel.Response) > 0)
                Response = new ApiHttpResponseWrapper(context);

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
        protected HttpActionExecutedContext mContext;

        protected ApiBoundaryWrapperBase(HttpActionExecutedContext context)
        {
            mContext = context;
        }
    }

    public class ApiHttpRequestWrapper: ApiBoundaryWrapperBase
    {
        public ApiHttpRequestWrapper(HttpActionExecutedContext context):base(context){}

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
    /// <summary>
    /// This is the HttpResponse wrapper class.
    /// </summary>
    public class ApiHttpResponseWrapper: ApiBoundaryWrapperBase
    {
        public ApiHttpResponseWrapper(HttpActionExecutedContext context):base(context){}

        public HttpResponseHeaders Headers => mContext.Response.Headers;

        public HttpContentHeaders ContentHeaders => mContext.Response.Content?.Headers;

        public HttpStatusCode StatusCode => mContext.Response.StatusCode;
    }

    #endregion

}
