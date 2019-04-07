using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This bridge is used to handle the impedance mismatch between derived entities.
    /// When an entity should e reported as a base entity for the interface but is implemented
    /// as an overridden entity to support extended meta data support.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The incoming base entity.</typeparam>
    /// <typeparam name="AE">The derived entity.</typeparam>
    /// <seealso cref="Xigadee.IRepositoryAsync{K, E}" />
    public class RepositoryBridge<K, E, AE> : IRepositoryAsync<K, E>
        where K: IEquatable<K>
        where E: class
        where AE : E
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBridge{K, E, AE}"/> class.
        /// </summary>
        /// <param name="repo">The repository.</param>
        public RepositoryBridge(IRepositoryAsync<K, AE> repo)
        {
            Repository = repo;
        }

        /// <summary>
        /// Gets the destination repository for the derived entity.
        /// </summary>
        public IRepositoryAsync<K, AE> Repository { get; }

        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public async Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null)
        {
            var e2 = (AE)entity;

            var rs = await Repository.Create(e2, options);

            return rs.ToBase<E>(); 
        }

        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public async Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null)
        {
            var rs = await Repository.Read(key, options);

            return rs.ToBase<E>();
        }

        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public async Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            var rs = await Repository.ReadByRef(refKey, refValue, options);

            return rs.ToBase<E>();
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public async Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null)
        {
            var e2 = (AE)entity;

            var rs = await Repository.Update(e2, options);

            return rs.ToBase<E>();
        }

        /// <summary>
        /// Searches the entity store are returns full entities.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public async Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest key, RepositorySettings options = null)
        {
            var rs = await Repository.SearchEntity(key, options);

            return rs.ToBase((e) =>
            {
                var sr =  new SearchResponse<E>();

                sr.Etag = rs.Entity.Etag;
                sr.Skip = rs.Entity.Skip;
                sr.Top = rs.Entity.Top;

                sr.Data = rs.Entity.Data?.Select((el) => el as E).ToList();
    
                return sr;
            });
        }

        /// <summary>
        /// Deletes the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null)
        {
            return Repository.Delete(key, options);
        }
        /// <summary>
        /// Deletes the entity by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            return Repository.DeleteByRef(refKey, refValue, options);
        }
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest key, RepositorySettings options = null)
        {
            return Repository.Search(key, options);
        }
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null)
        {
            return Repository.Version(key, options);
        }
        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options settings.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            return Repository.VersionByRef(refKey, refValue, options);
        }
    }
}
