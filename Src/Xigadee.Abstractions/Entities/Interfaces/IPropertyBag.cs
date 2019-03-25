using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by entities that require a set of extensible key-value properties.
    /// </summary>
    public interface IPropertyBag
    {
        /// <summary>
        /// The property bag container dictionary.
        /// </summary>
        Dictionary<string, string> Properties { get; set; }
    }
}
