using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the base class that holds basic search functionality.
    /// </summary>
    public abstract class ParameterBase
    {
        /// <summary>
        /// Entity date created
        /// </summary>
        public const string ODataEntityDateCreated = "datecreated";
        /// <summary>
        /// Entity data updated
        /// </summary>
        public const string ODataEntityDateUpdated = "dateupdated";
        /// <summary>
        /// Entity data updated, or if null, date created.
        /// </summary>
        public const string ODataEntityDateCombined = "datecombined";

        /// <summary>
        /// Date created, updated or a combination of both.
        /// </summary>
        public static IReadOnlyList<string> ODataDateParameters => new[] { ODataEntityDateCreated, ODataEntityDateUpdated, ODataEntityDateCombined };

        /// <summary>
        /// Returns true if the paramter field is a reserved data parameter name.
        /// </summary>
        public bool IsDateField => !string.IsNullOrEmpty(Parameter) 
            && ODataDateParameters.Contains(r => CompareOperator(r, Parameter));

        /// <summary>
        /// This is the specific parameter position.
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// This is the parameter value.
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// This helper method compares two parameter based on a case-insensitive format.
        /// </summary>
        /// <param name="op">The defined operator.</param>
        /// <param name="value">The incoming external data field.</param>
        /// <returns>True if there is a match.</returns>
        protected static bool CompareOperator(string op, string value) => string.Equals(value?.Trim(), op, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// This is the inherited Load method.
        /// </summary>
        /// <param name="position">The parameter position.</param>
        /// <param name="value">The raw value.</param>
        public virtual void Load(int position, string value)
        {
            Position = position;
            Parse(value);
        }

        /// <summary>
        /// This abstract method is used to parse the string value in to the relevant properties.
        /// </summary>
        /// <param name="value"></param>
        protected abstract void Parse(string value);

    }

}
