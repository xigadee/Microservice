using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestStartupContext : JwtApiStartUpContextBase<ConfigApiService, IApiUserSecurityModule, ConfigAuthorization>
    {
        protected override IApiUserSecurityModule CreateUserSecurityModule()
        {
            throw new NotImplementedException();
        }
    }
}
