using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This the the generic sql generator for the specified entity type.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class SqlJsonGenerator<E>: SqlJsonGenerator
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="names">This is the optional naming class. If this is null a class will be created with the default settings.</param>
        /// <param name="options">This is the SQL generation options settings.</param>
        /// <param name="fileExtractOptions">These are file extraction options.</param>
        public SqlJsonGenerator(SqlStoredProcedureResolver<E> names = null, RepositorySqlJsonOptions options = null, SqlFileExtractOptions fileExtractOptions = null)
            :base(names ?? new SqlStoredProcedureResolver<E>(), options, fileExtractOptions)
        {
        }
    }
    /// <summary>
    /// This class is used to generate the SQL for a specific entity.
    /// </summary>
    public class SqlJsonGenerator
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="names">This is the optional naming class. If this is null a class will be created with the default settings.</param>
        /// <param name="options">This is the SQL generation options settings.</param>
        /// <param name="fileExtractOptions">These are file extraction options.</param>
        public SqlJsonGenerator(SqlStoredProcedureResolver names, RepositorySqlJsonOptions options = null, SqlFileExtractOptions fileExtractOptions = null)
        {
            Names = names ?? throw new ArgumentNullException("names");

            Generator = new RepositorySqlJsonGenerator(names, options);

            FileExtractOptions = fileExtractOptions;
        }

        /// <summary>
        /// This is the SQL naming class.
        /// </summary>
        public SqlStoredProcedureResolver Names { get; }

        /// <summary>
        /// This is the Sql generator class
        /// </summary>
        public RepositorySqlJsonGenerator Generator { get; }

        /// <summary>
        /// This class contains the Sql file extraction options.
        /// </summary>
        public SqlFileExtractOptions FileExtractOptions { get; }

        /// <summary>
        /// This is a shortcut to the main SQL generation.
        /// </summary>
        public string SqlEntity => Generator.ScriptEntity;

        /// <summary>
        /// This is the SQL script definition without the base extension SQL code.
        /// </summary>
        public string ScriptEntityWithoutExtension => Generator.ScriptEntityWithoutExtension;

        /// <summary>
        /// This is the supported SQL extension script that you can customize for your data uses.
        /// </summary>
        public string ScriptExtensionLogic => Generator.ScriptExtensionLogic;

        /// <summary>
        /// This is the shortcut to the Extension Table SQL.
        /// </summary>
        public string ScriptExtensionTable => Generator.ScriptExtensionTable;

        /// <summary>
        /// This method extracts to the folder.
        /// </summary>
        /// <returns></returns>
        public SqlFileExtractOptions ExtractToFolder() => ExtractToFolder(FileExtractOptions ?? throw new ArgumentNullException("FileExtractOptions is null"));

        /// <summary>
        /// This method writes out the SQL definitions to set of SQL files.
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <param name="createFolder"></param>
        /// <param name="mode">The file extract mode. The default is a single file and a set of extension files.</param>
        /// <param name="ancillary">Include the ancillary definitions.</param>
        public SqlFileExtractOptions ExtractToFolder(string folderName = null, bool createFolder = true, SqlExtractMode mode = SqlExtractMode.SingleFileAndExtensions, bool ancillary = false) => 
            ExtractToFolder(new SqlFileExtractOptions()
            {
                  FolderName = string.IsNullOrWhiteSpace(folderName)?$"{Environment.CurrentDirectory}\\SqlEntities":folderName
                , FolderCreate = createFolder
                , Mode = mode
                , ExtractAncillary = ancillary
            });

        /// <summary>
        /// This method writes out the SQL definitions to set of SQL files.
        /// </summary>
        /// <param name="eOpts">The file extract options.</param>
        public SqlFileExtractOptions ExtractToFolder(SqlFileExtractOptions eOpts)
        {
            var dInfo = new DirectoryInfo(eOpts.FolderName);
            var dInfoEntity = new DirectoryInfo(Path.Combine(eOpts.FolderName, Names.EntityName));

            dInfo.CreateFolderOrRaiseException(eOpts.FolderCreate);

            if (eOpts.RemoveOldFiles)
                dInfo.FilesRemove(eOpts.FileAncillary);

            if (eOpts.ExtractAncillary)
                dInfo.WriteFile(eOpts.FileAncillary, Generator.ScriptAncillary);

            if (!dInfo.Exists && !eOpts.FolderCreate)
                throw new IOException($"Folder does not exist {eOpts.FolderName}");

            if (eOpts.Mode != SqlExtractMode.DoNotExtract)
            {
                dInfoEntity.CreateFolderOrRaiseException(eOpts.FolderCreate);

                if (eOpts.RemoveOldFiles)
                    dInfoEntity.FilesRemove(eOpts.ScriptFileNames(Generator.Options).ToArray());

                switch (eOpts.Mode)
                {
                    case SqlExtractMode.SingleFile:
                        dInfoEntity.WriteFile(eOpts.FileDefinition, Generator.ScriptEntity);
                        break;
                    case SqlExtractMode.SingleFileAndExtensions:
                        dInfoEntity.WriteFile(eOpts.FileDefinition, ScriptEntityWithoutExtension);
                        ExtensionsWrite(dInfoEntity, eOpts);
                        break;
                    case SqlExtractMode.MultipleFiles:

                        if (Generator.Options.SupportsTables.Supported)
                            dInfoEntity.WriteFile(eOpts.FileTables, Generator.ScriptTables);

                        dInfoEntity.WriteFile(eOpts.FileCreate, Generator.EntityCreate);
                        dInfoEntity.WriteFile(eOpts.FileRead, Generator.EntityRead);
                        dInfoEntity.WriteFile(eOpts.FileUpdate, Generator.EntityUpdate);
                        dInfoEntity.WriteFile(eOpts.FileDelete, Generator.EntityDelete);
                        dInfoEntity.WriteFile(eOpts.FileVersion, Generator.EntityVersion);

                        dInfoEntity.WriteFile(eOpts.FileUpsert, Generator.EntityUpsert);
                        dInfoEntity.WriteFile(eOpts.FileHistory, Generator.EntityHistory);

                        //Search
                        dInfoEntity.WriteFile(eOpts.FileSearch, Generator.ScriptSearch);
                        dInfoEntity.WriteFile(eOpts.FileSearchJson, Generator.ScriptSearchJson);
                        //Extensions
                        ExtensionsWrite(dInfoEntity, eOpts);

                        break;
                }
            }

            return eOpts;
        }

        private void ExtensionsWrite(DirectoryInfo dInfoEntity, SqlFileExtractOptions eOpts)
        {
            if (Generator.Options.SupportsExtension.Supported)
            {
                dInfoEntity.WriteFile(eOpts.FileExtensions, ScriptExtensionLogic);
                dInfoEntity.WriteFile(eOpts.FileExtensionsTable, ScriptExtensionTable);
            }
        }
    }

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
        public SqlExtractMode Mode { get; set; } = SqlExtractMode.SingleFile;

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
    public enum SqlExtractMode
    {
        /// <summary>
        /// Does not extract any of the entity sql files, but does extract ancillary Sql.
        /// </summary>
        DoNotExtract,
        /// <summary>
        /// Extract as a single file.
        /// </summary>
        SingleFile,
        /// <summary>
        /// Extract as a single file, but seperate Extensions if specified.
        /// </summary>
        SingleFileAndExtensions,
        /// <summary>
        /// Extract as multiple files.
        /// </summary>
        MultipleFiles
    }

    /// <summary>
    /// This helper class is used to generate files.
    /// </summary>
    public static class SqlJsonGeneratorFileHelper
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
