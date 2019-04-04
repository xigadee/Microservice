using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee
{
    /// <summary>
    /// This class holds the application settings.
    /// </summary>
    /// <seealso cref="Xigadee.ApiMicroserviceStartUpContext" />
    public class TestStartupContext : JwtApiMicroserviceStartUpContext
    {
        protected override IApiUserSecurityModule UserSecurityModuleCreate()
        {
            var usm = new UserSecurityModuleMemoryTest();
            //Add test security accounts here.

            return usm;
        }
    }
}
