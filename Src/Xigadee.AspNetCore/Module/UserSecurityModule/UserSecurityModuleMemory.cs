using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public class UserSecurityModuleMemoryTest : UserSecurityModuleMemoryTest<User, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>
    {


    }

    /// <summary>
    /// This module implements the core application security logic.
    /// </summary>
    /// <seealso cref="Xigadee.ApiModuleBase" />
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public class UserSecurityModuleMemoryTest<U> : UserSecurityModuleMemoryTest<U, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>
        where U : User
    {


    }

    /// <summary>
    /// This module is used for testing purposes.
    /// </summary>
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public class UserSecurityModuleMemoryTest<U, USEC, USES, UEXA, UR, UAT> : UserSecurityModuleBase<U, USEC, USES, UEXA, UR, UAT>
        where U : User
        where USEC : UserSecurity
        where USES : UserSession
        where UEXA : UserExternalAction
        where UR : UserRoles
        where UAT : UserAccessToken
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurityModuleMemoryTest"/> class.
        /// This class holds the security objects in memory only and can be used for unit testing.
        /// </summary>
        public UserSecurityModuleMemoryTest()
        {
            CreateRepositories();
        }

        /// <summary>
        /// Creates the repositories.
        /// </summary>
        protected virtual void CreateRepositories()
        {
            CreateUserRepo();

            CreateUserSecurityRepo();

            CreateUserSessionRepo();

            CreateUserExternalActions();

            CreateUserRoles();

            CreateUserAccessTokens();
        } 
        #endregion

        /// <summary>
        /// Creates the user repo.
        /// </summary>
        protected virtual void CreateUserRepo() =>
            RepositoryUsers = new RepositoryMemory<Guid, U>();

        /// <summary>
        /// Creates the user security repo.
        /// </summary>
        protected virtual void CreateUserSecurityRepo() =>
            RepositoryUserSecurities = new RepositoryMemory<Guid, USEC>();

        /// <summary>
        /// Creates the user session repo.
        /// </summary>
        protected virtual void CreateUserSessionRepo() =>
            RepositoryUserSessions = new RepositoryMemory<Guid, USES>();

        /// <summary>
        /// Creates the user external actions.
        /// </summary>
        protected virtual void CreateUserExternalActions() =>
            RepositoryUserExternalActions = new RepositoryMemory<Guid, UEXA>();

        /// <summary>
        /// Creates the user roles.
        /// </summary>
        protected virtual void CreateUserRoles() =>
            RepositoryUserRoles = new RepositoryMemory<Guid, UR>();

        /// <summary>
        /// Creates the user access tokens.
        /// </summary>
        protected virtual void CreateUserAccessTokens() =>
            RepositoryUserAccessTokens = new RepositoryMemory<Guid, UAT>();

    }
}
