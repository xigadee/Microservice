using System.Collections.Generic;
using System.Diagnostics;
namespace Xigadee
{
    /// <summary>
    /// This is the generic search response data.
    /// </summary>
    public class SearchResponse: SearchResponse<string[]>
    {
        /// <summary>
        /// This is the set of associated field metadata for the data set.
        /// </summary>
        public Dictionary<int, FieldMetadata> Fields { get; } = new Dictionary<int, FieldMetadata>();

        /// <summary>
        /// This is the list of string array response data.
        /// </summary>
        public override List<string[]> Data { get; set; } = new List<string[]>();
    }

    /// <summary>
    /// This is the entity search response class.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class SearchResponse<E>: SearchResponseBase
    {
        /// <summary>
        /// This is the set of entities for the response.
        /// </summary>
        public virtual List<E> Data { get; set; } = new List<E>();
    }

    /// <summary>
    /// This is the search response base class.
    /// </summary>
    public class SearchResponseBase:IPropertyBag
    {
        /// <summary>
        /// This is the etag value.
        /// </summary>
        public string Etag { get; set; }
        /// <summary>
        /// This is the top value of the result set.
        /// </summary>
        public int? Top { get; set; }
        /// <summary>
        /// This is the skip value of the result set.
        /// </summary>
        public int? Skip { get; set; }
        /// <summary>
        /// This is the optional property bag that additional properties can be appended to.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// This method populates the base request values to allow them to be passed though back to the client.
        /// </summary>
        /// <param name="rq"></param>
        public void PopulateSearchRequest(SearchRequest rq)
        {
            Etag = rq.ETag;
            Top = rq.TopValue;
            Skip = rq.SkipValue;
        }
    }

    /// <summary>
    /// This holds the field data.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class FieldMetadata
    {
        /// <summary>
        /// This is the field name.
        /// </summary>
        public string Name { get; set; }
    }
}
