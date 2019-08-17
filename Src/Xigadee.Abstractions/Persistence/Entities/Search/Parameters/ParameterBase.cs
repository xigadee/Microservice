using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

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
        /// This is the hash parameter that can be used to identify the same set of parameters for a parameter.
        /// </summary>
        public string Hash => HashBuild();

        /// <summary>
        /// This method combines the filter parameters in to a Base64 encoded hash.
        /// </summary>
        /// <returns>Returns the hash.</returns>
        protected virtual string HashBuild()
        {
            using (var sha256 = SHA256Managed.Create())
            {
                var sb = new StringBuilder();
                HashParts().ForEach(p => sb.Append($"{p}|"));

                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] hash = sha256.ComputeHash(bytes);
                //hash.
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// This override provides the parts used to create the parameter hash.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> HashParts();

        /// <summary>
        /// Date created, updated or a combination of both.
        /// </summary>
        public static IReadOnlyList<string> ODataDateParameters => new[] { ODataEntityDateCreated, ODataEntityDateUpdated, ODataEntityDateCombined };

        /// <summary>
        /// Returns true if the paramter field is a reserved data parameter name.
        /// </summary>
        public bool IsDateFieldParameter => !string.IsNullOrEmpty(Parameter) 
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
