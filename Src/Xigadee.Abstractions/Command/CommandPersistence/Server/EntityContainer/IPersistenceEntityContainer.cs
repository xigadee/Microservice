using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface defines the methods for an entity container.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IPersistenceEntityContainer<K, E>:IService, IRequireServiceHandlers
        where K : IEquatable<K>
    {
        /// <summary>
        /// Configures the specified container.
        /// </summary>
        /// <param name="transform">The persistence transform entity.</param>
        void Configure(EntityTransformHolder<K, E> transform);
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        int CountReference { get; }
        /// <summary>
        /// Gets the debug string containing a summary of the collection.
        /// </summary>
        string Debug { get; }
        /// <summary>
        /// Gets the keys collection.
        /// </summary>
        IEnumerable<K> Keys { get; }
        /// <summary>
        /// Gets the references collection.
        /// </summary>
        IEnumerable<Tuple<string, string>> References { get; }
        /// <summary>
        /// Gets the values collection.
        /// </summary>
        IEnumerable<E> Values { get; }

        /// <summary>
        /// Clears this collection of all entities and references.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains the key; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsKey(K key);
        /// <summary>
        /// Determines whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        ///   <c>true</c> if the collection contains reference; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsReference(Tuple<string, string> reference);

        /// <summary>
        /// This is the entity reference.
        /// </summary>
        /// <param name="key">The entity key</param>
        /// <param name="value">The entity</param>
        /// <param name="references">The optional references.</param>
        /// <returns>
        /// 201 - Created
        /// 409 - Conflict
        /// </returns>
        int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null);

        /// <summary>
        /// Removes an entity with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns true if the entity is removed successfully.</returns>
        bool Remove(K key);
        /// <summary>
        /// Removes the specified entities by the supplied reference.
        /// </summary>
        /// <param name="reference">The reference identifier.</param>
        /// <returns>Returns true if the entity is removed successfully.</returns>
        bool Remove(Tuple<string, string> reference);

        /// <summary>
        /// Tries to retrieve and entity by the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The output value.</param>
        /// <returns>True if the key exists.</returns>
        bool TryGetValue(K key, out E value);
        /// <summary>
        /// Tries to retrieve and entity by the reference id.
        /// </summary>
        /// <param name="reference">The reference id.</param>
        /// <param name="value">The output value.</param>
        /// <returns>True if the reference exists.</returns>
        bool TryGetValue(Tuple<string, string> reference, out E value);

        /// <summary>
        /// This method updates an existing entity.
        /// </summary>
        /// <param name="key">THe entity key.</param>
        /// <param name="newEntity">The newEntity.</param>
        /// <param name="newReferences">The optional new references.</param>
        /// <returns>
        /// 200 - Updated
        /// 404 - Not sound.
        /// 409 - Conflict
        /// </returns>
        int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null);
    }
}