using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This class contains the logic to store and retrieve entities from a file based store.
    /// This class is not optimised for speed, but is used to provide a simple File based IO system.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityContainerFileSystem<K,E>: EntityContainerBase<K,E>
        where K : IEquatable<K>
    {
        const string cnEntityMatch = "ent";
        const string cnReferenceMatch = "ref";

        protected override void StartInternal()
        {
            base.StartInternal();

            FS = FS ?? new EntityContainerFolderStructure(Transform.EntityName, new DirectoryInfo(Environment.CurrentDirectory));

            Keys = new EntityContainerFileSystemReadOnlyCollection<K>(FS.Entity
                , ToFilter(ExtensionEntity)
                , ParseEntityKey);

            References = new EntityContainerFileSystemReadOnlyCollection<Tuple<string, string>>(FS.Reference
                , ToFilter(ExtensionReference)
                , ParseReference);

            Values = new EntityContainerFileSystemReadOnlyCollection<E>(FS.Entity
                , ToFilter(ExtensionEntity)
                , ParseEntity);
        }



        /// <summary>
        /// This is the extension string for entity file objects.
        /// </summary>
        public string ExtensionEntity { get; set; } = cnEntityMatch;
        /// <summary>
        /// This is the extension string for reference file objects.
        /// </summary>
        public string ExtensionReference { get; set; } = cnReferenceMatch;

        private string ToFilter(string extension)
        {
            return $"*.{extension}";
        }

        /// <summary>
        /// Gets or sets the file system information.
        /// </summary>
        public EntityContainerFolderStructure FS { get; set; }

        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public override int Count => (Keys as EntityContainerFileSystemReadOnlyCollection).Count;
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public override int CountReference => (References as EntityContainerFileSystemReadOnlyCollection).Count;

        /// <summary>
        /// Gets the keys collection enumeration.
        /// </summary>
        public override IEnumerable<K> Keys { get; protected set; }
        /// <summary>
        /// Gets or sets the entity reference enumeration.
        /// </summary>
        public override IEnumerable<Tuple<string, string>> References { get; protected set; }
        /// <summary>
        /// Gets or sets the entity file enumeration.
        /// </summary>
        public override IEnumerable<E> Values { get; protected set; }


        protected virtual K ParseEntityKey(FileInfo f)
        {
            var ent = ParseEntity(f);

            return Transform.KeyMaker(ent);
        }

        protected virtual E ParseEntity(FileInfo f)
        {
            return default(E);
        }

        protected virtual Tuple<string, string> ParseReference(FileInfo f)
        {
            return null;
        }

        protected virtual string PrepareKey(K key)
        {
            return $"{Transform.KeyStringMaker(key)}.{ExtensionEntity}";
        }

        protected virtual string PrepareReference(Tuple<string, string> reference)
        {
            return $"{reference.Item1}_{reference.Item2}.{ExtensionReference}";
        }

        protected virtual FileInfo GetFileInfo(K key)
        {
            string sKey = PrepareKey(key);

            return new FileInfo(Path.Combine(FS.Entity.FullName, sKey));
        }

        /// <summary>
        /// This is the entity reference.
        /// </summary>
        /// <param name="key">The entity key</param>
        /// <param name="value">The entity</param>
        /// <param name="references">The optional references.</param>
        /// <returns>
        /// 201 - Created
        /// 409 - Conflict
        /// </returns>
        public override int Add(K key, E value, IEnumerable<Tuple<string, string>> references = null)
        {
            var fi = GetFileInfo(key);
            if (fi.Exists)
                return 409;

            var jsonHolder = Transform.JsonMaker(value);

            using (var fs = fi.Open(FileMode.CreateNew, FileAccess.Write))
            {
                //fs.e
            }

            return 201;
        }

        public override int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null)
        {
            throw new NotImplementedException();
        }

        public override bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(K key)
        {
            return FS.Entity.GetFiles(PrepareKey(key), SearchOption.TopDirectoryOnly).Length>0;
        }

        /// <summary>
        /// Clears the entity and reference collection.
        /// </summary>
        public override void Clear()
        {
            FS.Entity.Delete(true);
            FS.Reference.Delete(true);
        }

        public override bool TryGetValue(K key, out E value)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryGetKey(Tuple<string, string> reference, out K key)
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
