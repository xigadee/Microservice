using System;
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
            :base(names ?? new SqlStoredProcedureResolver<E>(), options, fileExtractOptions){}
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
        public SqlFileExtractOptions ExtractToFolder(string folderName = null, bool createFolder = true, SqlFileExtractMode mode = SqlFileExtractMode.DefinitionAndExtensionsFiles, bool ancillary = false) => 
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

            if (eOpts.Mode != SqlFileExtractMode.DoNotExtract)
            {
                dInfoEntity.CreateFolderOrRaiseException(eOpts.FolderCreate);

                if (eOpts.RemoveOldFiles)
                    dInfoEntity.FilesRemove(eOpts.ScriptFileNames(Generator.Options).ToArray());

                switch (eOpts.Mode)
                {
                    case SqlFileExtractMode.DefinitionFileOnly:
                        dInfoEntity.WriteFile(eOpts.FileDefinition, Generator.ScriptAll);
                        break;
                    case SqlFileExtractMode.DefinitionAndExtensionsFiles:
                        dInfoEntity.WriteFile(eOpts.FileDefinition, Generator.ScriptDefinition);
                        ExtensionsWrite(dInfoEntity, eOpts);
                        break;
                    case SqlFileExtractMode.DefinitionTableAndExtensionsFiles:
                        dInfoEntity.WriteFile(eOpts.FileDefinition, Generator.ScriptDefinitionLogic);
                        dInfoEntity.WriteFile(eOpts.FileTables, Generator.ScriptDefinitionTables);
                        ExtensionsWrite(dInfoEntity, eOpts);
                        break;
                    case SqlFileExtractMode.MultipleFiles:

                        if (Generator.Options.SupportsTables.Supported)
                            dInfoEntity.WriteFile(eOpts.FileTables, Generator.ScriptDefinitionTables);

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
                dInfoEntity.WriteFile(eOpts.FileExtensions, Generator.ScriptExtensionLogic);
                dInfoEntity.WriteFile(eOpts.FileExtensionsTable, Generator.ScriptExtensionTable);
            }
        }
    }
}
