using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    public class EntityContainerFolderStructure
    {
        public EntityContainerFolderStructure(string entityName)
        {

        }

        public string EntityName { get; }

        public DirectoryInfo Main { get; }

        public DirectoryInfo Entity { get; }

        public DirectoryInfo Reference { get; }
    }

    /// <summary>
    /// This class contains the logic to store and retrieve entities from a file based store.
    /// This class is not optimised for speed, but is used to provide a simple File based IO system.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <seealso cref="Xigadee.EntityContainerBase{K, E}" />
    public class EntityContainerFileSystem<K,E>: EntityContainerBase<K,E>
        where K : IEquatable<K>
    {
        EntityContainerFolderStructure mFS;

        public EntityContainerFileSystem()
        {

        }

        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public override int Count => mFS.Entity.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly).Length;
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public override int CountReference => mFS.Reference.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly).Length;

        public override ICollection<K> Keys => throw new NotImplementedException();

        public override ICollection<Tuple<string, string>> References => throw new NotImplementedException();

        public override ICollection<E> Values => throw new NotImplementedException();

        public override int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(K key)
        {
            return false;
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValue(K key, out E value)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetKey(Tuple<string, string> reference, out K key)
        {
            throw new NotImplementedException();
        }

        public override int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null)
        {
            throw new NotImplementedException();
        }

        #region ContainsReference(Tuple<string, string> reference)
        /// <summary>
        /// Determines whether the collection contains the entity reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>
        /// <c>true</c> if the collection contains reference; otherwise, <c>false</c>.
        /// </returns>
        public override bool ContainsReference(Tuple<string, string> reference)
        {
            K key;
            return TryGetKey(reference, out key) && ContainsKey(key);
        } 
        #endregion
        #region Remove(Tuple<string, string> reference)
        /// <summary>
        /// Removes the specified entities by the supplied reference.
        /// </summary>
        /// <param name="reference">The reference identifier.</param>
        /// <returns>
        /// Returns true if the entity is removed successfully.
        /// </returns>
        public override bool Remove(Tuple<string, string> reference)
        {
            K key;
            return TryGetKey(reference, out key) && Remove(key);
        } 
        #endregion
        #region TryGetValue(Tuple<string, string> reference, out E value)
        /// <summary>
        /// Tries to retrieve and entity by the reference id.
        /// </summary>
        /// <param name="reference">The reference id.</param>
        /// <param name="value">The output value.</param>
        /// <returns>
        /// True if the reference exists.
        /// </returns>
        public override bool TryGetValue(Tuple<string, string> reference, out E value)
        {
            value = default(E);
            K key;

            return TryGetKey(reference, out key) && TryGetValue(key, out value);
        } 
        #endregion

    }
}
