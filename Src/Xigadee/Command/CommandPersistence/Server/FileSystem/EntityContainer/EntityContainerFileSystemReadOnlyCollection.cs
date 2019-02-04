using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class is used to enumerate through a file system and return a list of FileInfos.
    /// </summary>
    public class EntityContainerFileSystemReadOnlyCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContainerFileSystemReadOnlyCollection"/> class.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="fileMatchPattern">The file match pattern.</param>
        /// <param name="filter">The FileInfo filter function. If this is left null then the system will filter on whether the file exists.</param>
        public EntityContainerFileSystemReadOnlyCollection(DirectoryInfo folder, string fileMatchPattern, Func<FileInfo, bool> filter = null)
        {
            Folder = folder;
            FileMatchPattern = fileMatchPattern;
            Filter = filter ?? ((FileInfo f) => f.Exists);
        }
        /// <summary>
        /// Gets the folder that contains the entities.
        /// </summary> 
        protected DirectoryInfo Folder { get; }
        /// <summary>
        /// Gets the file match pattern.
        /// </summary>
        protected string FileMatchPattern { get; }
        /// <summary>
        /// Gets the FileInfo filter function.
        /// </summary>
        protected Func<FileInfo, bool> Filter { get; }
        /// <summary>
        /// Gets a collection of file information for the collection.
        /// </summary>
        public virtual FileInfo[] Files => Folder.GetFiles(FileMatchPattern, SearchOption.TopDirectoryOnly).Where(Filter).ToArray();
        /// <summary>
        /// Gets the count of filtered files.
        /// </summary>
        public virtual int Count => Files.Length;

    }

    /// <summary>
    /// This class is used to enumerate through a file system and return a list of entities.
    /// </summary>
    /// <typeparam name="R">The enumeration object type.</typeparam>
    public class EntityContainerFileSystemReadOnlyCollection<R>: EntityContainerFileSystemReadOnlyCollection, IEnumerable<R>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContainerFileSystemReadOnlyCollection{R}"/> class.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="fileMatchPattern">The file match pattern.</param>
        /// <param name="convert">The FileInfo convert function.</param>
        /// <param name="filter">The FileInfo filter function. If this is left null then the system will filter on whether the file exists.</param>
        public EntityContainerFileSystemReadOnlyCollection(DirectoryInfo folder, string fileMatchPattern, Func<FileInfo, R> convert, Func<FileInfo, bool> filter = null)
            :base(folder, fileMatchPattern, filter)
        {
            Convert = convert;
        }

        /// <summary>
        /// Gets the FileInfo convert function..
        /// </summary>
        protected Func<FileInfo, R> Convert { get; }

        /// <summary>
        /// Returns an enumerator that iterates through the file system.
        /// </summary>
        /// <returns>
        /// An enumerator that contains the converted File system file to system entity.
        /// </returns>
        public IEnumerator<R> GetEnumerator()
        {
            return Files.Select(Convert).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
