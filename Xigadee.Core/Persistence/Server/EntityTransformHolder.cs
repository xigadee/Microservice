using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to transform an entity within the persistence handler.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityTransformHolder<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This function is used by optimistic locking, it is used to define the version id for the entity.
        /// </summary>
        public VersionPolicy<E> Version { get; set; }
        /// <summary>
        /// This is the entity name.
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public Func<E, K> KeyMaker { get; set; }
        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public Func<string, E> EntityDeserializer { get; set; }        
        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public Func<E, string> EntitySerializer { get; set; }
        /// <summary>
        /// This function can be used to extract the references from an incoming entity to allow for caching.
        /// </summary>
        public Func<E, IEnumerable<Tuple<string, string>>> ReferenceMaker { get; set; }
        /// <summary>
        /// This method makes a string reference from the key.
        /// </summary>
        public Func<K, string> KeySerializer { get; set; }
        /// <summary>
        /// This method deserializes the string version of the key to the object.
        /// </summary>
        public Func<string, K> KeyDeserializer { get; set; }
    }
}
