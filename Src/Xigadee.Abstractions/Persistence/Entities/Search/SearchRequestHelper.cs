using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the helper class that is used to extend the SearchRequest functionality.
    /// </summary>
    public static class SearchRequestHelper
    {
        /// <summary>
        /// This method builds the necessary parameters.
        /// </summary>
        /// <typeparam name="P">The parameter type.</typeparam>
        /// <param name="value">The string value.</param>
        /// <param name="splits">The arrays value.</param>
        /// <returns>Returns the relevant set of parameters.</returns>
        public static IEnumerable<P> BuildParameters<P>(string value, IEnumerable<string> splits) where P : ParameterBase, new()
        {
            if (value == null)
                yield break;

            var parts = value.Split(splits.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                yield break;

            int pos = 0;
            foreach (var part in parts)
            {
                var p = new P();
                p.Load(pos, part);
                yield return p;
                pos++;
            }
        }

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
                yield return (res.Key, res.Value?.Equals("ASC", StringComparison.InvariantCultureIgnoreCase) ?? true);

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

            var resL = StringHelper.SplitOnChars(sr.Select ?? ""
                , (s) => s.ToLowerInvariant()
                , (s) => s
                , new[] { ',' }, new[] { ' ' }, s => s.Trim());

            foreach (var res in resL)
                yield return res.Key;

        }
    }
}
