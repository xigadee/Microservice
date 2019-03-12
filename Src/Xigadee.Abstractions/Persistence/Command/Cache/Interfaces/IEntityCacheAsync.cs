using System;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the interface that is used to share cached content.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The cache entity.</typeparam>
    public interface IEntityCacheAsync<K, E>: ICacheComponent
        where K : IEquatable<K>
    {
        //event EventHandler<CollectionCacheChangeEventArgs> Changed;
        /// <summary>
        /// An async method to determine if the cache contains the entity.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns true if the cache contains the key.</returns>
        Task<bool> ContainsKey(K key);
        /// <summary>
        /// This method retrieves the entity from the cache if present.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <returns>Returns a cache result which contains the entity if it's present in the cache.</returns>
        Task<EntityCacheResult<E>> TryGetEntity(K key);
    }

}
