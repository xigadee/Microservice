using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class manages the key handling for the repository.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <seealso cref="Xigadee.RepositoryKeyManager" />
    public abstract class RepositoryKeyManager<K> : RepositoryKeyManager
        where K : IEquatable<K>
    {
        /// <summary>
        /// Gets the key type for the key manager.
        /// </summary>
        public Type KeyType => typeof(K);
        /// <summary>
        /// Gets or sets the key serializer function.
        /// </summary>
        public Func<K, string> Serialize { get; protected set; }
        /// <summary>
        /// Gets the key deserializer function.
        /// </summary>
        public Func<string, K> Deserialize { get; protected set; }
        /// <summary>
        /// Gets the key try deserialize function.
        /// </summary>
        public Func<string, (bool success, K key)> TryDeserialize { get; protected set; }
    }

    /// <summary>
    /// This class holds the references for the entity
    /// </summary>
    /// <typeparam name="K">The entity key type.</typeparam>
    public class ReferenceHolder<K>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public K Key { get; set; } = default(K);
        /// <summary>
        /// Gets or sets the type of the reference.
        /// </summary>
        public string RefType { get; set; }
        /// <summary>
        /// Gets or sets the reference value.
        /// </summary>
        public string RefValue { get; set; }
    }

    /// <summary>
    /// This is the base class for the key resolver.
    /// </summary>
    public abstract class RepositoryKeyManager
    {
        static ConcurrentDictionary<Type, RepositoryKeyManager> _resolvers;

        static RepositoryKeyManager()
        {
            _resolvers = new ConcurrentDictionary<Type, RepositoryKeyManager>();
        }
        /// <summary>
        /// Resolves this instance for known types..
        /// </summary>
        /// <typeparam name="K">The key type.</typeparam>
        /// <returns>Returns the resolver if it is registered.</returns>
        /// <exception cref="ArgumentOutOfRangeException">(Exception)null</exception>
        public static RepositoryKeyManager<K> Resolve<K>() where K : IEquatable<K>
        {
            if (_resolvers.ContainsKey(typeof(K)))
                return (RepositoryKeyManager<K>)_resolvers[typeof(K)];

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(K));
            if (converter != null)
            {
                return (RepositoryKeyManager<K>)_resolvers.AddOrUpdate(typeof(K), new TypeConverterRepositoryKeyManager<K>(converter), (t, r) => r);
            }

            throw new ArgumentOutOfRangeException($"RepositoryKeyManager cannot resolve for the type: {typeof(K).Name}", (Exception)null);
        }
    }

    internal class TypeConverterRepositoryKeyManager<K> : RepositoryKeyManager<K>
        where K : IEquatable<K>
    {
        TypeConverter _converter;

        public TypeConverterRepositoryKeyManager(TypeConverter converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

            if (!converter.CanConvertFrom(typeof(string)))
                throw new ArgumentOutOfRangeException();

            if (!converter.CanConvertTo(typeof(string)))
                throw new ArgumentOutOfRangeException();

            Serialize = (key) => _converter.ConvertToString(key);
            Deserialize = (s) => (K)_converter.ConvertFromString(s);

            TryDeserialize = (s) =>
            {
                try
                {
                    K key = Deserialize(s);
                    return (true, key);
                }
                catch (Exception)
                {
                    return (false, default(K));
                }
            };
        }
    }

}
