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
        /// XOr
        /// </summary>
        public const string ODataConditionalXOr = "xor";
        /// <summary>
        /// And, Or, XOr
        /// </summary>
        public static IReadOnlyList<string> ODataConditionals => new[] { ODataConditionalAnd, ODataConditionalOr, ODataConditionalXOr };
        #endregion

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="filter">The incoming filter to parse.</param>
        public FilterCollection(string filter)
        {
            Params = SearchRequestHelper.BuildParameters<FilterParameter>(filter, ODataConditionals).ToDictionary(r => r.Position, r => r);

            var words = filter.Split(' ').Where(w => ODataConditionals.Contains(w)).ToArray();

            var solutions = new List<int>();

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

                bool solution = options[0];

                for (int verify = 0; verify < words.Length; verify++)
                {
                    switch (words[verify].Trim().ToLowerInvariant())
                    {
                        case ODataConditionalAnd:
                            solution &= options[verify + 1];
                            break;
                        case ODataConditionalOr:
                            solution |= options[verify + 1];
                            break;
                        case ODataConditionalXOr:
                            solution ^= options[verify + 1];
                            break;
                    }
                }

                if (solution)
                    solutions.Add(i);
            }

            Solutions = solutions;
        }

        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions { get; }

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params { get; }
    }
}
