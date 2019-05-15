using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public static class SqlJsonSearchHelper
    {
        public static SqlJsonSearchRequest ToSqlJsonSearch(this SearchRequest rq)
        {
            var search = new SqlJsonSearchRequest();

            search.ETag = rq.ETag;
            search.Top = rq.TopValue;
            search.Skip = rq.SkipValue;

            return search;
        }
    }

    /// <summary>
    /// This is the stripped down search query for the SQL search
    /// </summary>
    public class SqlJsonSearchRequest
    {
        /// <summary>
        /// The search Etag.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// The top value.
        /// </summary>
        public int? Top { get; set; }
        /// <summary>
        /// The skip value.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the parameter collection.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();


    }
}
