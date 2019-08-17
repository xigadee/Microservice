using System;

namespace Xigadee
{
    /// <summary>
    /// This is the order by parameter.
    /// </summary>
    public class OrderByParameter : ParameterBase
    {
        /// <summary>
        /// ASC: the ascending parameter.
        /// </summary>
        public const string ODataAscending = "asc";
        /// <summary>
        /// DESC: the descending parameter.
        /// </summary>
        public const string ODataDecending = "desc";

        /// <summary>
        /// Specifies whether the parameter should be ordered in descending order.
        /// </summary>
        public bool IsDescending { get; set; }

        /// <summary>
        /// Parses the incoming data to the relevant order by parameters,
        /// </summary>
        /// <param name="value">The raw value.</param>
        protected override void Parse(string value)
        {
            var parts = value?.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return;

            Parameter = parts[0].Trim();

            IsDescending = parts.Length > 1 && string.Equals(parts[1]?.Trim(), ODataDecending, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
