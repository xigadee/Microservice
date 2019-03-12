using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the base repository holder.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class RepositoryBase<K, E> : IRepositoryAsyncServer<K, E>
        where K : IEquatable<K>
    {
        #region Events
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<SearchRequest> OnBeforeSearch;

        /// <summary>
        /// Occurs before and entity is created.
        /// </summary>
        public event EventHandler<E> OnBeforeCreate;
        /// <summary>
        /// Occurs before an entity is updated.
        /// </summary>
        public event EventHandler<E> OnBeforeUpdate;
        /// <summary>
        /// Occurs after an entity is created.
        /// </summary>
        public event EventHandler<E> OnAfterCreate;
        /// <summary>
        /// Occurs after an entity is updated.
        /// </summary>
        public event EventHandler<E> OnAfterUpdate;
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeRead;
        /// <summary>
        /// Occurs before an entity is deleted.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeDelete;
        /// <summary>
        /// Occurs before an entity is versioned.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeVersion;

        #region OnEntityEvent(EventType type, TEntity entity)        
        /// <summary>
        /// This is the entity event type.
        /// </summary>
        protected enum EntityEventType
        {
            /// <summary>
            /// Before the entity is created
            /// </summary>
            BeforeCreate,
            /// <summary>
            /// Before the entity is update
            /// </summary>
            BeforeUpdate,
            /// <summary>
            /// After the entity is created
            /// </summary>
            AfterCreate,
            /// <summary>
            /// After the entity is updated
            /// </summary>
            AfterUpdate
        }
        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="entity">The entity.</param>
        protected void OnEntityEvent(EntityEventType type, Func<E> entity)
        {
            switch (type)
            {
                case EntityEventType.BeforeCreate:
                    OnBeforeCreate?.Invoke(this, entity());
                    break;
                case EntityEventType.BeforeUpdate:
                    OnBeforeUpdate?.Invoke(this, entity());
                    break;
                case EntityEventType.AfterCreate:
                    OnAfterCreate?.Invoke(this, entity());
                    break;
                case EntityEventType.AfterUpdate:
                    OnAfterUpdate?.Invoke(this, entity());
                    break;
            }
        }
        #endregion
        #region OnKeyEvent(KeyEventType type, TKey key = default(TKey), string refType = null, string refValue = null)
        protected enum KeyEventType
        {
            BeforeRead,
            BeforeDelete,
            BeforeVersion,
        }
        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="key"></param>
        /// <param name="refType"></param>
        /// <param name="refValue"></param>
        protected void OnKeyEvent(KeyEventType type, K key = default(K), string refType = null, string refValue = null)
        {
            var holder = new ReferenceHolder<K>() { Key = key, RefType = refType, RefValue = refValue };

            switch (type)
            {
                case KeyEventType.BeforeRead:
                    OnBeforeRead?.Invoke(this, holder);
                    break;
                case KeyEventType.BeforeDelete:
                    OnBeforeDelete?.Invoke(this, holder);
                    break;
                case KeyEventType.BeforeVersion:
                    OnBeforeVersion?.Invoke(this, holder);
                    break;
            }
        }
        #endregion
        #region OnSearchEvent(SearchRequest key)
        /// <summary>
        /// Called when a search event occurs.
        /// </summary>
        /// <param name="key">The search request.</param>
        protected void OnSearchEvent(SearchRequest key)
        {
            OnBeforeSearch?.Invoke(this, key);
        }
        #endregion
        #endregion
        #region Declarations        
        /// <summary>
        /// The key maker used to extract the key from an incoming entity.
        /// </summary>
        protected readonly Func<E, K> _keyMaker;

        /// <summary>
        /// The reference maker used to make the reference values from an entity.
        /// </summary>
        protected readonly Func<E, IEnumerable<Tuple<string, string>>> _referenceMaker;
        /// <summary>
        /// The properties maker is used to extract the common entity properties.
        /// </summary>
        protected readonly Func<E, IEnumerable<Tuple<string, string>>> _propertiesMaker;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TKey, TEntity}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="propertiesMaker">The properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <exception cref="ArgumentNullException">keyMaker</exception>
        protected RepositoryBase(Func<E, K> keyMaker
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            )
        {
            _keyMaker = keyMaker ?? throw new ArgumentNullException(nameof(keyMaker));

            _referenceMaker = referenceMaker ?? (e => new List<Tuple<string, string>>());
            _propertiesMaker = propertiesMaker ?? (e => new List<Tuple<string, string>>());
            VersionPolicy = versionPolicy;
        }
        #endregion

        #region TryKeyExtract(TEntity entity, out TKey key)
        /// <summary>
        /// Extracts the key from the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="key">The key for the entity</param>
        /// <returns>
        /// Returns the key.
        /// </returns>
        public virtual bool TryKeyExtract(E entity, out K key)
        {
            if (entity == null || _keyMaker == null)
            {
                key = default(K);
                return false;
            }

            key = _keyMaker(entity);
            return true;
        }
        #endregion

        #region VersionPolicy
        /// <summary>
        /// Holds the policy concerning whether the repository implements optimistic locking 
        /// and or history of entities.
        /// </summary>
        public VersionPolicy<E> VersionPolicy { get; }
        #endregion

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null);

        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and entities.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null);

        #region Class -> EntityContainer
        /// <summary>
        /// This is a private class it is used to ensure that we do not duplicate data.
        /// </summary>
        protected class EntityContainer
        {
            private long _hitCount = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="EntityContainer"/> class.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="entity">The entity.</param>
            /// <param name="references">The entity references.</param>
            /// <param name="properties">The entity properties.</param>
            /// <param name="versionId">The version id of the entity..</param>
            public EntityContainer(K key, E entity
                , IEnumerable<Tuple<string, string>> references
                , IEnumerable<Tuple<string, string>> properties
                , string versionId
                )
            {
                Key = key;

                JsonBody = JsonConvert.SerializeObject(entity);

                References = references == null ? new List<Tuple<string, string>>() : references.ToList();
                Properties = properties == null ? new List<Tuple<string, string>>() : properties.ToList();

                VersionId = versionId;
            }
            /// <summary>
            /// Contains the key.
            /// </summary>
            public K Key { get; }
            /// <summary>
            /// Gets or sets the version identifier.
            /// </summary>
            public string VersionId { get; }
            /// <summary>
            /// Contains the entity.
            /// </summary>
            public E Entity => string.IsNullOrEmpty(JsonBody) ? default(E) : JsonConvert.DeserializeObject<E>(JsonBody);

            /// <summary>
            /// Gets or sets the json body of the entity. This is used to ensure that the entity is
            /// not modified in the main collection by other processes.
            /// </summary>
            public string JsonBody { get; }

            /// <summary>
            /// Contains the entity references.
            /// </summary>
            public List<Tuple<string, string>> References { get; }
            /// <summary>
            /// Contains the entity references.
            /// </summary>
            public List<Tuple<string, string>> Properties { get; }

            /// <summary>
            /// The current 
            /// </summary>
            /// <returns>The previous count.</returns>
            public void ReadHitIncrement() => Interlocked.Increment(ref _hitCount);

            /// <summary>
            /// Gets the current hit count.
            /// </summary>
            public long ReadHitCount => _hitCount;

        }
        #endregion

    }
}
