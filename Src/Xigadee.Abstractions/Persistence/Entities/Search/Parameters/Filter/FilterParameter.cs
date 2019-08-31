using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// https://www.ibm.com/support/knowledgecenter/en/SSYJJF_1.0.0/ApplicationSecurityonCloud/api_odata2.html
    /// </summary>
    
    [DebuggerDisplay("{Position}:{Parameter}-{Operator}-{ValueRaw} [Negation:{IsNegation}]")]
    public class FilterParameter : ParameterBase
    {
        #region OData constants
        /// <summary>
        /// This method converts the shortcut enumeration to the actual string representation.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <returns>Returns the string equivalent.</returns>
        public static string Convert(ODataFilterOperations op)
        {
            switch (op)
            {
                case ODataFilterOperations.Equal:
                    return ODataEqual;
                case ODataFilterOperations.NotEqual:
                    return ODataNotEqual;
                case ODataFilterOperations.LessThan:
                    return ODataLessThan;
                case ODataFilterOperations.LessThanOrEqual:
                    return ODataLessThanOrEqual;
                case ODataFilterOperations.GreaterThan:
                    return ODataGreaterThan;
                case ODataFilterOperations.GreaterThanOrEqual:
                    return ODataGreaterThanOrEqual;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Null
        /// </summary>
        public const string ODataNull = "null";
        /// <summary>
        /// Equal
        /// </summary>
        public const string ODataEqual = "eq";
        /// <summary>
        /// Not equal
        /// </summary>
        public const string ODataNotEqual = "ne";
        /// <summary>
        /// Less than
        /// </summary>
        public const string ODataLessThan = "lt";
        /// <summary>
        /// Less than or equal
        /// </summary>
        public const string ODataLessThanOrEqual = "le";
        /// <summary>
        /// Greater than
        /// </summary>
        public const string ODataGreaterThan = "gt";
        /// <summary>
        /// Greater than or equal
        /// </summary>
        public const string ODataGreaterThanOrEqual = "ge";
        #endregion

        /// <summary>
        /// The logical operator specified in the query.
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// The value specified.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Specifies whether this filter is a negation, i.e. not result
        /// </summary>
        public bool IsNegation { get; set; }

        /// <summary>
        /// The raw value specified with any speech marks removed.
        /// </summary>
        public string ValueRaw => Value?.Trim('\'');

        /// <summary>
        /// Specifies whether this is a null check.
        /// </summary>
        public bool IsNullOperator => CompareOperator(ODataNull, Value);

        /// <summary>
        /// This is an equal filter search.
        /// </summary>
        public bool IsEqual => CompareOperator(ODataEqual);
        /// <summary>
        /// This is a not equal filter request.
        /// </summary>
        public bool IsNotEqual => CompareOperator(ODataNotEqual);

        private bool CompareOperator(string op) => CompareOperator(op, Operator);

        #region Parse(string value)
        /// <summary>
        /// This method parses the incoming value and sets the relevant properties.
        /// </summary>
        /// <param name="value"></param>
        protected override void Parse(string value)
        {
            var parts = value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return;

            int offset = 0;

            if (string.Equals(parts[0].Trim(), "not", StringComparison.InvariantCultureIgnoreCase))
            {
                if (parts.Length == 1)
                    return;

                IsNegation = true;
                offset = 1;
            }

            this.Parameter = parts[offset];
            if (parts.Length == 1 + offset)
                return;

            this.Operator = parts[1 + offset];
            if (parts.Length == 2 + offset)
                return;

            this.Value = parts[2 + offset];
        }
        #endregion

        /// <summary>
        /// This returns the set of hash parts.
        /// </summary>
        /// <returns>Returns a set of strings.</returns>
        protected override IEnumerable<string> HashParts()
        {
            yield return Parameter.ToLowerInvariant();
            yield return Operator.ToLowerInvariant();
            yield return IsNullOperator? ODataNull : ValueRaw;
        }
    }

}
