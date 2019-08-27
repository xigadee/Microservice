using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the extract options.
    /// </summary>
    public class SqlFileExtractOptions
    {
        /// <summary>
        /// The root folder name.
        /// </summary>
        public string FolderName { get; set; }
        /// <summary>
        /// Specifies that the folders should be created is they do not exist.
        /// </summary>
        public bool FolderCreate { get; set; }
        /// <summary>
        /// Specifies whether the ancillary SQL should be written out.
        /// </summary>
        public bool ExtractAncillary { get; set; }
        /// <summary>
        /// Specifies whether the entities should be extracted.
        /// </summary>
        public SqlFileExtractMode Mode { get; set; } = SqlFileExtractMode.DefinitionFileOnly;

        public bool RemoveOldFiles { get; set; } = true;

        public string FileAncillary { get; set; } = "Ancillary.sql";

        public string FileCreate { get; set; } = "EntityCreate.sql";
        public string FileRead { get; set; } = "EntityRead.sql";
        public string FileUpdate { get; set; } = "EntityUpdate.sql";
        public string FileDelete { get; set; } = "EntityDelete.sql";
        public string FileVersion { get; set; } = "EntityVersion.sql";
        public string FileUpsert { get; set; } = "EntityUpsert.sql";
        public string FileHistory { get; set; } = "EntityHistory.sql";

        public string FileTables { get; set; } = "Tables.sql";

        public string FileSearch { get; set; } = "Search.sql";
        public string FileSearchJson { get; set; } = "SearchJson.sql";

        public string FileDefinition { get; set; } = "Definition.sql";
        public string FileExtensions { get; set; } = "Extensions.sql";
        public string FileExtensionsTable { get; set; } = "ExtensionsTable.sql";

        /// <summary>
        /// This is the list of supported files names.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> ScriptFileNames(RepositorySqlJsonOptions options)
        {
            yield return FileCreate;
            yield return FileRead;
            yield return FileUpdate;
            yield return FileDelete;
            yield return FileVersion;

            yield return FileUpsert;
            yield return FileHistory;

            if (options.SupportsTables.Supported)
                yield return FileTables;

            yield return FileSearch;
            yield return FileSearchJson;

            yield return FileDefinition;

            if (options.SupportsExtension.Supported)
            {
                yield return FileExtensions;
                yield return FileExtensionsTable;
            }
        }
    }

    /// <summary>
    /// This is the extraction policy.
    /// </summary>
    public enum SqlFileExtractMode
    {
        /// <summary>
        /// Does not extract any of the entity sql files, but does extract ancillary Sql.
        /// </summary>
        DoNotExtract,
        /// <summary>
        /// Extract as a single file.
        /// </summary>
        DefinitionFileOnly,
        /// <summary>
        /// Extract as a single file, but seperate Extensions if specified.
        /// </summary>
        DefinitionAndExtensionsFiles,

        DefinitionTableAndExtensionsFiles,
        /// <summary>
        /// Extract as multiple files.
        /// </summary>
        MultipleFiles
    }

    /// <summary>
    /// This helper class is used to generate files.
    /// </summary>
    public static class SqlFileJsonGeneratorHelper
    {
        /// <summary>
        /// This helper method writes for the file.
        /// </summary>
        /// <param name="dInfoEntity">The directory info.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="data">The text file content.</param>
        public static void WriteFile(this DirectoryInfo dInfoEntity, string fileName, string data)
        {
            var filePath = Path.Combine(dInfoEntity.FullName, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var fi = File.CreateText(filePath))
                fi.Write(data);

        }

        /// <summary>
        /// This extension method creates the folder or raises an exception if the folder does not exist.
        /// </summary>
        /// <param name="dInfo">The directory info.</param>
        /// <param name="create">Specifies whether the folder should be created if it does not exist.</param>
        public static void CreateFolderOrRaiseException(this DirectoryInfo dInfo, bool create)
        {
            if (!dInfo.Exists)
                if (create)
                    Directory.CreateDirectory(dInfo.FullName);
                else
                    throw new IOException($"Folder does not exist {dInfo.FullName}");
        }

        /// <summary>
        /// This method removes the files with the names specified.
        /// </summary>
        /// <param name="dInfo">The directory information</param>
        /// <param name="files">The file name collection.</param>
        public static void FilesRemove(this DirectoryInfo dInfo, params string[] files)
        {
            if (!dInfo.Exists)
                return;

            foreach (var file in files)
            {
                var fullName = Path.Combine(dInfo.FullName, file);
                if (File.Exists(fullName))
                    File.Delete(fullName);
            }

        }
    }
}
