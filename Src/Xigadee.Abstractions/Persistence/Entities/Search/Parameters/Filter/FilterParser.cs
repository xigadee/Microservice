using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Xigadee
{
    public static class FilterParser
    {
        public static FilterParserNodeRoot Parse(string filter)
        {
            var root = new FilterParserNodeRoot(filter);

            return root;
        }
    }

    public abstract class FilterParserNode
    {
        public string Filter { get; protected set; }

    }

    [DebuggerDisplay("{Filter}")]
    public class FilterParserNodeRoot: FilterParserNode
    {
        public FilterParserNodeRoot(string filter)
        {
            Filter = filter;
        }



    }

    public class FilterExpression : FilterOperation
    {

    }

    public class FilterOperation : FilterParserNode
    {
        public FilterLogical Next { get; set; }
    }

    public class FilterLogical : FilterParserNode
    {
        public FilterLogical(ODataLogicalOperators op)
        {
            Operator = op;
        }

        public ODataLogicalOperators Operator { get; }

        public FilterOperation Next { get; set; }

        #region Static declarations: Conditionals
        /// <summary>
        /// This method converts the shortcut enumeration to the actual string representation.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <returns>Returns the string equivalent.</returns>
        public static ODataLogicalOperators Convert(string op)
        {
            switch (op?.Trim().ToLowerInvariant()??"")
            {
                case ODataConditionalAnd:
                    return ODataLogicalOperators.OpAnd;
                case ODataConditionalOr:
                    return ODataLogicalOperators.OpOr;
                case ODataConditionalXOr:
                    return ODataLogicalOperators.OpXor;
                default:
                    throw new ArgumentOutOfRangeException(op);
            }
        }

        /// <summary>
        /// This method converts the shortcut enumeration to the actual string representation.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <returns>Returns the string equivalent.</returns>
        public static string Convert(ODataLogicalOperators op)
        {
            switch (op)
            {
                case ODataLogicalOperators.OpAnd:
                    return ODataConditionalAnd;
                case ODataLogicalOperators.OpOr:
                    return ODataConditionalOr;
                case ODataLogicalOperators.OpXor:
                    return ODataConditionalXOr;
                default:
                    throw new ArgumentOutOfRangeException(op.ToString());
            }
        }
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
    }

}
