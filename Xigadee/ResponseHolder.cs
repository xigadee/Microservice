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

        public double ResourceCharge { get; set; }

        public TimeSpan? ThrottleSuggestedWait { get; set; }

        public HttpResponseMessage Response { get; set; }

        public override int StatusCode { get { return (Response != null?(int)Response.StatusCode: base.StatusCode); } }

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
