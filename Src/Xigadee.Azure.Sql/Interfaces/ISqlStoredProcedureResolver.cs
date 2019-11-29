using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to specify the default names of the repository stored procedures.
    /// </summary>
    public interface ISqlStoredProcedureResolver
    {
        /// <summary>
        /// This iterator returns the specific sql name based on the specific method.
        /// </summary>
        /// <param name="o">The method type.</param>
        /// <returns>Returns a SQL name.</returns>
        string this[RepositoryMethod o] { get; }

        /// <summary>
        /// Gets the common name of the entity.
        /// </summary>
        string EntityName { get; }

        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        string ExternalSchemaName { get; }

        /// <summary>
        /// Gets the name of the table schema, usually dbo.
        /// </summary>
        string TableSchemaName { get; }
        /// <summary>
        /// Gets the name of the helper schema, usually Helper.
        /// </summary>
        string HelperSchemaName { get; }
        /// <summary>
        /// Gets the name of the migration schema, usually Migration.
        /// </summary>
        string MigrationSchemaName { get; }
        /// <summary>
        /// Gets the external schema.
        /// </summary>
        string ExternalSchema { get; }

        /// <summary>
        /// This list of defined schemas.
        /// </summary>
        IEnumerable<string> Schemas { get; }
        /// <summary>
        /// Gets the create stored procedure name.
        /// </summary>
        string StoredProcedureCreate { get; }

        /// <summary>
        /// Gets the read stored procedure name.
        /// </summary>
        string StoredProcedureRead { get; }
        /// <summary>
        /// Gets the read by reference stored procedure name.
        /// </summary>
        string StoredProcedureReadByRef { get; }

        /// <summary>
        /// Gets the update stored procedure name.
        /// </summary>
        string StoredProcedureUpdate { get; }

        /// <summary>
        /// Gets the delete stored procedure name.
        /// </summary>
        string StoredProcedureDelete { get; }
        /// <summary>
        /// Gets the delete by reference stored procedure name.
        /// </summary>
        string StoredProcedureDeleteByRef { get; }

        /// <summary>
        /// Gets the version stored procedure name.
        /// </summary>
        string StoredProcedureVersion { get; }
        /// <summary>
        /// Gets the version by reference stored procedure name.
        /// </summary>
        string StoredProcedureVersionByRef { get; }

        /// <summary>
        /// Gets the search stored procedure name.
        /// </summary>
        string StoredProcedureSearch(string id);
        /// <summary>
        /// Gets the search entity stored procedure name.
        /// </summary>
        string StoredProcedureSearchEntity(string id);

        /// <summary>
        /// Gets the history stored procedure name.
        /// </summary>
        string StoredProcedureHistory { get; }

        /// <summary>
        /// Gets the search stored procedure name.
        /// </summary>
        string StoredProcedureSearchJson(string id);
        /// <summary>
        /// Gets the search entity stored procedure name.
        /// </summary>
        string StoredProcedureSearchEntityJson(string id);

        /// <summary>
        /// The property/reference upsert stored procedure.
        /// </summary>
        string StoredProcedureNameUpsertRP { get; }
        /// <summary>
        /// The history log stored procedure.
        /// </summary>
        string StoredProcedureNameHistory { get; }
    }
}