using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is a base class that can be used to implement a standard set of security ids for a system entity.
    /// It is the base for the security classes for a standard Xigadee based application.
    /// </summary>
    public abstract class EntitySignatureBase : EntityAuditableBase, IEntitySignature
    {
        /// <summary>
        /// Gets or sets the optional signature.
        /// </summary>
        public KeyValuePair<string, string>? Signature { get; set; }
    }
}
