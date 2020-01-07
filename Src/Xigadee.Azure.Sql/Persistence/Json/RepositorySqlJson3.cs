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
    /// This is an experimental repository that uses a Json wrapper to pass the necessary parameters.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class RepositorySqlJson3<K, E> : RepositorySqlJson<K, E>
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
    }

    /// <summary>
    /// This wrapper holds an entity and it's associated properties.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class SqlJsonWrapper<E>
    {
        public SqlJsonWrapper()
        {

        }

        public SqlJsonWrapper(E entity)
        {
            Entity = entity;
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
}
