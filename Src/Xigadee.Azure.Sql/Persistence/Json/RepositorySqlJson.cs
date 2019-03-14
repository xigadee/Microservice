using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        protected override void DbSerializeKey(K key, SqlCommand cmd)
        {
            throw new NotImplementedException();
        }
    }
}
