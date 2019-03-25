using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to retrieve a user and user security object from a back-end store.
    /// It is primarily used by the Authentication service.
    /// </summary>
    public interface IUserSecurityModule : IModuleBase
    {
        /// <summary>
        /// This is the module realm.
        /// </summary>
        string Realm { get; }

        /// <summary>
        /// Gets the user using an async method.
        /// </summary>
        Task<(bool success, UserBase user)> RetrieveUser(Guid id);
        /// <summary>
        /// Gets the user by reference using an async method.
        /// </summary>
        Task<(bool success, UserBase user)> RetrieveUser(string type, string value);
        /// <summary>
        /// Gets the UserSecurity entity using an async function.
        /// </summary>
        Task<(bool success, UserSecurityBase uSec)> RetrieveUserSecurity(Guid id);

        /// <summary>
        /// Gets the UserSession entity using an async function.
        /// </summary>
        Task<(bool success, UserSessionBase uSec)> RetrieveUserSession(Guid id);
        /// <summary>
        /// Gets the UserExternalAction entity using an async function.
        /// </summary>
        Task<(bool success, UserExternalActionBase uSec)> RetrieveUserExternalAction(Guid id);


        /// <summary>
        /// Gets the users repository
        /// </summary>
        IRepositoryAsync<Guid, UserBase> Users { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserSecurityBase> UserSecurity { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserSessionBase> UserSession { get; }
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        IRepositoryAsync<Guid, UserExternalActionBase> UserExternalAction { get; }
    }
}
