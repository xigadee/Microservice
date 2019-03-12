using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    public class SearchResponse: SearchResponse<string>
    {

    }

    public class SearchResponse<E>
    {
        public string Etag { get; set; }

        public Dictionary<int, FieldMetadata> Fields { get; } = new Dictionary<int, FieldMetadata>();

        public List<E> Data { get; set; }

    }

    public class FieldMetadata
    {
        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
