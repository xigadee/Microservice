using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is a private class it is used to ensure that we do not duplicate data.
    /// </summary>
    public class EntityContainer<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        private long _hitCount = 0;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the EntityContainer class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="references">The entity references.</param>
        /// <param name="properties">The entity properties.</param>
        /// <param name="versionId">The version id of the entity..</param>
        /// <param name="deserializer">The deserializer that converts the body to an entity.</param>
        /// <param name="serializer">The serializer that turns the entity in to a blob.</param>
        /// <param name="keyAsString">The key as a serialized string.</param>
        /// <param name="signature">The entity signature.</param>
        public EntityContainer(K key, E entity
            , IEnumerable<Tuple<string, string>> references
            , IEnumerable<Tuple<string, string>> properties
            , string versionId
            , Func<byte[], E> deserializer
            , Func<E, byte[]> serializer
            , string keyAsString
            , string signature
            )
        {
            Key = key;
            Id = keyAsString;

            Serializer = serializer ?? throw new ArgumentNullException("serializer");

            Deserializer = deserializer ?? throw new ArgumentNullException("deserializer");

            Body = Serializer(entity);

            References = references == null ? new List<Tuple<string, string>>() : references.ToList();
            Properties = properties == null ? new List<Tuple<string, string>>() : properties.ToList();

            VersionId = versionId;

            Signature = signature;
        } 
        #endregion

        /// <summary>
        /// Gets the serializer that turns the entity in to a blob.
        /// </summary>
        protected Func<E, byte[]> Serializer { get; }

        /// <summary>
        /// Gets the deserializer that converts the body to an entity.
        /// </summary>
        protected Func<byte[], E> Deserializer { get; }

        /// <summary>
        /// Contains the key.
        /// </summary>
        public K Key { get; }

        /// <summary>
        /// Gets the reference tuple.
        /// </summary>
        public Tuple<string, string> Reference => new Tuple<string, string>(Id, VersionId);
        /// <summary>
        /// Gets the key as a string.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        public string VersionId { get; }
        /// <summary>
        /// Contains the entity.
        /// </summary>
        public E Entity => (Body?.Length ?? 0) == 0 ? default(E) : Deserializer(Body);

        /// <summary>
        /// Gets or sets the json body of the entity. This is used to ensure that the entity is
        /// not modified in the main collection by other processes.
        /// </summary>
        public byte[] Body { get; }

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

        /// <summary>
        /// This is the entity signature.
        /// </summary>
        public string Signature { get; }

    }

    #region Class -> EntityContainerWrapper
    /// <summary>
    /// This wrapper class is used to stop multiple deserializations of an entity when filtering a results set.
    /// </summary>
    public class EntityContainerWrapper<K,E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Initializes a new instance of the EntityContainerWrapper class. That holds a cached deserialized version of the entity.
        /// </summary>
        /// <param name="c">The collection.</param>
        public EntityContainerWrapper(EntityContainer<K, E> c)
        {
            Container = c;
        }

        private E _entity;
        private bool _interned = false;

        /// <summary>
        /// Gets the container.
        /// </summary>
        public EntityContainer<K, E> Container { get; }

        /// <summary>
        /// Gets the cached deserialized entity.
        /// </summary>
        public E Entity
        {
            get
            {
                if (!_interned)
                {
                    _entity = Container.Entity;
                    _interned = true;
                }
                return _entity;
            }
        }

        /// <summary>
        /// Checks a property for a match.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool PropertyMatch(string key, string value)
        {
            try
            {
                var result = Container.Properties
                    .FirstOrDefault(p => p.Item1.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                return result != null && (result.Item2?.Equals(value) ?? false);
            }
            catch (Exception)
            {

                return false; ;
            }
        }

        /// <summary>
        /// Gets the first named property.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the value or null.</returns>
        public string PropertyGet(string key)
        {
            return Container.Properties.FirstOrDefault(p => p.Item1.Equals(key, StringComparison.InvariantCultureIgnoreCase))?.Item2;
        }
    }
    #endregion

}
