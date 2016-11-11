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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Xigadee
{
    public class OData4ServiceDocumentResponse: IHttpActionResult
    {
        private readonly RepositoryHolder<SearchRequest, SearchResponse> mResponse;
        private readonly Uri mRequest;

        public OData4ServiceDocumentResponse(RepositoryHolder<SearchRequest, SearchResponse> response, Uri requestUri)
        {
            mRequest = requestUri;
            mResponse = response;
        }

        protected byte[] Data()
        {
            var entity = new OData<JObject>();
            entity.Metadata = $"{mRequest.Scheme}://{mRequest.Authority}{mRequest.AbsolutePath}";
            // we should change the mresponse.Entity.Data to a JObject since the query result will return a JObject
            if (mResponse.Entity.Data != null)
                foreach (JObject obj in mResponse.Entity.Data)
                {
                    entity.Value.Add(obj);
                }
            else
            {
                var tempJObject = new JObject();
                tempJObject.Add("Message", "No Entities Returned");
                entity.Value.Add(tempJObject);
            }
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            response.Content = new ByteArrayContent(Data());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (!string.IsNullOrEmpty(mResponse.Entity?.Etag))
                response.Headers.ETag = new EntityTagHeaderValue(mResponse.Entity.Etag);


            return response;
        }

        public class OData<V>
        {
            [JsonProperty("@odata.metadata")]
            public string Metadata { get; set; }
            public List<V> Value { get; } = new List<V>();
        }
    }

    
}
