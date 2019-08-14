using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the filter collection.
    /// </summary>
    public class FilterCollection
    {
        #region Static declarations
        /// <summary>
        /// And
        /// </summary>
        public const string ODataConditionalAnd = "and";
        /// <summary>
        /// Or
        /// </summary>
        public const string ODataConditionalOr = "or";
        /// <summary>
        /// And, Or
        /// </summary>
        public static IReadOnlyList<string> ODataConditionals => new[] { ODataConditionalAnd, ODataConditionalOr };
        #endregion

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="filter">The incoming filter to parse.</param>
        public FilterCollection(string filter)
        {
            Params = SearchRequestHelper.BuildParameters<FilterParameter>(filter, ODataConditionals).ToDictionary(r => r.Position, r => r);

            var words = filter.Split(' ').Where(w => ODataConditionals.Contains(w)).ToArray();

            var valids = new List<int>();

            //OK, let's quickly do this really easily. There are better ways but I don't have the time.
            int max = 1 << Params.Count;
            for (int i = 0; i < max; i++)
            {
                var options = new bool[Params.Count];
                for (int check = 0; check < Params.Count; check++)
                {
                    var power = 1 << check;
                    options[check] = (i & power) > 0;
                }

                bool valid = options[0];

                for (int verify = 0; verify < words.Length; verify++)
                {
                    switch (words[verify].Trim().ToLowerInvariant())
                    {
                        case ODataConditionalAnd:
                            valid &= options[verify + 1];
                            break;
                        case ODataConditionalOr:
                            valid |= options[verify + 1];
                            break;
                    }
                }

                if (valid)
                    valids.Add(i);
            }

            Validity = valids;
        }

        public List<int> Validity { get; }

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params { get; }
    }
}
