using System;

namespace Xigadee
{
    /// <summary>
    /// This helper is used to set the repository as memory backed.
    /// </summary>
    public static class UserSecurityModuleHelper
    {
        /// <summary>
        /// This set the repositories to be memory backed. This is used for testing.
        /// </summary>
        /// <param name="module">The User Security module.</param>
        public static UserSecurityModule<User, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken> SetAsMemoryBased(this UserSecurityModule<User, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken> module)
        {
            return module.SetAsMemoryBased<User, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>();
        }

        /// <summary>
        /// This set the repositories to be memory backed. This is used for testing.
        /// </summary>
        /// <typeparam name="U">The user type.</typeparam>
        /// <param name="module">The User Security module.</param>
        public static UserSecurityModule<U, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken> SetAsMemoryBased<U>(this UserSecurityModule<U, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken> module) 
            where U : User
        {
            return module.SetAsMemoryBased<U, UserSecurity, UserSession, UserExternalAction, UserRoles, UserAccessToken>();
        }

        /// <summary>
        /// This set the repositories to be memory backed. This is used for testing.
        /// </summary>
        /// <typeparam name="U">The user type.</typeparam>
        /// <typeparam name="USEC">The user security type.</typeparam>
        /// <typeparam name="USES">The user session type.</typeparam>
        /// <typeparam name="UEXA">The user external action type.</typeparam>
        /// <typeparam name="UR">The user roles type.</typeparam>
        /// <typeparam name="UAT">The user access token type.</typeparam>
        /// <param name="module">The User Security module.</param>
        public static UserSecurityModule<U, USEC, USES, UEXA, UR, UAT> SetAsMemoryBased<U, USEC, USES, UEXA, UR, UAT>(this UserSecurityModule<U, USEC, USES, UEXA, UR, UAT> module)
            where U : User
            where USEC : UserSecurity
            where USES : UserSession
            where UEXA : UserExternalAction
            where UR : UserRoles
            where UAT : UserAccessToken
        {
            module.RepositoryUsers = new RepositoryMemory<Guid, U>();
            module.RepositoryUserSessions = new RepositoryMemory<Guid, USES>();
            module.RepositoryUserSecurities = new RepositoryMemory<Guid, USEC>();
            module.RepositoryUserExternalActions = new RepositoryMemory<Guid, UEXA>();
            module.RepositoryUserAccessTokens = new RepositoryMemory<Guid, UAT>();
            module.RepositoryUserRoles = new RepositoryMemory<Guid, UR>();

            return module;
        }
    }
}
