using System;

namespace Xigadee
{
    /// <summary>
    /// This is the base interface for repository based operation.
    /// </summary>
    public interface IRepositoryBase
    {
        /// <summary>
        /// This is the generic repository type, i.e. IRepositoryAsyncServer
        /// </summary>
        Type RepositoryServerType { get; }
        /// <summary>
        /// This is the generic repository type, i.e. IRepository K,E
        /// </summary>
        Type RepositoryType { get; }
        /// <summary>
        /// This is the entity type.
        /// </summary>
        Type TypeEntity { get; }
        /// <summary>
        /// This is the key type,
        /// </summary>
        Type TypeKey { get; }
    }
}