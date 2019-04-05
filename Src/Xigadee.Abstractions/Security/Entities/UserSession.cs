using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the details of a user session on the system.
    /// This ensures that timeouts can be managed on the core system and internal user details are not exposed to the client.
    /// </summary>
    /// <seealso cref="Xigadee.EntityAuditableBase" />
    /// <seealso cref="Xigadee.IPropertyBag" />
    public class UserSession : UserReferenceBase
    {
        /// <summary>
        /// Gets or sets the source that created the session.
        /// </summary>
        [EntityPropertyHint("sessionsource")]
        public string Source { get; set; }
    }
}
