using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This module is used for testing purposes.
    /// </summary>
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public class UserSecurityModuleMemory : UserSecurityModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurityModuleMemory"/> class.
        /// This class holds the security objects in memory only and can be used for unit testing.
        /// </summary>
        public UserSecurityModuleMemory()
        {
            Users = new RepositoryMemory<Guid, User>(EntityAuditableBase.KeyMaker
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<User>()
                );

            UserSecurities = new RepositoryMemory<Guid, UserSecurity>(EntityAuditableBase.KeyMaker
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserSecurity>()
                );

            UserSessions = new RepositoryMemory<Guid, UserSession>(EntityAuditableBase.KeyMaker
                , propertiesMaker: UserReferenceBase.PropertiesGet
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserSession>()
                );

            UserExternalActions = new RepositoryMemory<Guid, UserExternalAction>(EntityAuditableBase.KeyMaker
                , propertiesMaker: UserReferenceBase.PropertiesGet
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserExternalAction>()
                );

            UserRoles = new RepositoryMemory<Guid, UserRoles>(EntityAuditableBase.KeyMaker
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserRoles>()
                );

            UserAccessTokens = new RepositoryMemory<Guid, UserAccessToken>(EntityAuditableBase.KeyMaker
                , propertiesMaker: UserReferenceBase.PropertiesGet
                , versionPolicy: EntityAuditableBase.VersionPolicyStandard<UserAccessToken>()
                );
        }


    }
}
