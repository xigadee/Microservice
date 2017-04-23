using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Xigadee
{
    /// <summary>
    /// This class is used to assign the status.
    /// </summary>
    public class StatusResult : IHttpActionResult
    {
        public StatusResult(HttpStatusCode status, HttpRequestMessage request, string reasonPhrase = null)
        {
            Status = status;
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public HttpStatusCode Status { get; private set; }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(Status);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase ?? Enum.GetName(typeof(HttpStatusCode), Status);
            return response;
        }
    }
}
