using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to generate the SQL Schema for the JSON based tables and stored procedures.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositorySqlJsonGenerator<E>
    {
        #region Declarations
        /// <summary>
        /// This is the root namespace for embedded templates.
        /// </summary>
        private const string msRoot = "Xigadee.Persistence.Json.SqlTemplates";

        private SqlStoredProcedureResolver<E> _resolver;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        public RepositorySqlJsonGenerator(SqlStoredProcedureResolver<E> resolver)
        {
            _resolver = resolver;
        } 
        #endregion

        #region ProcessTemplate(string resourceName)
        /// <summary>
        /// This method reads the internal template 
        /// </summary>
        /// <param name="resourceName">The resource partial path.</param>
        /// <returns>Returns the script.</returns>
        public string ProcessTemplate(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream($"{msRoot}.{resourceName}"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return ReplaceTokens(result);
            }
        }
        #endregion
        #region ReplaceTokens(string script)
        /// <summary>
        /// This method replaces the magic tokens in the Sql Script templates.
        /// </summary>
        /// <param name="script">The script to process.</param>
        /// <returns>Returns the script with the tokens replaced.</returns>
        public string ReplaceTokens(string script)
        {
            script = script.Replace("{EntityName}", _resolver.EntityName);

            script = script.Replace("{NamespaceTable}", _resolver.TableSchemaName);

            script = script.Replace("{NamespaceExternal}", _resolver.ExternalSchemaName);

            //Set the stored procedure names.
            Enum.GetValues(typeof(RepositoryMethod)).Cast<RepositoryMethod>().ForEach((m) => script = script.Replace($"{{sp{m.ToString()}}}", _resolver[m]));

            script = script.Replace("{spUpsertRP}", _resolver.StoredProcedureNameUpsertRP);

            script = script.Replace("{spHistory}", _resolver.StoredProcedureNameHistory);

            return script;
        }
        #endregion

        #region SBWrap(Action<StringBuilder> write, bool createoralter = false)
        /// <summary>
        /// This helper method is used to output the SQL script.
        /// </summary>
        /// <param name="write">The action.</param>
        /// <param name="createoralter">The optional parameter that replaces the CREATE PROCEDURE sql command with an ALTER command. The default is false - no replacement.</param>
        /// <returns>Returns the SQL script.</returns>
        public static string SBWrap(Action<StringBuilder> write, bool createoralter = false)
        {
            var sb = new StringBuilder();

            write(sb);

            if (createoralter)
                return sb.ToString().Replace("CREATE PROCEDURE", "CREATE OR ALTER PROCEDURE");
            else
                return sb.ToString();
        }
        #endregion

        #region Scripts Ancillary
        public string AncillarySchema => ProcessTemplate("Ancillary.Schema.sql");

        public string AncillaryKvpTableType => ProcessTemplate("Ancillary.KvpTableType.sql");

        public string AncillaryIdTableType => ProcessTemplate("Ancillary.IdTableType.sql");

        /// <summary>
        /// These are the CRUD stored procedures.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        protected void Append_SQL_Ancillary(StringBuilder sb)
        {
            sb.AppendLine(AncillarySchema);
            sb.AppendLine("GO");
            sb.AppendLine(AncillaryKvpTableType);
            sb.AppendLine("GO");
            sb.AppendLine(AncillaryIdTableType);
            sb.AppendLine("GO");
        }

        /// <summary>
        /// This method returns the Ancillary SQL for the application. This is the set of helper definitions that are needed to make the stored procedures work.
        /// </summary>
        /// <returns></returns>
        public string ScriptAncillary => SBWrap(Append_SQL_Ancillary);
        #endregion
        #region Scripts Tables
        public string TableEntity => ProcessTemplate("Tables.Table.sql");

        public string TableHistory => ProcessTemplate("Tables.TableHistory.sql");

        public string TableProperty => ProcessTemplate("Tables.TableProperty.sql");

        public string TablePropertyKey => ProcessTemplate("Tables.TablePropertyKey.sql");

        public string TableReference => ProcessTemplate("Tables.TableReference.sql");

        public string TableReferenceKey => ProcessTemplate("Tables.TableReferenceKey.sql");

        public string TableSearchHistory => ProcessTemplate("Tables.TableSearchHistory.sql");

        /// <summary>
        /// These are the SQL tables.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        protected void Append_SQL_Tables(StringBuilder sb)
        {
            sb.AppendLine(TableReferenceKey);
            sb.AppendLine("GO");
            sb.AppendLine(TablePropertyKey);
            sb.AppendLine("GO");
            sb.AppendLine(TableEntity);
            sb.AppendLine("GO");
            sb.AppendLine(TableHistory);
            sb.AppendLine("GO");
            sb.AppendLine(TableProperty);
            sb.AppendLine("GO");
            sb.AppendLine(TableReference);
            sb.AppendLine("GO");
            sb.AppendLine(TableSearchHistory);
            sb.AppendLine("GO");
        }

        /// <summary>
        /// This method returns the SQL scripts to create the necessary tables.
        /// </summary>
        /// <returns>Returns a SQL script.</returns>
        public string ScriptTables => SBWrap(Append_SQL_Tables);
        #endregion
        #region Scripts Stored Procedures - CRUD
        /// <summary>
        /// This is the entity create stored procedure.
        /// </summary>
        public string SpCreate => ProcessTemplate("StoredProcedures.spCreate.sql");
        /// <summary>
        /// This is the entity delete stored procedure.
        /// </summary>
        public string SpDelete => ProcessTemplate("StoredProcedures.spDelete.sql");
        /// <summary>
        /// This is the entity delete by reference stored procedure.
        /// </summary>
        public string SpDeleteByRef => ProcessTemplate("StoredProcedures.spDeleteByRef.sql");
        /// <summary>
        /// This is the entity read stored procedure.
        /// </summary>
        public string SpRead => ProcessTemplate("StoredProcedures.spRead.sql");
        /// <summary>
        /// This is the entity read by reference stored procedure.
        /// </summary>
        public string SpReadByRef => ProcessTemplate("StoredProcedures.spReadByRef.sql");
        /// <summary>
        /// This is the entity update stored procedure.
        /// </summary>
        public string SpUpdate => ProcessTemplate("StoredProcedures.spUpdate.sql");
        /// <summary>
        /// This is the entity version check stored procedure.
        /// </summary>
        public string SpVersion => ProcessTemplate("StoredProcedures.spVersion.sql");
        /// <summary>
        /// This is the entity version by reference check stored procedure.
        /// </summary>
        public string SpVersionByRef => ProcessTemplate("StoredProcedures.spVersionByRef.sql");
        /// <summary>
        /// This is the internal reference/properties stored procedure
        /// </summary>
        public string SpUpsertRP => ProcessTemplate("StoredProcedures.spUpsertRP.sql");
        /// <summary>
        /// This is the internal reference/properties stored procedure
        /// </summary>
        public string SpHistory => ProcessTemplate("StoredProcedures.spHistory.sql");

        /// <summary>
        /// These are the CRUD stored procedures.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        protected void Append_SQLSPs_EntityCRUD(StringBuilder sb)
        {
            sb.AppendLine(SpUpsertRP);
            sb.AppendLine("GO");
            sb.AppendLine(SpHistory);
            sb.AppendLine("GO");
            sb.AppendLine(SpCreate);
            sb.AppendLine("GO");
            sb.AppendLine(SpDelete);
            sb.AppendLine("GO");
            sb.AppendLine(SpDeleteByRef);
            sb.AppendLine("GO");
            sb.AppendLine(SpRead);
            sb.AppendLine("GO");
            sb.AppendLine(SpReadByRef);
            sb.AppendLine("GO");
            sb.AppendLine(SpUpdate);
            sb.AppendLine("GO");
            sb.AppendLine(SpVersion);
            sb.AppendLine("GO");
            sb.AppendLine(SpVersionByRef);
            sb.AppendLine("GO");
        }
        #endregion
        #region Scripts Stored Procedures - Search
        /// <summary>
        /// This is the entity search stored procedure.
        /// </summary>
        public string SpSearchInternalBuild => ProcessTemplate("StoredProcedures.spSearchInternalBuild.sql");
        /// <summary>
        /// This is the entity search stored procedure.
        /// </summary>
        public string SpSearch => ProcessTemplate("StoredProcedures.spSearch.sql");
        /// <summary>
        /// This is the entity search entity stored procedure.
        /// </summary>
        public string SpSearchEntity => ProcessTemplate("StoredProcedures.spSearchEntity.sql");

        /// <summary>
        /// These are the SQL Search stored procedures.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        protected void Append_SQLSPs_Search(StringBuilder sb)
        {
            //Search
            sb.AppendLine(SpSearchInternalBuild);
            sb.AppendLine("GO");
            sb.AppendLine(SpSearch);
            sb.AppendLine("GO");
            sb.AppendLine(SpSearchEntity);
            sb.AppendLine("GO");
        }

        /// <summary>
        /// These are the SQL StoredProcedures for SQL Search.
        /// </summary>
        public string ScriptSearch => SBWrap(Append_SQLSPs_Search);
        #endregion
        #region Scripts Stored Procedures - Search JSON 
        /// <summary>
        /// This is the SP that collates the Id from the incoming JSON search request.
        /// </summary>
        public string SpSearchInternalBuildJson => ProcessTemplate("StoredProcedures.spSearchInternalBuildJson.sql");
        /// <summary>
        /// This is the entity search stored procedure.
        /// </summary>
        public string SpSearchJson => ProcessTemplate("StoredProcedures.spSearchJson.sql");
        /// <summary>
        /// This is the entity search entity stored procedure.
        /// </summary>
        public string SpSearchEntityJson => ProcessTemplate("StoredProcedures.spSearchEntityJson.sql");
        /// <summary>
        /// This is the function filter for the SQL.
        /// </summary>
        public string FnPropertyFilter => ProcessTemplate("Functions.fnFilter.sql");
        /// <summary>
        /// This is the function filter for the SQL.
        /// </summary>
        public string FnPropertyPaginate => ProcessTemplate("Functions.fnPaginate.sql");

        /// <summary>
        /// These are the SQL Search JSON extended stored procedures.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        protected void Append_SQLSPs_SearchJson(StringBuilder sb)
        {
            //Json Search
            sb.AppendLine(SpSearchInternalBuildJson);
            sb.AppendLine("GO");
            sb.AppendLine(SpSearchJson);
            sb.AppendLine("GO");
            sb.AppendLine(SpSearchEntityJson);
            sb.AppendLine("GO");
            sb.AppendLine(FnPropertyFilter);
            sb.AppendLine("GO");
            sb.AppendLine(FnPropertyPaginate);
            sb.AppendLine("GO");
        }

        /// <summary>
        /// These are the SQL StoredProcedures for SQL Json Search.
        /// </summary>
        public string ScriptSearchJson => SBWrap(Append_SQLSPs_SearchJson);
        #endregion


        /// <summary>
        /// This is the stored procedures script.
        /// </summary>
        public string ScriptStoredProcedures => ScriptStoredProceduresManual(false);
        /// <summary>
        /// This method returns all the stored procedures.
        /// </summary>
        /// <param name="createoralter">Specifies whether the stored procedure should be created or altered.</param>
        /// <returns>Returns the SQL script.</returns>
        public string ScriptStoredProceduresManual(bool createoralter = false) =>
            SBWrap(sb =>
            {
                Append_SQLSPs_EntityCRUD(sb);

                //Search
                Append_SQLSPs_Search(sb);

                //Json Search
                Append_SQLSPs_SearchJson(sb);

            }, createoralter);

        /// <summary>
        /// Returns the full DB definition for the entity (excluding ancillary definitions)
        /// </summary>
        /// <returns>Returns the SQL script.</returns>
        public string ScriptEntity =>
            SBWrap(sb =>
            {
                //Tables
                Append_SQL_Tables(sb);

                //Entities
                Append_SQLSPs_EntityCRUD(sb);

                //Search
                Append_SQLSPs_Search(sb);

                //Json Search
                Append_SQLSPs_SearchJson(sb);

            });
    }
}
