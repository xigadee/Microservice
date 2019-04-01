using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This module is used for testing purposes.
    /// </summary>
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public class UserSecurityModuleMemoryTest : UserSecurityModuleBase
    {
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

        /// <summary>
        /// Creates the user repo.
        /// </summary>
        protected virtual void CreateUserRepo() =>
            RepoUsers = new RepositoryMemory<Guid, User>(EntityAuditableBase.KeyMaker
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<User>());

        /// <summary>
        /// Creates the user security repo.
        /// </summary>
        protected virtual void CreateUserSecurityRepo() =>
            RepoUserSecurities = new RepositoryMemory<Guid, UserSecurity>(EntityAuditableBase.KeyMaker
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserSecurity>()
                );

        /// <summary>
        /// Creates the user session repo.
        /// </summary>
        protected virtual void CreateUserSessionRepo() =>
            RepoUserSessions = new RepositoryMemory<Guid, UserSession>(EntityAuditableBase.KeyMaker
                , propertiesMaker: UserReferenceBase.PropertiesGet
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserSession>()
                );

        /// <summary>
        /// Creates the user external actions.
        /// </summary>
        protected virtual void CreateUserExternalActions() =>
            RepoUserExternalActions = new RepositoryMemory<Guid, UserExternalAction>(EntityAuditableBase.KeyMaker
                , propertiesMaker: UserReferenceBase.PropertiesGet
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserExternalAction>()
                );

        /// <summary>
        /// Creates the user roles.
        /// </summary>
        protected virtual void CreateUserRoles() =>
            RepoUserRoles = new RepositoryMemory<Guid, UserRoles>(EntityAuditableBase.KeyMaker
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserRoles>()
                );

        /// <summary>
        /// Creates the user access tokens.
        /// </summary>
        protected virtual void CreateUserAccessTokens() =>
            RepoUserAccessTokens = new RepositoryMemory<Guid, UserAccessToken>(EntityAuditableBase.KeyMaker
                , propertiesMaker: UserReferenceBase.PropertiesGet
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserAccessToken>()
                );

    }
}
