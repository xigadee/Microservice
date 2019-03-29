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
    public abstract class UserSession : UserReferenceBase
    {

    }
}
