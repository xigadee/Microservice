using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the base repository holder.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public abstract class RepositoryBase<TKey, TEntity> : IRepositoryAsyncServer<TKey, TEntity>
        where TKey : IEquatable<TKey>
    {
        #region Events
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<SearchRequest> OnBeforeSearch;

        /// <summary>
        /// Occurs before and entity is created.
        /// </summary>
        public event EventHandler<TEntity> OnBeforeCreate;
        /// <summary>
        /// Occurs before an entity is updated.
        /// </summary>
        public event EventHandler<TEntity> OnBeforeUpdate;
        /// <summary>
        /// Occurs after an entity is created.
        /// </summary>
        public event EventHandler<TEntity> OnAfterCreate;
        /// <summary>
        /// Occurs after an entity is updated.
        /// </summary>
        public event EventHandler<TEntity> OnAfterUpdate;
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<ReferenceHolder<TKey>> OnBeforeRead;
        /// <summary>
        /// Occurs before an entity is deleted.
        /// </summary>
        public event EventHandler<ReferenceHolder<TKey>> OnBeforeDelete;
        /// <summary>
        /// Occurs before an entity is versioned.
        /// </summary>
        public event EventHandler<ReferenceHolder<TKey>> OnBeforeVersion;

        #region OnEntityEvent(EventType type, TEntity entity)
        protected enum EntityEventType
        {
            BeforeCreate,
            BeforeUpdate,
            AfterCreate,
            AfterUpdate
        }
        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="entity">The entity.</param>
        protected void OnEntityEvent(EntityEventType type, Func<TEntity> entity)
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
        protected void OnKeyEvent(KeyEventType type, TKey key = default(TKey), string refType = null, string refValue = null)
        {
            var holder = new ReferenceHolder<TKey>() { Key = key, RefType = refType, RefValue = refValue };

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
        protected readonly Func<TEntity, TKey> _keyMaker;

        /// <summary>
        /// The reference maker used to make the reference values from an entity.
        /// </summary>
        protected readonly Func<TEntity, IEnumerable<Tuple<string, string>>> _referenceMaker;
        /// <summary>
        /// The properties maker is used to extract the common entity properties.
        /// </summary>
        protected readonly Func<TEntity, IEnumerable<Tuple<string, string>>> _propertiesMaker;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TKey, TEntity}"/> class.
        /// </summary>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="keyManager">The key manager.</param>
        /// <param name="referenceMaker">The reference maker.</param>
        /// <param name="propertiesMaker">The properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <exception cref="ArgumentNullException">keyMaker</exception>
        protected RepositoryBase(Func<TEntity, TKey> keyMaker
            , Func<TEntity, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<TEntity, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<TEntity> versionPolicy = null
            , RepositoryKeyManager<TKey> keyManager = null
            )
        {
            _keyMaker = keyMaker ?? throw new ArgumentNullException(nameof(keyMaker));
            KeyManager = keyManager ?? RepositoryKeyManager.Resolve<TKey>() ?? throw new ArgumentNullException(nameof(keyManager));

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
        public virtual bool TryKeyExtract(TEntity entity, out TKey key)
        {
            if (entity == null || _keyMaker == null)
            {
                key = default(TKey);
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
        public VersionPolicy<TEntity> VersionPolicy { get; }
        #endregion
        #region KeyManager
        /// <summary>
        /// The key manager.
        /// </summary>
        public virtual RepositoryKeyManager<TKey> KeyManager { get; } 
        #endregion

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, TEntity>> Create(TEntity entity, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, Tuple<TKey, string>>> Delete(TKey key, RepositorySettings options = null);
        /// <summary>
        /// Deletes the entity by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, Tuple<TKey, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, TEntity>> Read(TKey key, RepositorySettings options = null);
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, TEntity>> ReadByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, TEntity>> Update(TEntity entity, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, Tuple<TKey, string>>> Version(TKey key, RepositorySettings options = null);
        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<TKey, Tuple<TKey, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null);
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public abstract Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest key, RepositorySettings options = null);
    }
}
