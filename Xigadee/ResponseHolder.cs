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
    public class ResponseHolder
    {
        public ResponseHolder()
        {
            Fields = new Dictionary<string, string>();
        }

        public bool IsSuccess { get; set; }

        public bool IsTimeout { get; set; }

        public bool IsThrottled { get; set; }

        public bool ThrottleHasWaited { get; set; }

        public long ResourceCharge { get; set; }

        public TimeSpan? ThrottleSuggestedWait { get; set; }

        public HttpResponseMessage Response { get; set; }

        public string Id { get; set; }

        public string Content { get; set; }

        public Exception Ex { get; set; }

        public string DocumentId { get; set; }

        public string ETag { get; set; }

        public string ContinuationToken { get; set; }

        public string SessionToken { get; set; }

        public Dictionary<string, string> Fields { get; set; }
    }

    public class ResponseHolder<O> : ResponseHolder
        where O:class
    {
        public O Entity { get; set; }
    }


}
