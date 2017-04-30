//#region Copyright
//// Copyright Hitachi Consulting
//// 
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// 
////    http://www.apache.org/licenses/LICENSE-2.0
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//#endregion

//#region using
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Principal;
//using System.ServiceModel.Channels;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.WindowsAzure.Storage.Blob;
//using Newtonsoft.Json.Linq;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This class logs the incoming API requests and subsequent responses to the Azure Storage container.
//    /// </summary>
//    [Obsolete("Use the new boundary logging filter.")]
//    public class WebApiAzureBlobLoggingFilter : WebApiCorrelationIdFilter
//    {
//        #region LoggingFilterLevel
//        /// <summary>
//        /// This is the logging level.
//        /// </summary>
//        [Flags]
//        public enum LoggingFilterLevel
//        {
//            /// <summary>
//            /// No logging of any information.
//            /// </summary>
//            None = 0,

//            Exception = 1,
//            Request = 2,
//            Response = 4,
//            RequestContent = 8,
//            ResponseContent = 16,

//            All = 31
//        } 
//        #endregion
//        #region Declarations
//        protected CloudStorageAccount mStorageAccount;
//        protected CloudBlobClient mStorageClient;
//        protected CloudBlobContainer mEntityContainer;
//        protected readonly LoggingFilterLevel mLevel;
//        #endregion
//        #region Constructor
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="credentials"></param>
//        /// <param name="containerName"></param>
//        /// <param name="correlationIdKeyName"></param>
//        /// <param name="level"></param>
//        public WebApiAzureBlobLoggingFilter(StorageCredentials credentials, string containerName
//            , string correlationIdKeyName = "X-CorrelationId"
//            , LoggingFilterLevel level = LoggingFilterLevel.All):base(correlationIdKeyName)
//        {
//            mLevel = level;

//            mStorageAccount = new CloudStorageAccount(credentials, true);
//            mStorageClient = mStorageAccount.CreateCloudBlobClient();
//            mEntityContainer = mStorageClient.GetContainerReference(containerName.ToLowerInvariant()); // Containers names must be lowercase
//            var result = mEntityContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null).Result;
//        }
//        #endregion


//        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
//        {
//            var tasks = new List<Task>
//            {
//                base.OnActionExecutedAsync(actionExecutedContext, cancellationToken)
//            };

//            if (mLevel > LoggingFilterLevel.None)
//            {
//                var request = actionExecutedContext.Response.RequestMessage;
//                var response = actionExecutedContext.Response;
//                var exception = actionExecutedContext.Exception;
//                var principal = actionExecutedContext.ActionContext.RequestContext.Principal;
//                var folder = DateTime.UtcNow.ToString("yyyy-MM-dd/HH/mm");

//                // Retrieve the correlation id from the request and add to the response
//                IEnumerable<string> correlationValues;
//                string correlationId = null;
//                if (request.Headers.TryGetValues(mCorrelationIdKeyName, out correlationValues))
//                    correlationId = correlationValues.FirstOrDefault();

//                if (!string.IsNullOrEmpty(correlationId))
//                    response.Headers.Add(mCorrelationIdKeyName, correlationId);

//                var refDirectory = mEntityContainer.GetDirectoryReference(folder);
//                var refEntityDirectory =
//                    refDirectory.GetDirectoryReference(FormatDirectoryName(correlationId, principal, request.Method, response));

//                if ((mLevel & LoggingFilterLevel.Exception) > 0 && exception != null)
//                    tasks.Add(UploadBlob(refEntityDirectory, exception, $"{correlationId}.exception.json", cancellationToken));

//                if ((mLevel & LoggingFilterLevel.Request) > 0)
//                    tasks.Add(UploadBlob(refEntityDirectory, new HttpRequestWrapper(request, principal),
//                        $"{correlationId}.request.json", cancellationToken));

//                if ((mLevel & LoggingFilterLevel.Response) > 0)
//                    tasks.Add(UploadBlob(refEntityDirectory, new HttpResponseWrapper(response),
//                        $"{correlationId}.response.json", cancellationToken));

//                if ((mLevel & LoggingFilterLevel.RequestContent) > 0)
//                    tasks.Add(UploadContentBlob(refEntityDirectory, request.Content,
//                        $"{correlationId}.request.content", cancellationToken));

