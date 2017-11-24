using System.Security.Principal;

namespace Xigadee
{
    public class ApimPrincipal : GenericPrincipal
    {
        public ApimPrincipal(IIdentity identity, string[] roles) : base(identity, roles)
        {
        }
    }
}
