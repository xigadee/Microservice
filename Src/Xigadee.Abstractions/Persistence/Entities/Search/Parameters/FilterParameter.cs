using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// https://www.ibm.com/support/knowledgecenter/en/SSYJJF_1.0.0/ApplicationSecurityonCloud/api_odata2.html
    /// </summary>
    public class FilterParameter : ParameterBase
    {
        #region OData constants

        public const string ODataNull = "null";

        public const string ODataEqual = "eq";
        public const string ODataNotEqual = "ne";

        public const string ODataLessThan = "lt";
        public const string ODataLessThanOrEqual = "le";

        public const string ODataGreaterThan = "gt";
        public const string ODataGreaterThanOrEqual = "ge";

        #endregion

        public string Operator { get; set; }

        public string Value { get; set; }

        public bool IsNegation { get; set; }

        public string ValueRaw => Value?.Trim('\'');

        public bool IsNullOperator => CompareOperator(ODataNull, ValueRaw);

        public bool IsEqual => CompareOperator(ODataEqual);

        public bool IsNotEqual => CompareOperator(ODataNotEqual);

        public bool IsLessThan => CompareOperator(ODataLessThan);

        public bool IsGreaterThan => CompareOperator(ODataGreaterThan);

        private bool CompareOperator(string op) => CompareOperator(op, Operator);

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
    }

}
