using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This is an experimental repository that uses a Json wrapper to pass the necessary parameters.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class RepositorySqlJson3<K, E> : RepositorySqlJson2<K, E>
        where E : EntityAuditableBase
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySqlJson{K, E}"/> class.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection string.</param>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="spNamer">The stored procedure namer class.</param>
        /// <param name="referenceMaker">The optional entity reference maker.</param>
        /// <param name="propertiesMaker">The optional entity properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyManager">The key manager.</param>
        /// <param name="signaturePolicy">This is the manual signature policy for the entity. 
        /// If this is null, the repository attempts to set the policy using the EntitySignaturePolicyAttribute</param>
        public RepositorySqlJson3(string sqlConnection
            , Func<E, K> keyMaker = null
            , ISqlStoredProcedureResolver spNamer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            , ISignaturePolicy signaturePolicy = null
            )
            : base(sqlConnection, keyMaker, spNamer, referenceMaker, propertiesMaker, versionPolicy, keyManager, signaturePolicy)
        {
        }
        #endregion

        #region DbSerializeEntity(E entity, SqlCommand cmd)
        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="ctx">The context</param>
        protected override void DbSerializeEntity(SqlEntityContext<E> ctx)
        {
            try
            {
                var cmd = ctx.Command;
                var entity = ctx.EntityOutgoing;

                cmd.Parameters.Add(new SqlParameter("@ExternalId", SqlDbType.UniqueIdentifier) { Value = entity.Id });

                cmd.Parameters.Add(new SqlParameter("@VersionId", SqlDbType.UniqueIdentifier) { Value = ctx.EntityIncoming.VersionId });

                cmd.Parameters.Add(new SqlParameter("@VersionIdNew", SqlDbType.UniqueIdentifier) { Value = entity.VersionId });

                cmd.Parameters.Add(new SqlParameter("@UserIdAudit", SqlDbType.UniqueIdentifier) { Value = entity.UserIdAudit });
                cmd.Parameters.Add(new SqlParameter("@DateCreated", SqlDbType.DateTime) { Value = entity.DateCreated });
                cmd.Parameters.Add(new SqlParameter("@DateUpdated", SqlDbType.DateTime) { Value = entity.DateUpdated });

                cmd.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar) { Value = CreateBody(entity) });

                cmd.Parameters.Add(new SqlParameter("@Sig", SqlDbType.VarChar, 255) { Value = SignatureCreate(entity) });
            }
            catch (Exception e)
            {
                Collector?.LogWarning($"Unable to serialize entity {typeof(E).Name}/{ctx.EntityOutgoing.Id} - {e.Message}");

                throw;
            }

        }
        #endregion
        #region CreateBody(E entity)
        /// <summary>
        /// This method converts the entity in to JSON body. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the Json serialized entity wrapper.</returns>
        protected override string CreateBody(E entity)
        {
            var wrapper = new SqlJsonWrapper<E>(entity);

            return JsonConvert.SerializeObject(wrapper);
        }
        #endregion
    }

    #region SqlJsonWrapper<E>
    /// <summary>
    /// This wrapper holds an entity and it's associated properties.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class SqlJsonWrapper<E>
    {
        /// <summary>
        /// This is the empty constructor.
        /// </summary>
        public SqlJsonWrapper()
        {

        }
        /// <summary>
        /// This is the entity based constructor.
        /// </summary>
        /// <param name="entity">The entity to load.</param>
        public SqlJsonWrapper(E entity)
        {
            Load(entity);
        }

        /// <summary>
        /// This method loads the entity and extracts its properties and references.
        /// </summary>
        /// <param name="entity">The entity to load.</param>
        public void Load(E entity)
        {
            var res = EntityHintHelper.Resolve(entity.GetType());
            Entity = entity;

            if (res?.SupportsReferences ?? false)
                References = res.References(entity).Select(i => new KeyValuePair<string, string>(i.Item1, i.Item2))
                    .ToList();

            if (res?.SupportsProperties ?? false)
                Properties = res.Properties(entity).Select(i => new KeyValuePair<string, string>(i.Item1, i.Item2))
                    .ToList();
        }

        /// <summary>
        /// This is the entity.
        /// </summary>
        public E Entity { get; set; }
        /// <summary>
        /// This collection holds the entity properties.
        /// </summary>
        public List<KeyValuePair<string, string>> Properties { get; set; } = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// This collection holds the entity references.
        /// </summary>
        public List<KeyValuePair<string, string>> References { get; set; } = new List<KeyValuePair<string, string>>();
    } 
    #endregion
}
