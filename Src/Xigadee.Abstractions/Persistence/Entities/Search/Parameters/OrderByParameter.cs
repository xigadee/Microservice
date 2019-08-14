using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class OrderByParameter : ParameterBase
    {
        //Specify as ascending. 
        public const string ODataAscending = "asc";
        public const string ODataDecending = "desc";

        /// <summary>
        /// Specifies whether the parameter should be ordered in descending order.
        /// </summary>
        public bool IsDescending { get; set; }

        public bool IsDateField { get; private set; }

        protected override void Parse(string value)
        {
            var parts = value?.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return;

            Parameter = parts[0].Trim();

            IsDescending = parts.Length > 1 && string.Equals(parts[1].Trim(), ODataDecending, StringComparison.InvariantCultureIgnoreCase);

            IsDateField = string.Equals(Parameter, "DateCreated", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(Parameter, "DateUpdated", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
