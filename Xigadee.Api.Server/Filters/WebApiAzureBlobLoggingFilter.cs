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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class logs the incoming request and subsequent response to the Azure Storage container.
    /// </summary>
    public class WebApiAzureBlobLoggingFilter : ActionFilterAttribute
    {
        #region LoggingFilterLevel
        /// <summary>
        /// This is the logging level.
        /// </summary>
        [Flags]
        public enum LoggingFilterLevel
        {
            None = 0,

            Exception = 1,
            Request = 2,
            Response = 4,
            RequestContent = 8,
            ResponseContent = 16,

            All = 31
        } 
        #endregion
        #region Declarations
        protected CloudStorageAccount mStorageAccount;
        protected CloudBlobClient mStorageClient;
        protected CloudBlobContainer mEntityContainer;

        protected readonly string mCorrelationIdKeyName;
        protected readonly LoggingFilterLevel mLevel;
        #endregion
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="containerName"></param>
        /// <param name="correlationIdKeyName"></param>
        /// <param name="level"></param>
        public WebApiAzureBlobLoggingFilter(StorageCredentials credentials, string containerName
            , string correlationIdKeyName = "X-CorrelationId", LoggingFilterLevel level = LoggingFilterLevel.All)
        {
            mCorrelationIdKeyName = correlationIdKeyName;
            mLevel = level;

            mStorageAccount = new CloudStorageAccount(credentials, true);
            mStorageClient = mStorageAccount.CreateCloudBlobClient();
            mEntityContainer = mStorageClient.GetContainerReference(containerName.ToLowerInvariant()); // Containers names must be lowercase
            var result = mEntityContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null).Result;
        }
        #endregion
        #region CorrelationIdGet()
        /// <summary>
        /// This method creates the correlation id.
        /// </summary>
        /// <returns>A unique string.</returns>
        protected virtual string CorrelationIdGet()
        {
            return Guid.NewGuid().ToString("N").ToUpperInvariant();
        } 
        #endregion

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var request = actionContext.Request;
                IEnumerable<string> correlationValues;
                var correlationId = CorrelationIdGet();
                if (!request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
                    actionContext.Request.Headers.Add(mCorrelationIdKeyName, correlationId);

                var apiRequest = actionContext.ActionArguments.Values.FirstOrDefault(
                        aa => aa != null && aa.GetType() == typeof (ApiRequest)) as ApiRequest;

                if (apiRequest != null && apiRequest.Options != null)
                    apiRequest.Options.CorrelationId = correlationId;
            }
            catch (Exception)
            {
                // Don't prevent normal operation of the site where there is an exception
            }

            base.OnActionExecuting(actionContext);
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>
            {
                base.OnActionExecutedAsync(actionExecutedContext, cancellationToken)
            };

            if (mLevel > LoggingFilterLevel.None)
            {
                var request = actionExecutedContext.Response.RequestMessage;
                var response = actionExecutedContext.Response;
                var exception = actionExecutedContext.Exception;
                var principal = actionExecutedContext.ActionContext.RequestContext.Principal;
                var folder = DateTime.UtcNow.ToString("yyyy-MM-dd/HH/mm");

                // Retrieve the correlation id from the request and add to the response
                IEnumerable<string> correlationValues;
                string correlationId = null;
                if (request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
                    correlationId = correlationValues.FirstOrDefault();

                if (!string.IsNullOrEmpty(correlationId))
                    response.Headers.Add(mCorrelationIdKeyName, correlationId);

                var refDirectory = mEntityContainer.GetDirectoryReference(folder);
                var refEntityDirectory =
                    refDirectory.GetDirectoryReference(FormatDirectoryName(correlationId, principal, response));

                if ((mLevel & LoggingFilterLevel.Exception) > 0 && exception != null)
                    tasks.Add(UploadBlob(refEntityDirectory, exception,
                        string.Format("{0}.exception.json", correlationId), cancellationToken));

                if ((mLevel & LoggingFilterLevel.Request) > 0)
                    tasks.Add(UploadBlob(refEntityDirectory, new HttpRequestWrapper(request, principal),
                        string.Format("{0}.request.json", correlationId), cancellationToken));

                if ((mLevel & LoggingFilterLevel.Response) > 0)
                    tasks.Add(UploadBlob(refEntityDirectory, new HttpResponseWrapper(response),
                        string.Format("{0}.response.json", correlationId), cancellationToken));

                if ((mLevel & LoggingFilterLevel.RequestContent) > 0)
                    tasks.Add(UploadContentBlob(refEntityDirectory, request.Content,
                        string.Format("{0}.request.content", correlationId), cancellationToken));

                if ((mLevel & LoggingFilterLevel.ResponseContent) > 0)
                    tasks.Add(UploadContentBlob(refEntityDirectory, response.Content,
                        string.Format("{0}.response.content", correlationId), cancellationToken));

            }

            await Task.WhenAll(tasks);
        }

        private async Task UploadBlob(CloudBlobDirectory dir, object entity, string blobName, CancellationToken cancellationToken)
        {
            if (entity == null)
                return;

            try
            {
                var jObj = JObject.FromObject(entity);
                var blob = dir.GetBlockBlobReference(blobName);
                blob.Properties.ContentType = "application/json";
                await blob.UploadTextAsync(jObj.ToString(), cancellationToken);
            }
            catch(Exception)
            {
                // Do not cause application to throw an exception due to logging failure
            }
        }

        private async Task UploadContentBlob(CloudBlobDirectory dir, HttpContent content, string blobName, CancellationToken cancellationToken)
        {
            if (content == null || content.Headers.ContentLength == 0)
                return;

            IEnumerable<string> contentTypes;
            string contentType = null;
            if (content.Headers.TryGetValues("Content-Type", out contentTypes))
                contentType = contentTypes.FirstOrDefault();

            try
            {
                var blob = dir.GetBlockBlobReference(blobName);
                blob.Properties.ContentType = contentType ?? "text/plain";
                await blob.UploadFromStreamAsync(await content.ReadAsStreamAsync(), cancellationToken);
            }
            catch (Exception)
            {
                // Do not cause application to throw an exception due to logging failure
            }
        }

        private static string FormatDirectoryName(string correlationId, IPrincipal principal, HttpResponseMessage responseMessage)
        {
            var directoryName = string.Empty;
            if (principal != null)
            {
                var apimIdentity = principal.Identity as ApimIdentity;
                if (apimIdentity != null && !string.IsNullOrEmpty(apimIdentity.Source))
                    directoryName = string.Format("{0}.", apimIdentity.Source);
            }

            return string.Format("{0}/{1}.{2}", directoryName,
                responseMessage != null ? (int) responseMessage.StatusCode : 0, correlationId);
        }

        #region Request Wrapper

        private class HttpRequestWrapper
        {
            private readonly HttpRequestMessage mRequestMessage;
            private readonly IPrincipal mRequestPrincipal;

            public HttpRequestHeaders Headers
            {
                get { return mRequestMessage.Headers; }                
            }

            public HttpContentHeaders ContentHeaders
            {
                get { return mRequestMessage.Content == null ? null : mRequestMessage.Content.Headers; }
            }

            public HttpMethod Method
            {
                get { return mRequestMessage.Method; }
            }

            public Uri RequestUri
            {
                get { return mRequestMessage.RequestUri; }
            }

            public IIdentity Identity
            {
                get { return mRequestPrincipal == null ? null : mRequestPrincipal.Identity; }
            }

            public string ClientIPAddress
            {
                get
                {
                    try
                    {
                        if (mRequestMessage.Properties.ContainsKey("MS_HttpContext"))
                            return ((HttpContextWrapper)mRequestMessage.Properties["MS_HttpContext"]).Request.UserHostAddress;

                        if (mRequestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                            return ((RemoteEndpointMessageProperty)mRequestMessage.Properties[RemoteEndpointMessageProperty.Name]).Address;
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    return null;
                }
            }

            public HttpRequestWrapper(HttpRequestMessage requestMessage, IPrincipal requestPrincipal)
            {
                mRequestMessage = requestMessage;
                mRequestPrincipal = requestPrincipal;
            }
        }

        #endregion

        #region Response Wrapper

        private class HttpResponseWrapper
        {
            private readonly HttpResponseMessage mResponseMessage;

            public HttpResponseHeaders Headers
            {
                get { return mResponseMessage.Headers; }
            }

            public HttpContentHeaders ContentHeaders
            {
                get { return mResponseMessage.Content == null ? null : mResponseMessage.Content.Headers; }
            }


            public HttpStatusCode StatusCode
            {
                get { return mResponseMessage.StatusCode; }
            }

            public HttpResponseWrapper(HttpResponseMessage responseMessage)
            {
                mResponseMessage = responseMessage;
            }
        }

        #endregion
    }
}
