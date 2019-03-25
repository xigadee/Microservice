﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class contains the assigned user roles for a security class.
    /// </summary>
    /// <seealso cref="Xigadee.EntityAuditableBase" />
    /// <seealso cref="Xigadee.IPropertyBag" />
    public abstract class UserRolesBase : EntityAuditableBase, IPropertyBag
    {
        /// <summary>
        /// The property bag container dictionary. This has a set of extensible properties for the user.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
