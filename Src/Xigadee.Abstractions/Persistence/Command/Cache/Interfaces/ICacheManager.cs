using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by the cache service that can be attached to an existing persistence agent
    /// to provide fast cache support when needed.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface ICacheManager<K,E>
        where K : IEquatable<K>
    {
        bool IsReadOnly { get; }
        bool IsActive { get; }

        Task<bool> Write(E entity, TimeSpan? expiry = null);
        Task<bool> Write(EntityTransformHolder<K, E> transform, E entity, TimeSpan? expiry = null);
        Task<bool> WriteReference(Tuple<string, string> reference, K key, string version, TimeSpan? expiry = null);
        Task<bool> WriteReference(EntityTransformHolder<K, E> transform, Tuple<string, string> reference, K key, string version, TimeSpan? expiry = null);
        Task<bool> WriteVersion(K key, string version, TimeSpan? expiry = null);
        Task<bool> WriteVersion(EntityTransformHolder<K, E> transform, K key, string version, TimeSpan? expiry = null);
        Task<bool> Delete(K key);
        Task<bool> Delete(EntityTransformHolder<K, E> transform, K key);

        Task<bool> Delete(Tuple<string, string> reference);
        Task<bool> Delete(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        Task<IResponseHolder<E>> Read(K key);
        Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, K key);

        Task<IResponseHolder<E>> Read(Tuple<string, string> reference);
        Task<IResponseHolder<E>> Read(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);

        Task<IResponseHolder<Tuple<K, string>>> VersionRead(K key);
        Task<IResponseHolder<Tuple<K,string>>> VersionRead(EntityTransformHolder<K, E> transform, K key);

        Task<IResponseHolder<Tuple<K, string>>> VersionRead(Tuple<string, string> reference);
        Task<IResponseHolder<Tuple<K, string>>> VersionRead(EntityTransformHolder<K, E> transform, Tuple<string, string> reference);
    }
}
