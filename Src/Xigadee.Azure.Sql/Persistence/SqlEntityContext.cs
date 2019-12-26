using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Xigadee
{
    #region SqlEntityContext
    /// <summary>
    /// This class holds the Sql context.
    /// </summary>
    public class SqlEntityContext : EntityContext, ISqlEntityContext
    {
        /// <summary>
        /// This is the context constructor.
        /// </summary>
        /// <param name="spName">The stored procedure name.</param>
        /// <param name="options">The repository settings.</param>
        public SqlEntityContext(string spName, RepositorySettings options):base(options)
        {
            SpName = spName;
        }

        /// <summary>
        /// This is the stored procedure name.
        /// </summary>
        public string SpName { get; }
        /// <summary>
        /// This is the sql command to be executed.
        /// </summary>
        public SqlCommand Command { get; set; }

    }
    #endregion
    #region SqlEntityContext<E> 
    /// <summary>
    /// This class holds the sql context.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class SqlEntityContext<E> : SqlEntityContext, IEntityContext<E>
    {
        /// <summary>
        /// This is the context constructor.
        /// </summary>
        /// <param name="spName">The stored procedure name.</param>
        /// <param name="options">The repository settings.</param>
        /// <param name="entity">The optional incoming entity.</param>
        public SqlEntityContext(string spName, RepositorySettings options, E entity = default(E)) : base(spName, options)
        {
            EntityIncoming = entity;
        }

        /// <summary>
        /// This is the entity list sent back from the database.
        /// </summary>
        public List<E> ResponseEntities { get; } = new List<E>();

        /// <summary>
        /// This is the incoming entity for the request.
        /// </summary>
        public E EntityIncoming { get; }

        /// <summary>
        /// This is the entity sent to the Sql server.
        /// </summary>
        public E EntityOutgoing { get; set; }
    }
    #endregion
    #region SqlEntityContext<K, E>
    /// <summary>
    /// This class holds the sql context.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class SqlEntityContext<K, E> : SqlEntityContext<E>, ISqlEntityContextKey<K>, IEntityContext<K,E> 
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the context constructor.
        /// </summary>
        /// <param name="spName">The stored procedure name.</param>
        /// <param name="options">The repository settings.</param>
        /// <param name="key">The optional key value.</param>
        /// <param name="entity">The optional entity value.</param>
        public SqlEntityContext(string spName, RepositorySettings options
            , K key = default(K)
            , E entity = default(E)) : base(spName, options, entity)
        {
            Key = key;
        }

        /// <summary>
        /// This is the incoming key.
        /// </summary>
        public K Key { get; }
    }
    #endregion
}
