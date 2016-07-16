
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace Xigadee
{
    [ModelBinder(typeof(OData4ApiRequestModelBinder))]
    public class OData4ApiRequest: IRequestOptions
    {
        public string Id { get; set; }

        public string RefType { get; set; }

        public string RefValue { get; set; }

        public string Auth { get; set; }

        public RepositorySettings Options { get; set; }

        public byte[] Body { get; set; }

        public string BodyType { get; set; }

        public bool HasReference { get; set; }

        public bool HasKey { get; set; }

        public List<MediaTypeWithQualityHeaderValue> Accept { get; set; }
    }
}
