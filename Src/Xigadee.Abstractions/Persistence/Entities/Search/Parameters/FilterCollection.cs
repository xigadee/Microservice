using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the filter collection.
    /// </summary>
    [DebuggerDisplay("Searches={Params.Count}/Solutions={Solutions.Count}|{Filter}")]
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

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="filter">The incoming filter to parse.</param>
        public FilterCollection(string filter)
        {
            Filter = filter;

            if (string.IsNullOrWhiteSpace(filter))
            {
                Params = new Dictionary<int, FilterParameter>();
                Solutions = new List<int>();
                return;
            }

            var filterParsed = BracketDefinition.Parse(filter, out var bracketPositions);
            Params = SearchRequestHelper.BuildParameters<FilterParameter>(filterParsed, ODataConditionals).ToDictionary(r => r.Position, r => r);
            Solutions = CalculateSolutions(filter);
        }
        #endregion

        /// <summary>
        /// This is the raw filter string.
        /// </summary>
        public string Filter { get; }

        /// <summary>
        /// This is a list of valid solutions for the logical collection.
        /// </summary>
        public List<int> Solutions { get; }

        /// <summary>
        /// This is the list of supported filters. If this is 0, the Stored Procedure will return all entities.
        /// </summary>
        public int Count => Params?.Count ?? 0;

        /// <summary>
        /// The search parameters.
        /// </summary>
        public Dictionary<int, FilterParameter> Params { get; }

        /// <summary>
        /// This method returns true if the filter collection has the specific parameter.
        /// </summary>
        /// <param name="param">The parameter to check.</param>
        /// <param name="options">The string comparison options. The default value is case insensitive.</param>
        /// <returns>Returns true if the parameters is found.</returns>
        public bool ContainsParam(string param, StringComparison options = StringComparison.InvariantCultureIgnoreCase)
        {
            return Params.Values.Contains(p => string.Equals(p.Parameter, param, options));
        }


        /// <summary>
        /// This method calculates the possible solution bitmap.
        /// </summary>
        /// <param name="filter">The filter string.</param>
        /// <returns>Returns the list of solutions based on the raw boolean operators.</returns>
        public static List<int> CalculateSolutions(string filter)
        {
            var words = filter.Split(' ').Where(w => ODataConditionals.Contains(w)).ToArray();
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

            return solutions;
        }

        ///// <summary>
        ///// This method calculates the possible solution bitmap.
        ///// </summary>
        ///// <param name="filter">The filter string.</param>
        ///// <returns>Returns the list of solutions based on the raw boolean operators.</returns>
        //public static List<int> CalculateSolutionsWithBrackets(string filter)
        //{
        //    var words = filter.Split(' ').Where(w => ODataConditionals.Contains(w)).ToArray();
        //    int count = words.Length + 1;

        //    var solutions = new List<int>();


        //    return solutions;
        //}



    }

    /// <summary>
    /// This is the linked list for bracket definition.
    /// </summary>
    [DebuggerDisplay("{Start}->{End}|{IndentLevel}|{HasParent} {Value}")]
    public class BracketDefinition
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="filter"></param>
        /// <param name="parent"></param>
        public BracketDefinition(int start, string filter, BracketDefinition parent)
        {
            Start = start;
            Filter = filter;
            Parent = parent;
        }
        /// <summary>
        /// This method is set when the end bracket is reached.
        /// </summary>
        /// <param name="end">The end bracket position.</param>
        /// <param name="level">The current indent level.</param>
        public void Finish(int end, int level)
        {
            End = end;
            IndentLevel = level;
            if (HasParent)
                Parent.Children.Add(this);
        }
        /// <summary>
        /// This is the optional parent if the string is indented.
        /// </summary>
        public BracketDefinition Parent { get; }
        /// <summary>
        /// Specifies whether the bracket has children.
        /// </summary>
        public List<BracketDefinition> Children { get; } = new List<BracketDefinition>();

        /// <summary>
        /// Specifies whether it has a parent defined.
        /// </summary>
        public bool HasParent => Parent != null;
        /// <summary>
        /// Specifies whether this has a children.
        /// </summary>
        public bool HasChildren => Children.Count > 0;

        /// <summary>
        /// Specifies whether this is a duplicate and can be removed. This happens when we have a section
        /// with extra brackets.
        /// </summary>
        public bool IsExtraDuplicate => Children.Count == 1 && IsBracketStart && IsBracketEnd;

        private bool IsBracketStart => BracketStart.Trim() == "(";
        private string BracketStart => Filter.Substring(Start, Children[0].Start - Start);

        private bool IsBracketEnd => BracketEnd.Trim() == ")";
        private string BracketEnd => Filter.Substring(Children[0].End.Value, End.Value - Children[0].End.Value +1);
        /// <summary>
        /// This is the start position
        /// </summary>
        public int Start { get; }
        /// <summary>
        /// This is the endposition
        /// </summary>
        public int? End { get; private set; }
        /// <summary>
        /// This is the shared raw filter.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// This is the parsed filter section with the bracketed section trimmed out.
        /// </summary>
        public string Value => !HasParent?Filter:(string.IsNullOrWhiteSpace(Filter) || !End.HasValue) ? null : Filter.Substring(Start + 1, End.Value - Start - 1).Trim();
        /// <summary>
        /// This is the bracket indent level.
        /// </summary>
        public int IndentLevel { get; private set; }

        /// <summary>
        /// This method parses out the brackets in the logical expression.
        /// </summary>
        /// <param name="filter">The incoming filter.</param>
        /// <param name="bracketsOut">The position of the start and end brackets.</param>
        /// <returns>Returns the parsed string.</returns>
        public static string Parse(string filter, out List<BracketDefinition> bracketsOut)
        {
            //Remove any whitespace
            filter = filter.Trim();

            var posStack = new Stack<BracketDefinition>();
            var brackets = new List<BracketDefinition>();

            Action<int> push = (int pos) => {
                var bracket = new BracketDefinition(pos, filter, posStack.Count > 0 ? posStack.Peek() : null);
                brackets.Add(bracket);
                posStack.Push(bracket);
            };

            Action<int> pull = (int pos) => {
                var start = posStack.Pop();
                start.Finish(pos, posStack.Count);
            };

            push(0);

            var sb = new StringBuilder();
            bool withinStringLiteral = false;

            for (int pos = 0; pos < filter.Length; pos++)
            {
                var ch = filter[pos];
                switch (ch)
                {
                    case '\'':
                        //We may get brackets within a text section. We should not strip them out.
                        withinStringLiteral = !withinStringLiteral;
                        sb.Append(ch);
                        break;
                    case '(':
                        if (withinStringLiteral)
                            sb.Append(ch);
                        else
                            push(pos);
                        break;
                    case ')':
                        if (withinStringLiteral)
                            sb.Append(ch);
                        else
                            pull(pos);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }

            pull(filter.Length - 1);

            bracketsOut = brackets;

            return sb.ToString();
        }

    }
}
