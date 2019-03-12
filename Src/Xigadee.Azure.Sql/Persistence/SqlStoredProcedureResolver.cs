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
            , string prefix = "sp"
            , string interfix = null
            , string postfix = null
            , (RepositoryMethod method, string spName)[] overrides = null)
            : base(entityName ?? typeof(E).Name, schemaName, prefix, interfix, postfix, overrides)
        {

        }
    }

    /// <summary>
    /// This class is used to set the stored procedure names for an entity store.
    /// </summary>
    public class SqlStoredProcedureResolver
    {
        private readonly Dictionary<RepositoryMethod, string> _spNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlStoredProcedureResolver"/> class.
        /// </summary>
        public SqlStoredProcedureResolver(string entityName
            , string schemaName = null
            , string prefix = "sp"
            , string interfix = null
            , string postfix = null
            , (RepositoryMethod method, string spName)[] overrides = null)
        {
            EntityName = entityName ?? "";
            SchemaName = schemaName ?? "";
            PreFix = prefix ?? "";
            InterFix = interfix ?? "";
            PostFix = postfix ?? "";

            _spNames =
                Enum.GetValues(typeof(RepositoryMethod))
                    .Cast<RepositoryMethod>()
                    .ToDictionary((m) => m,
                        (m) => $"{ExternalSchema}{PreFix}{m.ToString()}{InterFix}{EntityName}{PostFix}");

            if (overrides != null)
                overrides.ForEach((o) => _spNames[o.method] = o.spName);
        }
        /// <summary>
        /// Gets the name of the entity.
        /// </summary>

        public string EntityName { get; }
        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        public string SchemaName { get; }
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
        public virtual string ExternalSchema => string.IsNullOrEmpty(SchemaName) ? "" : $"[{SchemaName}].";
        /// <summary>
        /// Gets the stored procedure create.
        /// </summary>
        public virtual string StoredProcedureCreate => _spNames[RepositoryMethod.Create];
        /// <summary>
        /// Gets the stored procedure read.
        /// </summary>
        public virtual string StoredProcedureRead => _spNames[RepositoryMethod.Read];
        /// <summary>
        /// Gets the stored procedure read by reference.
        /// </summary>
        public virtual string StoredProcedureReadByRef => _spNames[RepositoryMethod.ReadByRef];
        /// <summary>
        /// Gets the stored procedure update.
        /// </summary>
        public virtual string StoredProcedureUpdate => _spNames[RepositoryMethod.Update];
        /// <summary>
        /// Gets the stored procedure delete.
        /// </summary>
        public virtual string StoredProcedureDelete => _spNames[RepositoryMethod.Delete];
        /// <summary>
        /// Gets the stored procedure delete by reference.
        /// </summary>
        public virtual string StoredProcedureDeleteByRef => _spNames[RepositoryMethod.DeleteByRef];
        /// <summary>
        /// Gets the stored procedure version.
        /// </summary>
        public virtual string StoredProcedureVersion => _spNames[RepositoryMethod.Version];
        /// <summary>
        /// Gets the stored procedure version by reference.
        /// </summary>
        public virtual string StoredProcedureVersionByRef => _spNames[RepositoryMethod.VersionByRef];
        /// <summary>
        /// Gets the stored procedure search.
        /// </summary>
        public virtual string StoredProcedureSearch => _spNames[RepositoryMethod.Search];
        /// <summary>
        /// Gets the stored procedure search entity.
        /// </summary>
        public virtual string StoredProcedureSearchEntity => _spNames[RepositoryMethod.SearchEntity];
    }
}
