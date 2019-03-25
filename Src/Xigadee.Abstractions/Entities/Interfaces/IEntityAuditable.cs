using System;

namespace Xigadee
{
    /// <summary>
    /// This interface specifies the basic properties for a security based entity system.
    /// </summary>
    public interface IEntityAuditable
    {
        /// <summary>
        /// Gets or sets the system identifier.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        Guid? VersionId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier of the last person who created or updated the entity.
        /// </summary>
        Guid? UserIdAudit { get; set; }

        /// <summary>
        /// Gets or sets the date created for the entity is this system.
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date updated time.
        /// </summary>
        DateTime? DateUpdated { get; set; }
    }
}
