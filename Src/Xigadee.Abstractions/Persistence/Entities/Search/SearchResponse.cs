using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    public class SearchResponse: SearchResponse<string[]>
    {
        public Dictionary<int, FieldMetadata> Fields { get; } = new Dictionary<int, FieldMetadata>();

        public override List<string[]> Data { get; set; } = new List<string[]>();
    }

    public class SearchResponse<E>: SearchResponseBase
    {
        public virtual List<E> Data { get; set; }

    }

    public class SearchResponseBase
    {
        public string Etag { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }
    }

    public class FieldMetadata
    {
        public string Name { get; set; }
    }
}
