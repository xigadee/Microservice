using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to retrieve a user and user security object from a back-end store.
    /// It is primarily used by the Authentication service.
    /// </summary>
    public interface IApiUserSecurityModule : IApiModuleBase
    {
        /// <summary>
        /// Gets the users repository
        /// </summary>
        IRepositoryAsync<Guid, User> Users { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserSecurity> UserSecurities { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserSession> UserSessions { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserRoles> UserRoles { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserAccessToken> UserAccessTokens { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserExternalAction> UserExternalActions { get; }
    }
}
