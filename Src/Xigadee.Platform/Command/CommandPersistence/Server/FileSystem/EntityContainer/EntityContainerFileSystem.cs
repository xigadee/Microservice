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
        #region Declarations
        const string cnEntityMatch = "ent";
        const string cnReferenceMatch = "ref";
        #endregion

        #region StartInternal()
        /// <summary>
        /// This method starts the service and sets the file based collections.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();

            FS = FS ?? new EntityContainerFolderStructure(Transform.EntityName, new DirectoryInfo(Environment.CurrentDirectory));

            Keys = new EntityContainerFileSystemReadOnlyCollection<K>(FS.Entity
                , ToSearchFilter(FileExtensionEntity)
                , ExtractKey);

            References = new EntityContainerFileSystemReadOnlyCollection<Tuple<string, string>>(FS.Reference
                , ToSearchFilter(FileExtensionReference)
                , ExtractReference);

            Values = new EntityContainerFileSystemReadOnlyCollection<E>(FS.Entity
                , ToSearchFilter(FileExtensionEntity)
                , ExtractEntity);
        }
        #endregion

        #region FS
        /// <summary>
        /// Gets or sets the file system information.
        /// </summary>
        public EntityContainerFolderStructure FS { get; set; }
        #endregion

        #region FileExtension Entity/Reference
        /// <summary>
        /// This is the extension string for entity file objects.
        /// </summary>
        public string FileExtensionEntity { get; set; } = cnEntityMatch;
        /// <summary>
        /// This is the extension string for reference file objects.
        /// </summary>
        public string FileExtensionReference { get; set; } = cnReferenceMatch;
        #endregion
        #region ToSearchFilter(string extension)
        /// <summary>
        /// Creates the search filter string.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>A string that case be used to scan for the preferred extension type.</returns>
        protected virtual string ToSearchFilter(string extension)
        {
            return $"*.{extension}";
        } 
        #endregion

        #region Count
        /// <summary>
        /// This is the number of entities in the collection.
        /// </summary>
        public override int Count => (Keys as EntityContainerFileSystemReadOnlyCollection).Count;
        #endregion
        #region CountReference
        /// <summary>
        /// This is the number of entity references in the collection.
        /// </summary>
        public override int CountReference => (References as EntityContainerFileSystemReadOnlyCollection).Count; 
        #endregion


        protected virtual K ExtractKey(FileInfo f)
        {
            var ent = ExtractEntity(f);
            return Transform.KeyMaker(ent);
        }

        protected virtual E ExtractEntity(FileInfo f)
        {
            return default(E);
        }

        protected virtual Tuple<string, string> ExtractReference(FileInfo f)
        {
            return null;
        }

        protected virtual string PrepareFileName(K key)
        {
            return $"{Transform.KeyStringMaker(key)}.{FileExtensionEntity}";
        }

        protected virtual string PrepareFileName(Tuple<string, string> reference)
        {
            return $"{reference.Item1}_{reference.Item2}.{FileExtensionReference}";
        }

        protected virtual FileInfo FileInfoPrepare(K key)
        {
            string sKey = PrepareFileName(key);

            return new FileInfo(Path.Combine(FS.Entity.FullName, sKey));
        }

        protected virtual FileInfo FileInfoPrepare(Tuple<string,string> reference)
        {
            string sKey = PrepareFileName(reference);

            return new FileInfo(Path.Combine(FS.Entity.FullName, sKey));
        }


        protected virtual void WriteEntity(FileStream fs, E entity)
        {
            var json = Transform.JsonMaker(entity);
            var holder = SerializationHolder.CreateWithObject(entity);
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
            ValidateServiceStarted();

            var fi = FileInfoPrepare(key);
            if (fi.Exists)
                return 409;

            using (var fs = fi.Open(FileMode.CreateNew, FileAccess.Write))
            {
                WriteEntity(fs, value);
            }

            return 201;
        }


        public override int Update(K key, E newEntity, IEnumerable<Tuple<string, string>> newReferences = null)
        {
            ValidateServiceStarted();

            var fi = FileInfoPrepare(key);
            if (!fi.Exists)
                return 404;

            using (var fs = fi.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                WriteEntity(fs, newEntity);
            }

            return 200;
        }

        public override bool Remove(K key)
        {
            ValidateServiceStarted();

            var fi = FileInfoPrepare(key);
            if (!fi.Exists)
                return false;

            return false;
        }

        public override bool ContainsKey(K key)
        {
            ValidateServiceStarted();
            return FS.Entity.GetFiles(PrepareFileName(key), SearchOption.TopDirectoryOnly).Length>0;
        }

        /// <summary>
        /// Clears the entity and reference collection.
        /// </summary>
        public override void Clear()
        {
            ValidateServiceStarted();
            FS.Entity.Delete(true);
            FS.Reference.Delete(true);
        }

        public override bool TryGetValue(K key, out E value)
        {
            ValidateServiceStarted();
            throw new NotImplementedException();
        }

        public virtual bool TryGetKey(Tuple<string, string> reference, out K key)
        {
            ValidateServiceStarted();
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
