﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee.AspNetCore50.Server
{
    public class StartupContext : JwtApiMicroserviceStartUpContext
    {
        protected override IApiUserSecurityModule UserSecurityModuleCreate()
        {
            var usm = new UserSecurityModule<TestUser>()
                .SetAsMemoryBased();

            //Add test security accounts here.
            var user = new TestUser() { Username = "paul" };
            var rs = usm.Users.Create(user).Result;

            var rsv = usm.Users.ReadByRef(TestUser.KeyUsername, "paul").Result;

            var uSec = new UserSecurity() { Id = user.Id };
            uSec.AuthenticationSet("", "123Enter.");
            var rs2 = usm.UserSecurities.Create(uSec).Result;

            var ur = new UserRoles() { Id = user.Id };
            ur.RoleAdd("paul");
            var rs3 = usm.UserRoles.Create(ur).Result;

            //uSec.

            return usm;
        }
    }
}