//                if ((mLevel & LoggingFilterLevel.ResponseContent) > 0)
//                    tasks.Add(UploadContentBlob(refEntityDirectory, response.Content,
//                        $"{correlationId}.response.content", cancellationToken));
//            }

//            await Task.WhenAll(tasks);
//        }

//        private async Task UploadBlob(CloudBlobDirectory dir, object entity, string blobName, CancellationToken cancellationToken)
//        {
//            if (entity == null)
//                return;

//            try
//            {
//                var jObj = JObject.FromObject(entity);
//                var blob = dir.GetBlockBlobReference(blobName);
//                blob.Properties.ContentType = "application/json";
//                await blob.UploadTextAsync(jObj.ToString(), cancellationToken);
//            }
//            catch(Exception)
//            {
//                // Do not cause application to throw an exception due to logging failure
//            }
//        }

//        private async Task UploadContentBlob(CloudBlobDirectory dir, HttpContent content, string blobName, CancellationToken cancellationToken)
//        {
//            if (content == null || content.Headers.ContentLength == 0)
//                return;

//            IEnumerable<string> contentTypes;
//            string contentType = null;
//            if (content.Headers.TryGetValues("Content-Type", out contentTypes))
//                contentType = contentTypes.FirstOrDefault();

//            try
//            {
//                var blob = dir.GetBlockBlobReference(blobName);
//                blob.Properties.ContentType = contentType ?? "text/plain";
//                await blob.UploadFromStreamAsync(await content.ReadAsStreamAsync(), cancellationToken);
//            }
//            catch (Exception)
//            {
//                // Do not cause application to throw an exception due to logging failure
//            }
//        }

//        private static string FormatDirectoryName(string correlationId, IPrincipal principal, HttpMethod requestMethod, HttpResponseMessage responseMessage)
//        {
//            var directoryName = "UNKNOWN";
//            if (principal != null)
//            {
//                var apimIdentity = principal.Identity as ApimIdentity;
//                if (!string.IsNullOrEmpty(apimIdentity?.Source))
//                    directoryName = $"{apimIdentity.Source}.";
//            }

//            // CMS/GET.200.05.04785AB98F8843C7BC972F27CF1E5C68
//            return $"{directoryName}/{requestMethod}.{(responseMessage != null ? (int) responseMessage.StatusCode : 0)}.{DateTime.UtcNow.ToString("ss")}.{correlationId}";
//        }

//        #region Request Wrapper

//        private class HttpRequestWrapper
//        {
//            private readonly HttpRequestMessage mRequestMessage;
//            private readonly IPrincipal mRequestPrincipal;

//            public HttpRequestHeaders Headers => mRequestMessage.Headers;

//            public HttpContentHeaders ContentHeaders => mRequestMessage.Content?.Headers;

//            public HttpMethod Method => mRequestMessage.Method;

//            public Uri RequestUri => mRequestMessage.RequestUri;

//            public IIdentity Identity => mRequestPrincipal?.Identity;

//            public string ClientIPAddress
//            {
//                get
//                {
//                    try
//                    {
//                        if (mRequestMessage.Properties.ContainsKey("MS_HttpContext"))
//                            return ((HttpContextWrapper)mRequestMessage.Properties["MS_HttpContext"]).Request.UserHostAddress;

//                        if (mRequestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
//                            return ((RemoteEndpointMessageProperty)mRequestMessage.Properties[RemoteEndpointMessageProperty.Name]).Address;
//                    }
//                    catch (Exception)
//                    {
//                        return null;
//                    }

//                    return null;
//                }
//            }

//            public HttpRequestWrapper(HttpRequestMessage requestMessage, IPrincipal requestPrincipal)
//            {
//                mRequestMessage = requestMessage;
//                mRequestPrincipal = requestPrincipal;
//            }
//        }

//        #endregion

//        #region Response Wrapper

//        private class HttpResponseWrapper
//        {
//            private readonly HttpResponseMessage mResponseMessage;

//            public HttpResponseHeaders Headers => mResponseMessage.Headers;

//            public HttpContentHeaders ContentHeaders => mResponseMessage.Content?.Headers;


//            public HttpStatusCode StatusCode => mResponseMessage.StatusCode;

//            public HttpResponseWrapper(HttpResponseMessage responseMessage)
//            {
//                mResponseMessage = responseMessage;
//            }
//        }

//        #endregion
//    }
//}
