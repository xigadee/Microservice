using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the select parameter.
    /// </summary>
    public class SelectParameter : ParameterBase
    {
        /// <summary>
        /// This is only the parameter.
        /// </summary>
        /// <param name="value"></param>
        protected override void Parse(string value)
        {
            Parameter = value?.Trim();
        }

        /// <summary>
        /// This returns the set of hash parts.
        /// </summary>
        /// <returns>Returns a set of strings.</returns>
        protected override IEnumerable<string> HashParts()
        {
            yield return Parameter.ToLowerInvariant();
        }
    }
}
