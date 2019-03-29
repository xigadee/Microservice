using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xigadee;

namespace Tests.Xigadee
{
    public class TestStartupContext: ApiStartUpContext
    {
        protected override void Bind()
        {
            base.Bind();

            SecurityJwt = new ConfigAuthenticationJwt();
            Configuration.Bind("SecurityJwt", SecurityJwt);
        }

        public ConfigAuthenticationJwt SecurityJwt { get; set; }
    }
}
