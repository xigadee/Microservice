using System;
using System.Security.Principal;
namespace Xigadee
{
    /// <summary>
    /// This is a default repository async interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepositoryAsyncClient<K, E> : IRepositoryAsync<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Gets or sets the default security principal.
        /// </summary>
        IPrincipal DefaultPrincipal { get; set; }
    }
}
