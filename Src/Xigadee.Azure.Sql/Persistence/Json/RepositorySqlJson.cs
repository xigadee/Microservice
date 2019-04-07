using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        where K : IEquatable<K>
    {
        public RepositorySqlJson(string sqlConnection
            , Func<E, K> keyMaker
            , ISqlStoredProcedureResolver spNamer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null) 
            : base(sqlConnection, keyMaker, spNamer, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
        }

        public override Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        public override Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null)
        {
            throw new NotImplementedException();
        }

        protected override E DbDeserializeEntity(SqlDataReader dataReader)
        {
            throw new NotImplementedException();
        }

        protected override void DbSerializeEntity(E entity, SqlCommand cmd)
        {
            //cmd.Parameters.Add(new SqlParameter("@ExternalId", SqlDbType.UniqueIdentifier) { Value = entity.Id });
            //cmd.Parameters.Add(new SqlParameter("@VersionId", SqlDbType.UniqueIdentifier) { Value = entity.VersionId });
            //cmd.Parameters.Add(new SqlParameter("@VersionIdNew", SqlDbType.UniqueIdentifier) { Value = entity.VersionId });
            //cmd.Parameters.Add(new SqlParameter("@DateCreated", SqlDbType.UniqueIdentifier) { Value = entity.VersionId });
            //cmd.Parameters.Add(new SqlParameter("@DateUpdated", SqlDbType.UniqueIdentifier) { Value = entity.VersionId });

            //cmd.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entity) });
            //cmd.Parameters.Add(new SqlParameter("@UserIdReference", SqlDbType.UniqueIdentifier) { Value = entity.UserIdReference });

            //cmd.Parameters.Add(CreateReferencesParameter(entity));

        }

        //protected SqlParameter CreateReferencesParameter(E entity)
        //{
        //    var data = new DataTable();
        //    data.Columns.Add(new DataColumn("RefType", typeof(string)));
        //    data.Columns.Add(new DataColumn("RefValue", typeof(string)));

        //    foreach (var reference in _referenceMaker(entity).Where(r => !string.IsNullOrEmpty(r.Item2)))
        //    {
        //        data.Rows.Add(reference.Item1, reference.Item2);
        //    }

        //    return new SqlParameter("@References", SqlDbType.Structured) { TypeName = $"[{ExternalSchema}].ReferenceTableType", Value = data };
        //}

    }
}
