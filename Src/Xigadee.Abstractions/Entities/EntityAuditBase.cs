using System;

namespace Xigadee
{
    /// <summary>
    /// This is a base class that can be used to implement a standard set of security ids for a system entity.
    /// It is the base for the security classes for a standard Xigadee based application.
    /// </summary>
    public abstract class EntityAuditableBase : IEntityAuditable
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the version id of the entity.
        /// </summary>
        public virtual Guid? VersionId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the user identifier of the last person who created or updated the entity.
        /// </summary>
        public virtual Guid? UserIdAudit { get; set; }

        /// <summary>
        /// This is the UTC time that the entity was initially created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// This is the optional last update date.
        /// </summary>
        public virtual DateTime? DateUpdated { get; set; }

    }
}
