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
        /// This is the module realm.
        /// </summary>
        string Realm { get; }

        /// <summary>
        /// Gets the user using an async method.
        /// </summary>
        Task<(bool success, User user)> RetrieveUser(Guid id);
        /// <summary>
        /// Gets the user by reference using an async method.
        /// </summary>
        Task<(bool success, User user)> RetrieveUser(string type, string value);
        /// <summary>
        /// Gets the UserSecurity entity using an async function.
        /// </summary>
        Task<(bool success, UserSecurity uSec)> RetrieveUserSecurity(Guid id);

        /// <summary>
        /// Gets the UserSession entity using an async function.
        /// </summary>
        Task<(bool success, UserSession uSess)> RetrieveUserSession(Guid id);
        /// <summary>
        /// Gets the UserExternalAction entity using an async function.
        /// </summary>
        Task<(bool success, UserExternalAction uExAc)> RetrieveUserExternalAction(Guid id);

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
