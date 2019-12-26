
using System;
using System.Collections.Generic;

namespace Xigadee
{
    public interface IEntityContext
    {
        bool IsNotFoundResponse { get; }
        bool IsSuccessResponse { get; }
        RepositorySettings Options { get; }
        (string type, string value) Reference { get; set; }
        int ResponseCode { get; set; }
        string ResponseMessage { get; set; }
        string VersionId { get; set; }
    }

    public interface IEntityContext<E> : IEntityContext
    {
        /// <summary>
        /// This is the entity list sent back from the database.
        /// </summary>
        List<E> ResponseEntities { get; }

        /// <summary>
        /// This is the incoming entity for the request.
        /// </summary>
        E EntityIncoming { get; }

        /// <summary>
        /// This is the entity sent to the Sql server.
        /// </summary>
        E EntityOutgoing { get; set; }
    }

    public interface IEntityContext<K, E> : IEntityContext<E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the incoming key.
        /// </summary>
        K Key { get; }
    }
}