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
    public abstract class UserSecurityModuleBase : ApiModuleBase, IApiUserSecurityModule
    {
        public virtual string Realm { get; set; }

        public virtual IRepositoryAsync<Guid, User> Users { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserSecurity> UserSecurities { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserSession> UserSessions { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserExternalAction> UserExternalActions { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserRoles> UserRoles { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserAccessToken> UserAccessTokens { get; protected set; }

        public virtual async Task<(bool success, User user)> RetrieveUser(Guid id)
        {
            try
            {
                var rs = await Users.Read(id);

                return (rs.IsSuccess, rs.Entity);
            }
            catch (Exception ex)
            {
                Logger?.LogError("");
                return (false, null);
            }

        }

        public virtual async Task<(bool success, User user)> RetrieveUser(string type, string value)
        {
            try
            {
                var rs = await Users.ReadByRef(type,value);

                return (rs.IsSuccess, rs.Entity);
            }
            catch (Exception ex)
            {
                Logger?.LogError("");
                return (false, null);
            }

        }

        public virtual Task<(bool success, UserExternalAction uExAc)> RetrieveUserExternalAction(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<(bool success, UserSecurity uSec)> RetrieveUserSecurity(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<(bool success, UserSession uSess)> RetrieveUserSession(Guid id)
        {
            throw new NotImplementedException();
        }
    }

}
