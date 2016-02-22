#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
#endregion
namespace Xigadee
{
    public class ApiResponse : IHttpActionResult
    {
        public ApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public byte[] Data { get; set; }

        public string ContentId { get; set; }

        public string VersionId { get; set; }

        public string MediaType { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = StatusCode
            };

            if (Data != null)
            {
                response.Content = new ByteArrayContent(Data);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
            }

            if (ContentId != null)
                response.Headers.Add("X-IMG-ContentId", ContentId);

            if (VersionId != null)
                response.Headers.Add("X-IMG-VersionId", VersionId);

            return Task.FromResult(response);
        }
    }
}
