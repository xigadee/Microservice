using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public abstract class UserSecurityModuleBase : UserSecurityModuleBase<User, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>
    {


    }

    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public abstract class UserSecurityModuleBase<U,USEC,USES,UEXA,UR,UAT> : ApiModuleBase, IApiUserSecurityModule
        where U : User
        where USEC : UserSecurity
        where USES : UserSession
        where UEXA : UserExternalAction
        where UR : UserRoles
        where UAT : UserAccessToken
    {
        protected virtual RepositoryBase<Guid, U> RepoUsers { get; set; }

        protected virtual RepositoryBase<Guid, USEC> RepoUserSecurities { get; set; }

        protected virtual RepositoryBase<Guid, USES> RepoUserSessions { get; set; }

        protected virtual RepositoryBase<Guid, UEXA> RepoUserExternalActions { get; set; }

        protected virtual RepositoryBase<Guid, UR> RepoUserRoles { get; set; }

        protected virtual RepositoryBase<Guid, UAT> RepoUserAccessTokens { get; set; }

        /// <summary>
        /// Gets the users repository
        /// </summary>
        public virtual IRepositoryAsync<Guid, User> Users => RepoUsers as IRepositoryAsync<Guid, User>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserSecurity> UserSecurities => RepoUserSecurities as IRepositoryAsync<Guid, UserSecurity>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserSession> UserSessions => RepoUserSessions as IRepositoryAsync<Guid, UserSession>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserRoles> UserRoles => RepoUserRoles as IRepositoryAsync<Guid, UserRoles>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserAccessToken> UserAccessTokens => RepoUserAccessTokens as IRepositoryAsync<Guid, UserAccessToken>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserExternalAction> UserExternalActions => RepoUserExternalActions as IRepositoryAsync<Guid, UserExternalAction>;
    }

}
