using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This repository is for entities that store the body of the entity as native JSON in a SQL server.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <seealso cref="Xigadee.RepositorySqlBase{K, E}" />
    public class RepositorySqlJson<K, E> : RepositorySqlBase<K, E>
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
        public RepositorySqlJson(string sqlConnection
            , Func<E, K> keyMaker = null
            , ISqlStoredProcedureResolver spNamer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null)
            : base(sqlConnection, keyMaker, spNamer, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
        } 
        #endregion

        public override Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        public override Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        #region DbDeserializeEntity(SqlDataReader dataReader)
        /// <summary>
        /// This method deserializes a data reader record into an entity.
        /// </summary>
        /// <param name="dataReader">Data reader</param>
        /// <param name="ctx">The context.</param>
        protected override void DbDeserializeEntity(SqlDataReader dataReader, SqlEntityContext<E> ctx)
        {
            try
            {
                var json = dataReader["Body"]?.ToString();
                var entity = JsonConvert.DeserializeObject<E>(json);
                var sig = dataReader.GetFieldValue<string>(dataReader.GetOrdinal("Sig"));
                SignatureValidate(entity, sig);
                ctx.ResponseEntities.Add(entity);
            }
            catch (JsonException e)
            {
                throw;// new RepositoryException($"Unable to deserialize correspondent from {dllVersion} to {_dllVersion}", e);
            }
        } 
        #endregion

        #region DbSerializeEntity(E entity, SqlCommand cmd)
        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="ctx">The context</param>
        protected override void DbSerializeEntity(SqlEntityContext<E> ctx)
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

            cmd.Parameters.Add(new SqlParameter("@References", SqlDbType.Structured)
            { TypeName = $"{SpNamer.ExternalSchema}[KvpTableType]", Value = CreateReferences(entity) });

            cmd.Parameters.Add(new SqlParameter("@Properties", SqlDbType.Structured)
            { TypeName = $"{SpNamer.ExternalSchema}[KvpTableType]", Value = CreateProperties(entity) });

            cmd.Parameters.Add(new SqlParameter("@Sig", SqlDbType.VarChar, 255) { Value = SignatureCreate(entity) });
        }
        #endregion

        #region SignatureCreate(E entity)
        /// <summary>
        /// TODO: Creates the signature. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the string signature.</returns>
        protected virtual string SignatureCreate(E entity)
        {
            return "";
        }
        #endregion
        #region SignatureValidate(E entity, string signature)
        /// <summary>
        /// TODO: Creates the signature. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="signature">The signature.</param>
        /// <returns>Returns the string signature.</returns>
        protected virtual void SignatureValidate(E entity, string signature)
        {
        }
        #endregion

        #region CreateBody(E entity)
        /// <summary>
        /// This method converts the entity in to JSON body. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected virtual string CreateBody(E entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
        #endregion

        #region CreateProperties/CreateReferences
        /// <summary>
        /// Creates the properties data table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the data table.</returns>
        protected virtual DataTable CreateProperties(E entity) => CreateDataTable(entity, PropertiesMaker);

        /// <summary>
        /// Creates the references data table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the data table.</returns>
        protected virtual DataTable CreateReferences(E entity) => CreateDataTable(entity, ReferencesMaker); 
        #endregion
        #region CreateDataTable(E entity, Func<E, IEnumerable<Tuple<string, string>>> extractor)
        /// <summary>
        /// Creates the data table that contains the references or the properties..
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="extractor">The extractor function.</param>
        /// <returns>Returns the SqlTable definition.</returns>
        protected virtual DataTable CreateDataTable(E entity, Func<E, IEnumerable<Tuple<string, string>>> extractor)
        {
            var data = new DataTable();
            data.Columns.Add(new DataColumn("RefType", typeof(string)));
            data.Columns.Add(new DataColumn("RefValue", typeof(string)));

            foreach (var reference in extractor(entity).Where(r => !string.IsNullOrEmpty(r.Item2)))
            {
                data.Rows.Add(reference.Item1, reference.Item2);
            }

            return data;
        }
        #endregion

    }
}
