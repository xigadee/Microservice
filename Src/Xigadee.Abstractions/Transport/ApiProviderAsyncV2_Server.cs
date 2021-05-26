using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace Xigadee
{
    //Server
    public partial class ApiProviderAsyncV2<K, E> : IRepositoryAsyncServer<K, E>
    {
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

        #region Collector
        /// <summary>
        /// This is the data collector used to collect logging and data information.
        /// </summary>
        public virtual IDataCollection Collector { get; set; }
        #endregion
        #region EntityName
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public string EntityName { get; protected set; } = typeof(E).Name;
        #endregion

        /// <summary>
        /// This is the default key maker.
        /// </summary>
        public Func<E, K> KeyMaker { get; protected set; }

        /// <summary>
        /// This is the default version policy.
        /// </summary>
        public VersionPolicy<E> VersionPolicy { get; protected set; }

        #region OnBeforeCreate...
        /// <summary>
        /// Called before and entity is created.
        /// </summary>
        /// <param name="entity">The incoming entity.</param>
        protected void OnBeforeCreateEvent(E entity) => OnBeforeCreate?.Invoke(this, entity);
        /// <summary>
        /// Occurs before and entity is created.
        /// </summary>
        public event EventHandler<E> OnBeforeCreate;
        #endregion
        #region OnAfterCreate
        /// <summary>
        /// Occurs after an entity is created/attempted.
        /// </summary>
        protected void OnAfterCreateEvent(RepositoryHolder<K, E> holder) => OnAfterCreate?.Invoke(this, holder);
        /// <summary>
        /// Occurs after an entity is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, E>> OnAfterCreate;
        #endregion

        #region OnBeforeUpdate ...
        /// <summary>
        /// Called before and entity is created.
        /// </summary>
        /// <param name="entity">The incoming entity.</param>
        protected void OnBeforeUpdateEvent(E entity) => OnBeforeUpdate?.Invoke(this, entity);
        /// <summary>
        /// Occurs before an entity is updated.
        /// </summary>
        public event EventHandler<E> OnBeforeUpdate;
        #endregion
        #region OnAfterUpdate ...
        /// <summary>
        /// Occurs after an entity is created/attempted.
        /// </summary>
        protected void OnAfterUpdateEvent(RepositoryHolder<K, E> holder) => OnAfterUpdate?.Invoke(this, holder);
        /// <summary>
        /// Occurs after an entity is updated.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, E>> OnAfterUpdate;
        #endregion

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeRead;

        #region OnAfterRead
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        protected void OnAfterReadEvent(RepositoryHolder<K, E> holder) => OnAfterRead?.Invoke(this, holder);

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, E>> OnAfterRead;
        #endregion

        /// <summary>
        /// Occurs before an entity is deleted.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeDelete;

        #region OnAfterDelete
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        protected void OnAfterDeleteEvent(RepositoryHolder<K, Tuple<K, string>> holder) => OnAfterDelete?.Invoke(this, holder);

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, Tuple<K, string>>> OnAfterDelete;
        #endregion
        /// <summary>
        /// Occurs before an entity is versioned.
        /// </summary>
        public event EventHandler<ReferenceHolder<K>> OnBeforeVersion;

        #region OnAfterVersion
        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        protected void OnAfterVersionEvent(RepositoryHolder<K, Tuple<K, string>> holder) => OnAfterVersion?.Invoke(this, holder);

        /// <summary>
        /// Occurs before an entity is read.
        /// </summary>
        public event EventHandler<RepositoryHolder<K, Tuple<K, string>>> OnAfterVersion;
        #endregion

        #region OnBeforeSearch...
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<SearchRequest> OnBeforeSearch;
        /// <summary>
        /// Called when a search event occurs.
        /// </summary>
        /// <param name="key">The search request.</param>
        protected void OnBeforeSearchEvent(SearchRequest key) => OnBeforeSearch?.Invoke(this, key);
        #endregion
        #region OnAfterSearchEvent(RepositoryHolder<SearchRequest, SearchResponse> rs)
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<SearchRequest, SearchResponse>> OnAfterSearch;
        /// <summary>
        /// Called after a search event occurs.
        /// </summary>
        /// <param name="rs">The search response.</param>
        protected void OnAfterSearchEvent(RepositoryHolder<SearchRequest, SearchResponse> rs) => OnAfterSearch?.Invoke(this, rs);

        #endregion
        #region OnAfterSearchEntityEvent(RepositoryHolder<SearchRequest, SearchResponse<E>> rs)
        /// <summary>
        /// Occurs before an entity is created.
        /// </summary>
        public event EventHandler<RepositoryHolder<SearchRequest, SearchResponse<E>>> OnAfterSearchEntity;
        /// <summary>
        /// Called after a search event occurs.
        /// </summary>
        /// <param name="rs">The search response.</param>
        protected void OnAfterSearchEntityEvent(RepositoryHolder<SearchRequest, SearchResponse<E>> rs) => OnAfterSearchEntity?.Invoke(this, rs);
        #endregion


        /// <summary>
        /// This is the generic repository type, i.e. IRepositoryAsyncServer
        /// </summary>
        public Type RepositoryServerType { get; } = typeof(IRepositoryAsyncServer<K, E>);
        /// <summary>
        /// This is the generic repository type, i.e. IRepository K,E
        /// </summary>
        public Type RepositoryType { get; } = typeof(IRepositoryAsync<K, E>);
        /// <summary>
        /// This is the entity type.
        /// </summary>
        public Type TypeEntity { get; } = typeof(E);
        /// <summary>
        /// This is the key type,
        /// </summary>
        public Type TypeKey { get; } = typeof(K);
    }
}
