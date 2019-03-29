using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the entity signature.
    /// </summary>
    public interface IEntitySignature
    {
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        KeyValuePair<string,string>? Signature { get; set; }

    }
}
