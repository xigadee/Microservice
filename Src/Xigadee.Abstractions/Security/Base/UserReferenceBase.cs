using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This abstract class is used by security entities that have a one-to-many relationship with a user.
    /// </summary>
    /// <seealso cref="Xigadee.EntityAuditableBase" />
    /// <seealso cref="Xigadee.IPropertyBag" />
    public abstract class UserReferenceBase : EntitySignatureBase, IPropertyBag
    {
        /// <summary>
        /// Gets or sets the user identifier. This may not be set for all entities.
        /// </summary>
        [EntityPropertyHint("userid")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// The property bag container dictionary. This has a set of extensible properties for the user.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Returns the key entity property, i.e. 'userid'.
        /// </summary>
        /// <param name="e">The entity.</param>
        /// <returns>A kvp collection.</returns>
        public static IEnumerable<Tuple<string, string>> PropertiesGet(UserReferenceBase e)
        {
            if (e.UserId.HasValue)
                yield return new Tuple<string, string>("userid", e.UserId.Value.ToString("N").ToUpperInvariant());
        }
    }
}
