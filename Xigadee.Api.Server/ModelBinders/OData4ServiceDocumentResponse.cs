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
            //var entity = mResponse.Entity;
            var entity = new OData<string>();
            entity.Metadata = $"{mRequest.Scheme}://{mRequest.Authority}{mRequest.AbsolutePath}";
            //mResponse.Entity.
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity)); ;
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
