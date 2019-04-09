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
    /// <summary>
    /// This class is used by all services for the application.
    /// </summary>
    /// <typeparam name="CTX">The application context type.</typeparam>
    public abstract class JwtApiMicroserviceStartupBase<CTX> : ApiStartupBase<CTX>
        where CTX : JwtApiMicroserviceStartUpContext, new()
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtApiMicroserviceStartupBase{CTX}"/> class.
        /// </summary>
        /// <param name="env">The environment.</param>
        protected JwtApiMicroserviceStartupBase(Microsoft.AspNetCore.Hosting.IHostingEnvironment env) : base(env)
        {
        }
        #endregion

        #region ConfigureSecurity(IApplicationBuilder app)
        /// <summary>
        /// This override turns authentication on.
        /// </summary>
        /// <param name="app">The application.</param>
        protected override void ConfigureSecurity(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
        #endregion
        #region ConfigureSecurityAuthentication(IServiceCollection services)
        /// <summary>
        /// Configures the authentication using the Jwt token configuration.
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureSecurityAuthentication(IServiceCollection services)
        {
            services.AddJwtAuthentication(Context.SecurityJwt);
        }
        #endregion
    }

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
        [SingletonRegistration]
        public ConfigAuthenticationJwt SecurityJwt { get; set; } 
        #endregion

        #region CXB => ModulesCreate(IServiceCollection services)
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="services">The services.</param>
        public override void ModulesCreate(IServiceCollection services)
        {
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
        [SingletonRegistration(typeof(IApiUserSecurityModule))]
        public IApiUserSecurityModule UserSecurityModule { get; set; }

        /// <summary>
        /// Users the security module create.
        /// </summary>
        /// <returns></returns>
        protected abstract IApiUserSecurityModule UserSecurityModuleCreate();

    }
    #endregion
}
