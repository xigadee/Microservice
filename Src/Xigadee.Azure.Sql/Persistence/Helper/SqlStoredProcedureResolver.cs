using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class is used to set the stored procedure names for an entity store.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <seealso cref="Xigadee.SqlStoredProcedureResolver" />
    public class SqlStoredProcedureResolver<E>: SqlStoredProcedureResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedureResolver{E}"/> class.
        /// </summary>
        public SqlStoredProcedureResolver(string entityName = null
            , string schemaName = null
            , string internalSchemaName = null
            , string prefix = "sp"
            , string interfix = null
            , string postfix = null
            , (RepositoryMethod method, string spName)[] overrides = null)
            : base(entityName ?? typeof(E).Name, schemaName, internalSchemaName, prefix, interfix, postfix, overrides)
        {

        }
    }

    /// <summary>
    /// This class is used to set the stored procedure names for an entity store.
    /// </summary>
    public class SqlStoredProcedureResolver : ISqlStoredProcedureResolver
    {
        private readonly Dictionary<RepositoryMethod, string> _spNames;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedureResolver"/> class.
        /// </summary>
        public SqlStoredProcedureResolver(string entityName
            , string schemaName = null
            , string internalSchemaName = null
            , string prefix = "sp"
            , string interfix = null
            , string postfix = null
            , (RepositoryMethod method, string spName)[] overrides = null)
        {
            EntityName = entityName ?? "";
            ExternalSchemaName = schemaName ?? "External";
            TableSchemaName = internalSchemaName ?? "dbo";
            PreFix = prefix ?? "";
            InterFix = interfix ?? "";
            PostFix = postfix ?? "";

            _spNames =
                Enum.GetValues(typeof(RepositoryMethod))
                    .Cast<RepositoryMethod>()
                    .ToDictionary((m) => m,
                        (m) => Name1(m.ToString()));

            if (overrides != null)
                overrides.ForEach((o) => _spNames[o.method] = o.spName);
        } 
        #endregion

        string Name1(string m) => $"{PreFix}{EntityName}{InterFix}{m.ToString()}{PostFix}";

        string Name3(string m) => $"{PreFix}{m.ToString()}{InterFix}{EntityName}{PostFix}";

        /// <summary>
        /// This iterator returns the specific sql name based on the specific method.
        /// </summary>
        /// <param name="o">The method type.</param>
        /// <returns>Returns a SQL name.</returns>
        public string this[RepositoryMethod o] => _spNames[o];
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public string EntityName { get; }
        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        public string ExternalSchemaName { get; }
        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        public string TableSchemaName { get; }
        /// <summary>
        /// Gets the pre fix.
        /// </summary>
        public string PreFix { get; }
        /// <summary>
        /// Gets the post fix.
        /// </summary>
        public string PostFix { get; }
        /// <summary>
        /// Gets the inter fix character.
        /// </summary>
        public string InterFix { get; }
        /// <summary>
        /// Gets the external schema.
        /// </summary>
        public virtual string ExternalSchema => string.IsNullOrEmpty(ExternalSchemaName) ? "" : $"[{ExternalSchemaName}].";
        /// <summary>
        /// Gets the stored procedure create.
        /// </summary>
        public virtual string StoredProcedureCreate => ExternalSchema +_spNames[RepositoryMethod.Create];
        /// <summary>
        /// Gets the stored procedure read.
        /// </summary>
        public virtual string StoredProcedureRead => ExternalSchema + _spNames[RepositoryMethod.Read];
        /// <summary>
        /// Gets the stored procedure read by reference.
        /// </summary>
        public virtual string StoredProcedureReadByRef => ExternalSchema + _spNames[RepositoryMethod.ReadByRef];
        /// <summary>
        /// Gets the stored procedure update.
        /// </summary>
        public virtual string StoredProcedureUpdate => ExternalSchema + _spNames[RepositoryMethod.Update];
        /// <summary>
        /// Gets the stored procedure delete.
        /// </summary>
        public virtual string StoredProcedureDelete => ExternalSchema + _spNames[RepositoryMethod.Delete];
        /// <summary>
        /// Gets the stored procedure delete by reference.
        /// </summary>
        public virtual string StoredProcedureDeleteByRef => ExternalSchema + _spNames[RepositoryMethod.DeleteByRef];
        /// <summary>
        /// Gets the stored procedure version.
        /// </summary>
        public virtual string StoredProcedureVersion => ExternalSchema + _spNames[RepositoryMethod.Version];
        /// <summary>
        /// Gets the stored procedure version by reference.
        /// </summary>
        public virtual string StoredProcedureVersionByRef => ExternalSchema + _spNames[RepositoryMethod.VersionByRef];
        /// <summary>
        /// Gets the stored procedure search.
        /// </summary>
        /// <param name="id">The search identifier.</param>
        /// <returns>Returns the combined string which matches to a specific search stored procedure.</returns>
        public virtual string StoredProcedureSearch(string id) => ExternalSchema + $"{_spNames[RepositoryMethod.Search]}_{id?.Trim().ToLowerInvariant()??"Default"}";
        /// <summary>
        /// Gets the stored procedure search entity.
        /// </summary>
        /// <param name="id">The search identifier.</param>
        /// <returns>Returns the combined string which matches to a specific search stored procedure.</returns>
        public virtual string StoredProcedureSearchEntity(string id) => ExternalSchema + $"{_spNames[RepositoryMethod.SearchEntity]}_{id?.Trim().ToLowerInvariant()??"Default"}";

        /// <summary>
        /// This is the new Json based search request.
        /// </summary>
        /// <param name="id">The search identifier.</param>
        /// <returns>Returns the combined string which matches to a specific search stored procedure.</returns>
        public virtual string StoredProcedureSearchJson(string id) => ExternalSchema + $"{_spNames[RepositoryMethod.Search]}_{id?.Trim().ToLowerInvariant() ?? "Default"}_Json";
        /// <summary>
        /// This is the new entity based Json search request.
        /// </summary>
        /// <param name="id">The search identifier.</param>
        /// <returns>Returns the combined string which matches to a specific search stored procedure.</returns>
        public virtual string StoredProcedureSearchEntityJson(string id) => ExternalSchema + $"{_spNames[RepositoryMethod.SearchEntity]}_{id?.Trim().ToLowerInvariant() ?? "Default"}_Json";


        /// <summary>
        /// This is the shared internal upsert stored procedure.
        /// </summary>
        public virtual string StoredProcedureUpsertRP => ExternalSchema + StoredProcedureNameUpsertRP;
        /// <summary>
        /// This is the upsert sp
        /// </summary>
        public virtual string StoredProcedureNameUpsertRP => Name1("UpsertRP");
        /// <summary>
        /// This is for suture support.
        /// </summary>
        public virtual string StoredProcedureNameHistory => Name1("History");

    }
}
