using System;
using System.Diagnostics;
using System.IO;

namespace Xigadee
{
    /// <summary>
    /// This class contains the folder structure for the entity storage.
    /// </summary>
    [DebuggerDisplay("{EntityName} @ {Main.FullName}")]
    public class EntityContainerFolderStructure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContainerFolderStructure"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="mainFolder">The main folder.</param>
        /// <param name="entityFolder">The entity folder, if not defined then main\entity.</param>
        /// <param name="referenceFolder">The reference folder, if not defined then main\entityName\References.</param>
        /// <exception cref="ArgumentNullException">entityName - EntityName cannot be null. This is needed for the folder structure.</exception>
        public EntityContainerFolderStructure(string entityName, DirectoryInfo mainFolder
            , DirectoryInfo entityFolder = null
            , DirectoryInfo referenceFolder = null
            )
        {
            EntityName = entityName?.ToLowerInvariant() ?? throw new ArgumentNullException("entityName", "EntityName cannot be null. This is needed for the folder structure.");

            Main = mainFolder ?? throw new ArgumentNullException("mainFolder","The main folder parameter must be set.");

            if (!Main.Exists)
                Main.Create();

            Entity = entityFolder ?? new DirectoryInfo(Path.Combine(Main.FullName, EntityName));
            if (!Entity.Exists)
                Entity.Create();

            Reference = referenceFolder ?? new DirectoryInfo(Path.Combine(Entity.FullName, "References"));
            if (!Reference.Exists)
                Reference.Create();
        }
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public string EntityName { get; }
        /// <summary>
        /// Gets the root main directory info..
        /// </summary>
        public DirectoryInfo Main { get; }
        /// <summary>
        /// Gets the entity folder information.
        /// </summary>
        public DirectoryInfo Entity { get; }
        /// <summary>
        /// Gets the reference folder information.
        /// </summary>
        public DirectoryInfo Reference { get; }
    }
}
