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

            Filters = new FilterCollection(Filter);

            ParamsOrderBy =
            SearchRequestHelper.BuildParameters<OrderByParameter>(OrderBy, new[] { "," }).ToDictionary(r => r.Position, r => r);

            ParamsSelect =
            SearchRequestHelper.BuildParameters<SelectParameter>(Select, new[] { "," }).ToDictionary(r => r.Position, r => r);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        /// <param name="query">The search a uri.</param>
        public SearchRequest(Uri query) : this(query?.Query)
        {
        }
        #endregion

        #region Assign(KeyValuePair<string,string> toSet)
        /// <summary>
        /// Assigns the kvp to the collection.
        /// </summary>
        /// <param name="toSet">The KeyValuePair to set.</param>
        protected void Assign(KeyValuePair<string, string> toSet)
        {
            Assign(toSet.Key, toSet.Value);
        }
        #endregion
        #region Assign(string key, string value)
        /// <summary>
        /// Assigns the specified key and value to the search collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void Assign(string key, string value)
        {
            bool isSet = true;

            switch (key?.Trim().ToLowerInvariant())
            {
                case "$id":
                    Id = value?.Trim();
                    break;
                case "$etag":
                    ETag = value?.Trim();
                    break;
                case "$filter":
                    Filter = value?.Trim();
                    break;
                case "$orderby":
                    OrderBy = value?.Trim();
                    break;
                case "$top":
                    Top = value?.Trim();
                    break;
                case "$skip":
                    Skip = value?.Trim();
                    break;
                case "$select":
                    Select = value?.Trim();
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
        public FilterCollection Filters { get; } 

        /// <summary>
        /// This is the set of order by parameters.
        /// </summary>
        public Dictionary<int, OrderByParameter> ParamsOrderBy { get; }

        /// <summary>
        /// This is a set of select parameters. If this is an entity request then this is skipped.
        /// </summary>
        public Dictionary<int, SelectParameter> ParamsSelect { get; }

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

            addAmp |= AppendParam(sb, "$id", Id, false);
            addAmp |= AppendParam(sb, "$etag", ETag, addAmp);
            addAmp |= AppendParam(sb, "$filter", Filter, addAmp);
            addAmp |= AppendParam(sb, "$orderby", OrderBy, addAmp);
            addAmp |= AppendParam(sb, "$skip", Skip, addAmp);
            addAmp |= AppendParam(sb, "$top", Top, addAmp);
            addAmp |= AppendParam(sb, "$select", Select, addAmp);

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
