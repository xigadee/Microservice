using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class UserSecurityModuleBase : IApiUserSecurityModule
    {
        public virtual string Realm { get; set; }

        public virtual IRepositoryAsync<Guid, User> Users { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserSecurity> UserSecurity { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserSession> UserSession { get; protected set; }

        public virtual IRepositoryAsync<Guid, UserExternalAction> UserExternalAction { get; protected set; }

        public virtual ILogger Logger { get; set; }


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

        public Task<(bool success, UserExternalAction uExAc)> RetrieveUserExternalAction(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool success, UserSecurity uSec)> RetrieveUserSecurity(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool success, UserSession uSess)> RetrieveUserSession(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// This module is used for testing purposes.
    /// </summary>
    /// <seealso cref="Xigadee.IApiUserSecurityModule" />
    public class UserSecurityModuleMemory : UserSecurityModuleBase
    {



    }
}
