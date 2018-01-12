using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for an Entity Container.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <seealso cref="Xigadee.IPersistenceEntityContainer{K, E}" />
    public abstract class EntityContainerBase<K, E>: ServiceBase<StatusBase>, IPersistenceEntityContainer<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This lock is used when modifying references.
        /// </summary>
        ReaderWriterLockSlim mReferenceModifyLock;
        #endregion

        /// <summary>
        /// Gets or sets the transform container.
        /// </summary>
        protected EntityTransformHolder<K, E> Transform { get; set; }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContainerBase{K, E}"/> class.
        /// </summary>
        public EntityContainerBase()
        {
            mReferenceModifyLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }
        #endregion

        #region IncomingParameterChecks ...
        /// <summary>
        /// Checks the incoming key parameter has a value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentOutOfRangeException">Key must be set to a value</exception>
        protected virtual void IncomingParameterChecks(K key)
        {
            if (key.Equals(default(K)))
                throw new ArgumentOutOfRangeException("key must be set to a value");
        }
        /// <summary>
        /// Checks the incoming key and entity value parameters have values.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The entity value.</param>
        /// <exception cref="ArgumentNullException">key or value must be set</exception>
        protected virtual void IncomingParameterChecks(K key, E value)
        {
            IncomingParameterChecks(key);
            if (value.Equals(default(E)))
                throw new ArgumentNullException("value must be set to a value");
        } 
        #endregion

        #region Atomic Wrappers...
        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
        protected void Atomic(Action action)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                action();
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }

        /// <summary>
        /// This wraps the requests the ensure that only one is processed at the same time.
        /// </summary>
        /// <param name="action">The action to process.</param>
        /// <returns>Returns the value.</returns>
        protected T Atomic<T>(Func<T> action)
        {
            try
            {
                mReferenceModifyLock.EnterReadLock();

                return action();
            }
            finally
            {
                mReferenceModifyLock.ExitReadLock();
            }
        }
        #endregion

        #region Start/Stop
        /// <summary>
        /// This method starts the service. You should override this method for your own logic and implement your specific start-up implementation.
        /// </summary>
        protected override void StartInternal()
        {

        }
        /// <summary>
        /// This method stops the service. You should override this method for your own logic.
        /// </summary>
        protected override void StopInternal()
        {

        }
        #endregion

        #region Configure(EntityTransformHolder<K, E> transform)
        /// <summary>
        /// Configures the specified container.
        /// </summary>
        /// <param name="transform">The persistence transform entity.</param>
        public virtual void Configure(EntityTransformHolder<K, E> transform)
        {
            Transform = transform;
        }
        #endregion

        #region Debug
        /// <summary>
        /// Gets the debug string.
        /// </summary>
        public virtual string Debug => $"{typeof(K).Name}/{typeof(E).Name} Entities={Count} References={CountReference}"; 
        #endregion

        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public abstract int Count { get; }
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public abstract int CountReference { get; }
        /// <summary>
        /// Gets the keys collection.
        /// </summary>
        public abstract ICollection<K> Keys { get; }
        /// <summary>
        /// Gets the references collection.
        /// </summary>
        public abstract ICollection<Tuple<string, string>> References { get; }
        /// <summary>
        /// Gets the values collection.
        /// </summary>
        public abstract ICollection<E> Values { get; }
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
        public abstract int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null);
        /// <summary>
        /// Clears this collection of all entities and references.
        /// </summary>
        public abstract void Clear();
        /// <summary>
        /// Determines whether the collection contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// <c>true</c> if the collection contains the key; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool ContainsKey(K key);
        /// <summary>
        /// Determines whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        /// <c>true</c> if the collection contains reference; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool ContainsReference(Tuple<string, string> reference);
        /// <summary>
        /// Removes an entity with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Returns true if the entity is removed successfully.
        /// </returns>
        public abstract bool Remove(K key);
        /// <summary>
        /// Removes the specified entities by the supplied reference.
        /// </summary>
        /// <param name="reference">The reference identifier.</param>
        /// <returns>
        /// Returns true if the entity is removed successfully.
        /// </returns>
        public abstract bool Remove(Tuple<string, string> reference);
        /// <summary>
        /// Tries to retrieve and entity by the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The output value.</param>
        /// <returns>
        /// True if the key exists.
        /// </returns>
        public abstract bool TryGetValue(K key, out E value);
        /// <summary>
        /// Tries to retrieve and entity by the reference id.
        /// </summary>
        /// <param name="reference">The reference id.</param>
        /// <param name="value">The output value.</param>
        /// <returns>
        /// True if the reference exists.
        /// </returns>
        public abstract bool TryGetValue(Tuple<string, string> reference, out E value);
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
        public abstract int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null);
    }
}
