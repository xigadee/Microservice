using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    #region JwtApiMicroserviceStartUpContext
    /// <summary>
    /// This is the default start up context.
    /// </summary>
    public abstract class JwtApiMicroserviceStartUpContext : ApiStartUpContext
    {
        #region 2b.Bind()
        /// <summary>
        /// Creates and binds specific configuration components required by the application.
        /// </summary>
        protected override void Bind()
        {
            base.Bind();

            SecurityJwt = new ConfigAuthenticationJwt();
            if (!string.IsNullOrEmpty(BindNameSecurityJwt))
                Configuration.Bind(BindNameSecurityJwt, SecurityJwt);
        }
        #endregion

        #region SecurityJwt
        /// <summary>
        /// Gets the bind name for SecurityJwt.
        /// </summary>
        protected virtual string BindNameSecurityJwt => "SecurityJwt";
        /// <summary>
        /// Gets or sets the JWT security settings.
        /// </summary>
        [RegisterAsSingleton]
        public ConfigAuthenticationJwt SecurityJwt { get; set; }
        #endregion

        #region CXB => ModulesCreate(IServiceCollection services)
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="services">The services.</param>
        public override void ModulesCreate(IServiceCollection services)
        {
            base.ModulesCreate(services);
            UserSecurityModule = UserSecurityModuleCreate();
        }
        #endregion

        #region Connect(ILoggerFactory lf)
        /// <summary>
        /// Sets the user security module logger.
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        public override void Connect(ILoggerFactory lf)
        {
            base.Connect(lf);
            UserSecurityModule.Logger = lf.CreateLogger<IApiUserSecurityModule>();
        }
        #endregion

        /// <summary>
        /// Gets or sets the user security module that is used to manages the security entities and user logic.
        /// </summary>
        [RegisterAsSingleton(typeof(IApiUserSecurityModule))]
        [RepositoriesProcess]
        public IApiUserSecurityModule UserSecurityModule { get; set; }

        /// <summary>
        /// Users the security module create.
        /// </summary>
        /// <returns></returns>
        protected abstract IApiUserSecurityModule UserSecurityModuleCreate();

    }
    #endregion

}
