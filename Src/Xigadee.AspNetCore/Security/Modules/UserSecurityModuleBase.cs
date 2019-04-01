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
    public abstract class UserSecurityModuleBase<U> : UserSecurityModuleBase<U, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>
        where U : User
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
        #region Repositories
        /// <summary>
        /// Gets or sets the generic users repository
        /// </summary>
        public virtual RepositoryBase<Guid, U> RepositoryUsers { get; protected set; }
        /// <summary>
        /// Gets or sets the generic user security repository.
        /// </summary>
        public virtual RepositoryBase<Guid, USEC> RepositoryUserSecurities { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user sessions.
        /// </summary>
        public virtual RepositoryBase<Guid, USES> RepositoryUserSessions { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user external actions.
        /// </summary>
        public virtual RepositoryBase<Guid, UEXA> RepositoryUserExternalActions { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user roles.
        /// </summary>
        public virtual RepositoryBase<Guid, UR> RepositoryUserRoles { get; protected set; }
        /// <summary>
        /// Gets or sets the generic repository user access tokens.
        /// </summary>
        public virtual RepositoryBase<Guid, UAT> RepositoryUserAccessTokens { get; protected set; }
        #endregion

        #region IApiUserSecurityModule
        /// <summary>
        /// Gets the users repository
        /// </summary>
        public virtual IRepositoryAsync<Guid, User> Users => RepositoryUsers as IRepositoryAsync<Guid, User>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserSecurity> UserSecurities => RepositoryUserSecurities as IRepositoryAsync<Guid, UserSecurity>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserSession> UserSessions => RepositoryUserSessions as IRepositoryAsync<Guid, UserSession>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserRoles> UserRoles => RepositoryUserRoles as IRepositoryAsync<Guid, UserRoles>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserAccessToken> UserAccessTokens => RepositoryUserAccessTokens as IRepositoryAsync<Guid, UserAccessToken>;
        /// <summary>
        /// Gets the user security repository.
        /// </summary>
        public virtual IRepositoryAsync<Guid, UserExternalAction> UserExternalActions => RepositoryUserExternalActions as IRepositoryAsync<Guid, UserExternalAction>; 
        #endregion
    }
}
