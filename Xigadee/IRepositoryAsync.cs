#region using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a default repositry async interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null);

        Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null);

        Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null);

        Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null);

        Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null);

        Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null);

        Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null);

        Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null);

        Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest key, RepositorySettings options = null);

        
    }
}
