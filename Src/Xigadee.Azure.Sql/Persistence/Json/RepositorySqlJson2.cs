﻿
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
    public class RepositorySqlJson2<K, E> : RepositorySqlBase<K, E>
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
        public RepositorySqlJson2(string sqlConnection
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
        protected virtual DataTable CreateDataTable<ET>(ET entity, Func<ET, IEnumerable<Tuple<string, string>>> extractor)
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

        #region DbSerializeSearchRequestCombined(ISqlEntityContextKey<SearchRequest> ctx)
        /// <summary>
        /// This method serializes the search request to a set of SQL parameters. 
        /// </summary>
        /// <param name="ctx">The Sql context</param>
        protected override void DbSerializeSearchRequestCombined(ISqlEntityContextKey<SearchRequest> ctx)
        {
            var cmd = ctx.Command;
            var entity = ctx.Key;

            cmd.Parameters.Add(new SqlParameter("@ETag", SqlDbType.VarChar, 50) { Value = entity.ETag });
            //cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = entity. });

            cmd.Parameters.Add(new SqlParameter("@PropertiesFilter", SqlDbType.Structured)
            { TypeName = $"{SpNamer.ExternalSchema}[KvpTableType]", Value = CreateDataTable(entity, CreateFilterParamsDataTable) });

            cmd.Parameters.Add(new SqlParameter("@PropertyOrder", SqlDbType.Structured)
            { TypeName = $"{SpNamer.ExternalSchema}[KvpTableType]", Value = CreateDataTable(entity, CreateOrderParamsDataTable) });

            cmd.Parameters.Add(new SqlParameter("@Skip", SqlDbType.Int) { Value = entity.SkipValue });
            cmd.Parameters.Add(new SqlParameter("@Top", SqlDbType.Int) { Value = entity.TopValue });
        } 
        #endregion

        private IEnumerable<Tuple<string, string>> CreateFilterParamsDataTable(SearchRequest sr)
        {
            return sr.FilterParameters.Select(r => new Tuple<string,string>(r.Key, r.Value)) ;
        }

        private IEnumerable<Tuple<string, string>> CreateOrderParamsDataTable(SearchRequest sr)
        {
            return sr.FilterParameters.Select(r => new Tuple<string, string>(r.Key, r.Value));
        }

        private void AddResponseFields(SearchRequest rq, SearchResponse rs)
        {
            rs.Fields.Add(0, new FieldMetadata { Name = "_" });

            rq.Select().ForIndex((i, s) => rs.Fields[i + 1] = new FieldMetadata { Name = s });

            if (rs.Fields.Count > 1)
                return;

            //OK, check the entity for property hints.
            var res = EntityHintHelper.Resolve<E>();

            if (!res.SupportsProperties)
                return;

            res.PropertyNames.ForIndex((i, s) => rs.Fields[i + 1] = new FieldMetadata { Name = s });
        }

        /// <summary>
        /// THis method deserializes the field search response.
        /// </summary>
        /// <param name="dataReader">The dataReader</param>
        /// <param name="ctx">The Sql context.</param>
        protected override void DbDeserializeSearchResponse(SqlDataReader dataReader, SqlEntityContext<SearchRequest, SearchResponse> ctx)
        {
            var rs = ctx.EntityOutgoing;

            if (rs.Fields.Count == 0)
                AddResponseFields(ctx.Key, rs);

            var id = dataReader["ExternalId"]?.ToString();

            var json = dataReader["Body"]?.ToString();
            var entity = JsonConvert.DeserializeObject<SqlProperties>(json);

            var values = new string[rs.Fields.Count];
            foreach (var field in rs.Fields)
            {
                if (field.Value.Name == "_")
                {
                    values[field.Key] = id;
                    continue;
                }
                var value = entity.Property.FirstOrDefault(k => k.Type.Equals(field.Value.Name, StringComparison.InvariantCultureIgnoreCase));
                if (value != null)
                    values[field.Key] = value.Value;
            }

            rs.Data.Add(values);
        }

        private class SqlProperties
        {
            public List<SqlProperty> Property { get; set; }
        }

        private class SqlProperty
        {
            public string Type;
            public string Value;
        }

        /// <summary>
        /// This method resolves the reader in to the search response.
        /// </summary>
        /// <param name="dataReader">The reader containing the response.</param>
        /// <param name="ctx">The current context.</param>
        protected override void DbDeserializeSearchResponseEntity(SqlDataReader dataReader, SqlEntityContext<SearchRequest, SearchResponse<E>> ctx)
        {
            var rs = ctx.EntityOutgoing;
            var json = dataReader["Body"]?.ToString();
            var entity = JsonConvert.DeserializeObject<E>(json);
            rs.Data.Add(entity);
            
        }

    }
}