using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    #region EntityContext
    /// <summary>
    /// This class holds the Sql context.
    /// </summary>
    public class EntityContext : IEntityContext
    {
        /// <summary>
        /// This is the context constructor.
        /// </summary>
        /// <param name="options">The repository settings.</param>
        public EntityContext(RepositorySettings options)
        {
            Options = options;
        }
        /// <summary>
        /// The repository options.
        /// </summary>
        public RepositorySettings Options { get; }
        /// <summary>
        /// Gets or sets the version identifier.
        /// </summary>
        public string VersionId { get; set; }
        /// <summary>
        /// This is the reference
        /// </summary>
        public (string type, string value) Reference { get; set; }

        /// <summary>
        /// Specifies that the response code is a success.
        /// </summary>
        public bool IsSuccessResponse => ResponseCode >= 200 && ResponseCode <= 299;

        /// <summary>
        /// The response is a not found response.
        /// </summary>
        public bool IsNotFoundResponse => ResponseCode == 404;

        /// <summary>
        /// Gets or sets the response code.
        /// </summary>
        public int ResponseCode { get; set; }
        /// <summary>
        /// Gets or sets the optional response message.
        /// </summary>
        public string ResponseMessage { get; set; }
    }
    #endregion
    #region EntityContext<E> 
    /// <summary>
    /// This class holds the sql context.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityContext<E> : EntityContext, IEntityContext<E>
    {
        /// <summary>
        /// This is the context constructor.
        /// </summary>
        /// <param name="options">The repository settings.</param>
        /// <param name="entity">The optional incoming entity.</param>
        public EntityContext(RepositorySettings options, E entity = default(E)) : base(options)
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
    #region EntityContext<K, E>
    /// <summary>
    /// This class holds the sql context.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityContext<K, E> : EntityContext<E>, IEntityContext<K, E> 
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the context constructor.
        /// </summary>
        /// <param name="options">The repository settings.</param>
        /// <param name="key">The optional key value.</param>
        /// <param name="entity">The optional entity value.</param>
        public EntityContext(RepositorySettings options
            , K key = default(K)
            , E entity = default(E)) : base(options, entity)
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
