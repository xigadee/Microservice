using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class holds the filter collection.
    /// </summary>
    [DebuggerDisplay("Searches={Params.Count}/Solutions={Solutions.Count}|{Filter}")]
    public class FilterCollection
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="filter">The incoming filter to parse.</param>
        public FilterCollection(string filter = null)
        {
            Tokens = new ODataTokenCollection(filter);
        }
        #endregion

        /// <summary>
        /// This is the parsed token collection.
        /// </summary>
        protected ODataTokenCollection Tokens { get; }

        /// <summary>
        /// This is the raw filter string.
        /// </summary>
        public string Filter => Tokens?.ToString() ?? "";

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params => Tokens?.Params;

        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions => Tokens?.Solutions;

        /// <summary>
        /// This is the list of supported filters. If this is 0, the Stored Procedure will return all entities.
        /// </summary>
        public int Count => Params?.Count ?? 0;

        /// <summary>
        /// This method returns true if the filter collection has the specific parameter.
        /// </summary>
        /// <param name="param">The parameter to check.</param>
        /// <param name="options">The string comparison options. The default value is case insensitive.</param>
        /// <returns>Returns true if the parameters is found.</returns>
        public bool ContainsParam(string param, StringComparison options = StringComparison.InvariantCultureIgnoreCase)
            => Params.Values.Contains(p => string.Equals(p.Parameter, param, options));

        /// <summary>
        /// This method calculates the possible solution bitmap.
        /// </summary>
        /// <param name="filter">The filter string.</param>
        /// <returns>Returns the list of solutions based on the raw boolean operators.</returns>
        public static List<int> CalculateSolutions(string filter)
        {
            var words = filter.Split(' ').Where(w => ODataTokenString.ODataConditionals.Contains(w)).ToArray();
            int count = words.Length + 1;

            var solutions = new List<int>();

            //OK, let's quickly do this really easily. There are better ways but I don't have the time.
            int max = 1 << count;
            for (int i = 0; i < max; i++)
            {
                var options = new bool[count];
                for (int check = 0; check < count; check++)
                {
                    var power = 1 << check;
                    options[check] = (i & power) > 0;
                }

                bool solution = options[0];

                for (int verify = 0; verify < words.Length; verify++)
                {
                    switch (words[verify].Trim().ToLowerInvariant())
                    {
                        case ODataTokenString.ODataConditionalAnd:
                            solution &= options[verify + 1];
                            break;
                        case ODataTokenString.ODataConditionalOr:
                            solution |= options[verify + 1];
                            break;
                        case ODataTokenString.ODataConditionalXOr:
                            solution ^= options[verify + 1];
                            break;
                    }
                }

                if (solution)
                    solutions.Add(i);
            }

            return solutions;
        }
    }
}
