using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the incoming OData parameters.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class SearchRequest : IEquatable<SearchRequest>, IPropertyBag
    {
        #region OData constants
        /// <summary>
        /// $id
        /// </summary>
        public const string ODataId = "$id";
        /// <summary>
        /// $etag
        /// </summary>
        public const string ODataETag = "$etag";
        /// <summary>
        /// $filter
        /// </summary>
        public const string ODataFilter = "$filter";
        /// <summary>
        /// $orderby
        /// </summary>
        public const string ODataOrderBy = "$orderby";
        /// <summary>
        /// $skip
        /// </summary>
        public const string ODataSkip = "$skip";
        /// <summary>
        /// $top
        /// </summary>
        public const string ODataTop = "$top";
        /// <summary>
        /// $select
        /// </summary>
        public const string ODataSelect = "$select"; 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        public SearchRequest()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        /// <param name="query">The search parameters.</param>
        public SearchRequest(string query)
        {
            StringHelper.SplitOnChars(query ?? ""
                , (s) => s.ToLowerInvariant()
                , (s) => s
                , new[] { '&' }, new[] { '=' })
                .ForEach(kv => Assign(kv));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        /// <param name="query">The search a uri.</param>
        public SearchRequest(Uri query) : this(query?.Query)
        {
        }
        #endregion

        public void AssignTop(int? value) => Assign(ODataTop, value?.ToString()??null);
        public void AssignSkip(int? value) => Assign(ODataSkip, value?.ToString() ?? null);
        public void AssignId(string value) => Assign(ODataId, value);
        public void AssignETag(string value) => Assign(ODataETag, value);


        public void AssignFilter(string value) => Assign(ODataFilter, value);

        public void AppendFilter(string param, string op, string value, string conditional)
        {
            var filterAdd = $"{param} {op} {value}";
            AppendFilter(filterAdd, conditional);
        }

        public void AppendFilter(string filterAdd, string conditional)
        {
            if (ParamsFilter == null)
                ParamsFilter = new FilterCollection(filterAdd);
            else if (ParamsFilter.Count == 0)
                AssignFilter(filterAdd);
            else
                AssignFilter(ParamsFilter.Filter + $" {conditional} {filterAdd}");

            Filter = ParamsFilter.Filter;
        }

        public void AssignSelect(string value) => Assign(ODataSelect, value);

        public void AssignOrderBy(string value) => Assign(ODataOrderBy, value);



        #region Assign...
        /// <summary>
        /// Assigns the kvp to the collection.
        /// </summary>
        /// <param name="toSet">The KeyValuePair to set.</param>
        public void Assign(KeyValuePair<string, string> toSet)
        {
            Assign(toSet.Key, toSet.Value);
        }
        /// <summary>
        /// Assigns the specified key and value to the search collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Assign(string key, string value)
        {
            bool isSet = true;

            switch (key?.Trim().ToLowerInvariant())
            {
                case ODataId:
                    Id = value?.Trim();
                    break;
                case ODataETag:
                    ETag = value?.Trim();
                    break;
                case ODataFilter:
                    Filter = value?.Trim();
                    ParamsFilter = new FilterCollection(Filter);
                    break;
                case ODataOrderBy:
                    OrderBy = value?.Trim();
                    ParamsOrderBy = SearchRequestHelper.BuildParameters<OrderByParameter>(OrderBy, new[] { "," }).ToDictionary(r => r.Position, r => r);
                    break;
                case ODataTop:
                    Top = value?.Trim();
                    break;
                case ODataSkip:
                    Skip = value?.Trim();
                    break;
                case ODataSelect:
                    Select = value?.Trim();
                    ParamsSelect = SearchRequestHelper.BuildParameters<SelectParameter>(Select, new[] { "," }).ToDictionary(r => r.Position, r => r);
                    break;
                case "":
                case default(string):
                    isSet = false;
                    break;
            }

            IsSet |= isSet;
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance is set.
        /// </summary>
        public bool IsSet { get; private set; }

        /// <summary>
        /// This is the filter collection, along with the verification options.
        /// </summary>
        public FilterCollection ParamsFilter { get; private set; } 

        /// <summary>
        /// This is the set of order by parameters.
        /// </summary>
        public Dictionary<int, OrderByParameter> ParamsOrderBy { get; private set; }

        /// <summary>
        /// This is a set of select parameters. If this is an entity request then this is skipped.
        /// </summary>
        public Dictionary<int, SelectParameter> ParamsSelect { get; private set; }

        /// <summary>
        /// This is the search algorithm that is used to search against the parameters specified.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// This is the search collection identifier. It allows pagination to occur with a cached sample.
        /// </summary>
        public string ETag { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The raw $filter query value from the incoming request, this maps to the filter algorithm defined
        /// on the server side.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// The raw $orderby query value from the incoming request
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// The raw $top query value from the incoming request
        /// </summary>
        public string Top { get; set; }
        /// <summary>
        /// Tries to parse the top value in to a integer.
        /// </summary>
        public int? TopValue => int.TryParse(Top, out int value) ? value : default(int?);

        /// <summary>
        /// The raw $skip query value from the incoming request
        /// </summary>
        public string Skip { get; set; }
        /// <summary>
        /// Tries to parse the skip value in to a integer.
        /// </summary>
        public int? SkipValue => int.TryParse(Skip, out int value) ? value : default(int?);

        /// <summary>
        /// The raw $select query value from the incoming request, this is used for non-entity searches.
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// This is a set of additional properties that can be attached to a search request for logging later.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        #region Equals...
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SearchRequest other)
        {
            return other.ToString() == ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SearchRequest)
                return Equals((SearchRequest)obj);
            return false;
        } 
        #endregion
        #region GetHashCode()
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = ToString().GetHashCode();
                return result;
            }
        } 
        #endregion
        #region ToString()
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            bool addAmp = false;

            addAmp |= AppendParam(sb, ODataId, Id, false);
            addAmp |= AppendParam(sb, ODataETag, ETag, addAmp);
            addAmp |= AppendParam(sb, ODataFilter, Filter, addAmp);
            addAmp |= AppendParam(sb, ODataOrderBy, OrderBy, addAmp);
            addAmp |= AppendParam(sb, ODataSkip, Skip, addAmp);
            addAmp |= AppendParam(sb, ODataTop, Top, addAmp);
            addAmp |= AppendParam(sb, ODataSelect, Select, addAmp);

            return sb.ToString();
        }
        #endregion

        private bool AppendParam(StringBuilder sb, string part, string value, bool addAmp)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (addAmp)
                sb.Append('&');

            sb.AppendFormat("{0}={1}", part, value);

            return true;
        }

        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="query">The search query.</param>
        public static implicit operator SearchRequest(string query)
        {
            return new SearchRequest(query??"");
        }

        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="sr">The search result.</param>
        public static implicit operator string(SearchRequest sr)
        {
            return sr?.ToString();
        }
    }

}
