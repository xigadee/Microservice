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
    /// <seealso cref="Xigadee.RepositorySqlJson{K, E}" />
    public class RepositorySqlJson2<K, E> : RepositorySqlJson<K, E>
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
        public RepositorySqlJson2(string sqlConnection
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

        #region DbSerializeSearchRequestCombined(ISqlEntityContextKey<SearchRequest> ctx)
        /// <summary>
        /// This method serializes the search request to a set of SQL parameters. 
        /// </summary>
        /// <param name="ctx">The Sql context</param>
        protected override void DbSerializeSearchRequestCombined(ISqlEntityContextKey<SearchRequest> ctx)
        {
            var cmd = ctx.Command;
            var entity = ctx.Key;

            cmd.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar) { Value = CreateSearchBody(entity) });
        }
        #endregion
        #region CreateSearchBody(SearchRequest entity)
        /// <summary>
        /// This method converts the search request in to an JSON request.
        /// </summary>
        /// <param name="entity">The search request</param>
        /// <returns>Returns the JSON formatted request data.</returns>
        protected string CreateSearchBody(SearchRequest entity)
        {
            return JsonConvert.SerializeObject(entity);
        }
        #endregion

        /// <summary>
        /// This returns the name of the search stored procedure.
        /// </summary>
        /// <param name="id">The search type. This will be set to Default if left as null.</param>
        /// <returns>Returns the stored procedure name.</returns>
        protected override string SearchSpName(string id) => SpNamer.StoredProcedureSearchJson(id ?? "Default");
        /// <summary>
        /// This returns the name of the search entity stored procedure.
        /// </summary>
        /// <param name="id">The search type. This will be set to Default if left as null.</param>
        /// <returns>Returns the stored procedure name.</returns>
        protected override string SearchEntitySpName(string id) => SpNamer.StoredProcedureSearchEntityJson(id ?? "Default");
    }
}
