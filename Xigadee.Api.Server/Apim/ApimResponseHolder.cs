#region using
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
#endregion
namespace Xigadee.Api
{
    public class ApimResponseHolder
    {
        public ApimResponseHolder()
        {
            Fields = new Dictionary<string, string>();
        }

        public bool IsSuccess { get; set; }

        public bool IsTimeout { get; set; }

        public HttpResponseMessage Response { get; set; }

        public string Content { get; set; }

        public Exception Ex { get; set; }

        public string DocumentId { get; set; }

        public string ETag { get; set; }

        public Dictionary<string, string> Fields { get; set; }
    }
}
