using System;
using System.Diagnostics;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the incoming OData parameters.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class SearchRequest: SearchOData4Base, IEquatable<SearchRequest>
    {
        public bool Equals(SearchRequest other)
        {
            return other.ToString() == ToString();
        }

        public Uri ToUri(string baseUri = null)
        {
            if (string.IsNullOrEmpty(baseUri))
                return ToUri((Uri)null);

            return ToUri(new Uri(baseUri));
        }

        public Uri ToUri(Uri baseUri = null)
        {
            Uri build;

            if (baseUri == null)
                build = new Uri("?" + ToString(), UriKind.Relative);
            else
                build = new Uri(baseUri, "?" + ToString());

            return build;       
        }


        public override bool Equals(object obj)
        {
            if (obj is SearchRequest)
                return Equals((SearchRequest)obj);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = ToString().GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            bool addAmp = false;

            addAmp |= AppendParam(sb, "$filter", Filter, false);
            addAmp |= AppendParam(sb, "$apply", Apply, addAmp);
            addAmp |= AppendParam(sb, "$orderby", OrderBy, addAmp);
            addAmp |= AppendParam(sb, "$top", Top, addAmp);
            addAmp |= AppendParam(sb, "$skip", Skip, addAmp);
            addAmp |= AppendParam(sb, "$select", Select, addAmp);
            addAmp |= AppendParam(sb, "$expand", Expand, addAmp);
            addAmp |= AppendParam(sb, "$count", Count, addAmp);
            addAmp |= AppendParam(sb, "$format", Format, addAmp);
            addAmp |= AppendParam(sb, "$skiptoken", SkipToken, addAmp);
            addAmp |= AppendParam(sb, "$deltatoken", DeltaToken, addAmp);

            return sb.ToString();
        }

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
        /// The raw $filter query value from the incoming request
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        ///  The raw $apply query value from the incoming request
        /// </summary>
        public string Apply { get; set; }

        /// <summary>
        /// The raw $orderby query value from the incoming request
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// The raw $top query value from the incoming request
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// The raw $skip query value from the incoming request
        /// </summary>
        public string Skip { get; set; }

        /// <summary>
        /// The raw $select query value from the incoming request
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// The raw $expand query value from the incoming request
        /// </summary>
        public string Expand { get; set; }

        /// <summary>
        /// The raw $count query value from the incoming request
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// The raw $format query value from the incoming request
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The raw $skiptoken query value from the incoming request
        /// </summary>
        public string SkipToken { get; set; }

        /// <summary>
        /// The raw $deltatoken query value from the incoming request
        /// </summary>
        public string DeltaToken { get; set; }
    }
}
