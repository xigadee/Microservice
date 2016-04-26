#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public class ResponseHolderBase: IResponseHolder
    {
        public ResponseHolderBase()
        {
            Fields = new Dictionary<string, string>();
        }

        public bool IsSuccess { get; set; }

        public bool IsTimeout { get; set; }

        public bool IsCacheHit { get; set; }

        public virtual int StatusCode { get; set; }

        public virtual string StatusMessage { get; set; }

        public string Id { get; set; }

        public string VersionId { get; set; }

        public string Content { get; set; }

        public Exception Ex { get; set; }

        public Dictionary<string, string> Fields { get; set; }
    }

    public class ResponseHolder: ResponseHolderBase
    {
        public bool IsThrottled { get; set; }

        public bool ThrottleHasWaited { get; set; }

        public long ResourceCharge { get; set; }

        public TimeSpan? ThrottleSuggestedWait { get; set; }

        public HttpResponseMessage Response { get; set; }

        public override int StatusCode { get { return (Response != null?(int)Response.StatusCode: 0); } }

        public string DocumentId { get; set; }

        public string ETag { get; set; }

        public string ContinuationToken { get; set; }

        public string SessionToken { get; set; }
    }

    public class ResponseHolder<O> : ResponseHolder, IResponseHolder<O>
        where O:class
    {
        public O Entity { get; set; }
    }


}
