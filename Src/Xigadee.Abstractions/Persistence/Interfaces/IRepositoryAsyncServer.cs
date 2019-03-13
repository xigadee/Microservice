using System;
using System.Security.Principal;
namespace Xigadee
{
    /// <summary>
    /// This is a default repository async interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepositoryAsyncClient<K, E> : IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Gets or sets the default security principal.
        /// </summary>
        IPrincipal DefaultPrincipal { get; set; }
    }

    /// <summary>
    /// This is a default repository async interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepositoryAsyncServer<K, E> : IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        string EntityName { get; }
        /// <summary>
        /// Gets the key maker.
        /// </summary>
        Func<E, K> KeyMaker { get; }
        /// <summary>
        /// Holds the policy concerning whether the repository implements optimistic locking 
        /// and or history of entities.
        /// </summary>
        VersionPolicy<E> VersionPolicy { get; }

        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        event EventHandler<SearchRequest> OnBeforeSearch;

        /// <summary>
        /// Occurs before and entity is created.
        /// </summary>
        event EventHandler<E> OnBeforeCreate;
        /// <summary>
        /// Occurs before an entity is updated.
        /// </summary>
        event EventHandler<E> OnBeforeUpdate;
        /// <summary>
        /// Occurs after an entity is created.
        /// </summary>
        event EventHandler<E> OnAfterCreate;
        /// <summary>
        /// Occurs after an entity is updated.
        /// </summary>
        event EventHandler<E> OnAfterUpdate;

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        event EventHandler<ReferenceHolder<K>> OnBeforeRead;
        /// <summary>
        /// Occurs before an entity is deleted.
        /// </summary>
        event EventHandler<ReferenceHolder<K>> OnBeforeDelete;
        /// <summary>
        /// Occurs before an entity is versioned.
        /// </summary>
        event EventHandler<ReferenceHolder<K>> OnBeforeVersion;
    }

}
