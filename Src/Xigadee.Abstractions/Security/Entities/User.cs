using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for a user on the system.
    /// </summary>
    /// <seealso cref="Xigadee.EntityAuditableBase" />
    /// <seealso cref="Xigadee.IPropertyBag" />
    public class User : EntitySignatureBase, IPropertyBag
    {
        /// <summary>
        /// Gets the realm for the user. This is to allow users groupings to be segmented across an application.
        /// </summary>
        [EntityPropertyHint("realm")]
        public string Realm { get; set; }
        /// <summary>
        /// The property bag container dictionary. This has a set of extensible properties for the user.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
