using System.Security.Principal;

namespace Xigadee.Api
{
    public class ApimPrincipal : GenericPrincipal
    {
        public ApimPrincipal(IIdentity identity, string[] roles) : base(identity, roles)
        {
        }
    }
}
