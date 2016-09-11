#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
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

        Task<bool> ContainsKey(K key);

        Task<EntityCacheResult<E>> TryGetEntity(K key);
    }

}
