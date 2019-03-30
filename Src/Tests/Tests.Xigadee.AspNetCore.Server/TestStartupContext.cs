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
    public class TestStartupContext: ApiMicroserviceStartUpContext
    {
        protected override void Bind()
        {
            base.Bind();

            SecurityJwt = new ConfigAuthenticationJwt();
            Configuration.Bind("SecurityJwt", SecurityJwt);
        }

        /// <summary>
        /// Gets or sets the JWT security settings.
        /// </summary>
        public ConfigAuthenticationJwt SecurityJwt { get; set; }
        /// <summary>
        /// Gets or sets the user security module that is used to manages the security entities and user logic.
        /// </summary>
        public IApiUserSecurityModule UserSecurityModule { get; set; }
    }
}
