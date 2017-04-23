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
        public HttpStatusCode StatusCode { get; set; }

        public byte[] Data { get; set; }

        public string ContentId { get; set; }

        public string VersionId { get; set; }

        public string MediaType { get; set; }


        public bool? IsCached { get; set; }

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

            if (IsCached ?? false)
                response.Headers.Add("X-IMG-IsCached", "1");

            return Task.FromResult(response);
        }
    }
}
