namespace Xigadee
{
    /// <summary>
    /// This is the set of methods that a repository should support.
    /// </summary>
    public enum RepositoryMethod
    {
        /// <summary>
        /// Create the entity
        /// </summary>
        Create,
        /// <summary>
        /// Read the entity by the key.
        /// </summary>
        Read,
        /// <summary>
        /// Read the entity by reference
        /// </summary>
        ReadByRef,
        /// <summary>
        /// Update an existing entity.
        /// </summary>
        Update,
        /// <summary>
        /// The delete
        /// </summary>
        Delete,
        /// <summary>
        /// Delete the entity by the key.
        /// </summary>
        DeleteByRef,
        /// <summary>
        /// Read the entity version by the key
        /// </summary>
        Version,
        /// <summary>
        /// Read the entity version by the reference
        /// </summary>
        VersionByRef,
        /// <summary>
        /// Search the entities and retrieve a set of entity properties
        /// </summary>
        Search,
        /// <summary>
        /// Search the entities and retrieve a set of entities
        /// </summary>
        SearchEntity
    }
}
