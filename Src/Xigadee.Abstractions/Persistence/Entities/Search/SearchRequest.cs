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
    public class SearchRequest: IEquatable<SearchRequest>
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

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequest"/> class.
        /// </summary>
        /// <param name="query">The search a uri.</param>
        public SearchRequest(Uri query) : this(query?.Query)
        {
        } 
        #endregion

        private void Assign(KeyValuePair<string,string> toSet)
        {
            var key = toSet.Key?.ToLowerInvariant();

            switch (key)
            {
                case "$id":
                    Id = toSet.Value?.Trim();
                    break;
                case "$etag":
                    ETag = toSet.Value?.Trim();
                    break;
                case "$filter":
                    Filter = toSet.Value?.Trim();
                    break;
                case "$orderby":
                    OrderBy = toSet.Value?.Trim();
                    break;
                case "$top":
                    Top = toSet.Value?.Trim();
                    break;
                case "$skip":
                    Skip = toSet.Value?.Trim();
                    break;
                case "$select":
                    Select = toSet.Value?.Trim();
                    break;
                case "":
                    break;
                case default(string):
                    break;
                default:
                    FilterParameters[key] = toSet.Value;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the parameter collection.
        /// </summary>
        public Dictionary<string, string> FilterParameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// This is the search algorithm that is used to search against the parameters specified.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// This is the search collection identifier. It allows pagination to occur with a cached sample.
        /// </summary>
        public string ETag { get; set; }

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

            FilterParameters
                .OrderBy(p => p.Key?.ToLowerInvariant()??"")
                .ForEach(p => addAmp |= AppendParam(sb, p.Key, p.Value, addAmp));

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
        public static explicit operator SearchRequest(string query)
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

    /// <summary>
    /// This is the helper class that is used to extend the SearchRequest functionality.
    /// </summary>
    public static class SearchRequestHelper
    {
        /// <summary>
        /// Converts the search request to a URI.
        /// </summary>
        /// <param name="sr">The search request object.</param>
        /// <param name="baseUri">The base URI as a string.</param>
        /// <returns>Returns a Uri with the combined parameters.</returns>
        public static Uri ToUri(this SearchRequest sr, string baseUri = null)
        {
            if (string.IsNullOrEmpty(baseUri))
                return sr.ToUri((Uri)null);

            return sr.ToUri(new Uri(baseUri));
        }
        /// <summary>
        /// Converts the search request to a URI.
        /// </summary>
        /// <param name="sr">The search request object.</param>
        /// <param name="baseUri">The optional base URI.</param>
        /// <returns>Returns a Uri with the combined parameters.</returns>
        public static Uri ToUri(this SearchRequest sr, Uri baseUri = null)
        {
            Uri build;

            if (baseUri == null)
                build = new Uri("?" + sr.ToString(), UriKind.Relative);
            else
                build = new Uri(baseUri, "?" + sr.ToString());

            return build;
        }

        /// <summary>
        /// This method parses the OrderBy parameters
        /// </summary>
        /// <param name="sr">The search request.</param>
        /// <returns>Returns an enumerable list of parameters along with the asc/desc flag</returns>
        public static IEnumerable<(string property, bool asc)> OrderBy(this SearchRequest sr)
        {
            if (string.IsNullOrEmpty(sr.OrderBy))
                yield break;

            var resL = StringHelper.SplitOnChars(sr.OrderBy ?? ""
                , (s) => s.ToLowerInvariant()
                , (s) => s
                , new[] { ',' }, new[] { ' ' }, s => s.Trim());

            foreach (var res in resL)
                yield return (res.Key, res.Value?.Equals("ASC", StringComparison.InvariantCultureIgnoreCase)??true);

        }

        /// <summary>
        /// This method parses the select parameters
        /// </summary>
        /// <param name="sr">The search request.</param>
        /// <returns>Returns an enumerable list of parameters.</returns>
        public static IEnumerable<string> Select(this SearchRequest sr)
        {
            if (string.IsNullOrEmpty(sr.Select))
                yield break;

            var resL = StringHelper.SplitOnChars(sr.OrderBy ?? ""
                , (s) => s.ToLowerInvariant()
                , (s) => s
                , new[] { ',' }, new[] { ' ' }, s => s.Trim());

            foreach (var res in resL)
                yield return res.Key;

        }
    }
}
