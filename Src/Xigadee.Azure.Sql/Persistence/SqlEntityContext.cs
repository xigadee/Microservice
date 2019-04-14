using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Xigadee
{
    public interface ISqlEntityContextKey<K>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the key.
        /// </summary>
        K Key { get; }

        /// <summary>
        /// This is the sql command to be executed.
        /// </summary>
        SqlCommand Command { get; }
    }

    public class SqlEntityContext
    {
        /// <summary>
        /// This is the context 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="options"></param>
        public SqlEntityContext(string spName, RepositorySettings options)
        {
            SpName = spName;
            Options = options;
        }
        /// <summary>
        /// The repository options.
        /// </summary>
        public RepositorySettings Options { get; }
        /// <summary>
        /// This is the stored procedure name.
        /// </summary>
        public string SpName { get; }
        /// <summary>
        /// This is the sql command to be executed.
        /// </summary>
        public SqlCommand Command { get; set; }
        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// This is the reference
        /// </summary>
        public (string type, string value) Reference { get; set; }
    }

    public class SqlEntityContext<E> : SqlEntityContext
    {
        /// <summary>
        /// This is the context 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="options"></param>
        public SqlEntityContext(string spName, RepositorySettings options, E entity = default(E)) : base(spName, options)
        {
            EntityIncoming = entity;
        }

        /// <summary>
        /// Gets the entity list.
        /// </summary>
        public List<E> ResponseEntities { get; } = new List<E>();

        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public int ResponseCode { get; set; }
        /// <summary>
        /// Gets or sets the optional response message.
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// This is the entity.
        /// </summary>
        public E EntityIncoming { get; set; }
    }

    /// <summary>
    /// This class holds the context
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class SqlEntityContext<K, E> : SqlEntityContext<E>, ISqlEntityContextKey<K>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the context 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="options"></param>
        public SqlEntityContext(string spName, RepositorySettings options
            , K key = default(K)
            , E entity = default(E)) : base(spName, options, entity)
        {
            Key = key;
        }

        /// <summary>
        /// This is the incoming key.
        /// </summary>
        public K Key { get; set; }
    }
}
