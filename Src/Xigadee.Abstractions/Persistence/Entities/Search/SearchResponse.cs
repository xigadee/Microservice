using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    public class SearchResponse: SearchOData4Base
    {
        public string Etag { get; set; }

        public Dictionary<int, FieldMetadata> Fields { get; } = new Dictionary<int, FieldMetadata>();

        public List<JObject> Data { get; set;} 

    }

    public class FieldMetadata
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}
