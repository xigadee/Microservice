using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This is the default start up context.
    /// </summary>
    /// <seealso cref="Xigadee.IApiMicroservice" />
    public class ApiStartUpContext : IApiMicroservice
    {
        #region Initialize(IHostingEnvironment env = null)
        /// <summary>
        /// Initializes the context.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        public virtual void Initialize(IHostingEnvironment env = null)
        {
            Environment = env;

            //Determine whether to automatically build the configuration and then call Bind.
            BuildConfiguration();
        }
        #endregion
        #region BuildConfiguration()
        /// <summary>
        /// Builds the configuration and then binds the configuration objects.
        /// </summary>
        protected virtual void BuildConfiguration()
        {
            Build();
            Bind();
        }
        #endregion

        #region Build()
        /// <summary>
        /// Builds the Adapters configuration and then populates the necessary services.
        /// </summary>
        /// <returns>Returns the configuration builder.</returns>
        protected virtual void Build()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        #endregion

        /// <summary>
        /// Creates and set the specific configuration based on the configuration.
        /// </summary>
        protected virtual void Bind()
        {

        }

        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        public virtual IHostingEnvironment Environment { get; set; }
        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        public virtual IConfiguration Configuration { get; set; }
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public virtual ILogger Logger { get; set; }

        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        public virtual void Connect(ILoggerFactory lf)
        {
            Logger = lf.CreateLogger<IApiMicroservice>();
        }
    }

    /// <summary>
    /// This is the base API application context class.
    /// </summary>
    /// <typeparam name="MSCONF">The microservice configuration type.</typeparam>
    /// <typeparam name="MODSEC">The user security module type.</typeparam>
    /// <typeparam name="CONATEN">The authentication configuration type..</typeparam>
    /// <typeparam name="CONATHZ">The authorization configuration type.</typeparam>
    public abstract class ApiStartUpContextBase<MSCONF, MODSEC, CONATEN, CONATHZ> : ApiStartUpContext, IApiMicroservice<MODSEC, CONATEN, CONATHZ>
        where MSCONF : ConfigApplication, new()
        where MODSEC : IApiUserSecurityModule
        where CONATEN : ConfigAuthentication, new()
        where CONATHZ : ConfigAuthorization, new()
    {

        #region Bind()
        /// <summary>
        /// Creates and set the specific configuration based on the configuration.
        /// </summary>
        protected override void Bind()
        {
            Configuration.Bind("ConfigurationMicroservice", ConfigurationMicroservice);
            ConfigurationMicroservice.Name = Environment.ApplicationName;
            ConfigurationMicroservice.Connections = Configuration.GetSection("ConnectionStrings").GetChildren().ToDictionary((e) => e.Key, (e) => e.Value);

            //Authorization
            Configuration.Bind("ConfigurationAuthorization", ConfigurationAuthorization);

            //Authentication
            ConfigurationAuthentication.SetEnvironmentName(Environment.EnvironmentName);
            Configuration.Bind("ConfigurationAuthentication", ConfigurationAuthentication);

            //Secrets and Certificates
            ConfigureSecurityModules();

            //Set the Microservice Identity
            string instanceId = System.Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            string siteName = System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
            string iisSiteName = System.Environment.GetEnvironmentVariable("WEBSITE_IIS_SITE_NAME");
            var url = string.IsNullOrEmpty(siteName) ? "http://localhost" : $"https://{siteName}.azurewebsites.net/";
            var ass = GetType().Assembly;
            Identity = new MicroserviceId(siteName, Environment.ApplicationName.Replace(".", "")
                , serviceVersionId: ass.GetName().Version.ToString());
            //, url, instanceId, Environment.EnvironmentName);//, ConfigurationAuthentication?.Jwt?.Audience);

            UserSecurityModule = CreateUserSecurityModule();
        }
        #endregion

        /// <summary>
        /// Configures the security modules, by default, the secret module is not functional, and the certificate module can only retrieve local certificates.
        /// </summary>
        protected virtual void ConfigureSecurityModules()
        {
            SecretModule = new SecretModule();
            CertificateModule = new CertificateModule(SecretModule);
        }

        protected abstract MODSEC CreateUserSecurityModule();

        /// <summary>
        /// Gets or sets the generic configuration microservice.
        /// </summary>
        public MSCONF ConfigurationMicroservice { get; protected set; } = new MSCONF();
        /// <summary>
        /// Gets the user security collection, which is used to authenticate a user.
        /// </summary>
        public MODSEC UserSecurityModule { get; set; }
        /// <summary>
        /// Gets the authentication service configuration .
        /// </summary>
        public CONATEN ConfigurationAuthentication { get; protected set; } = new CONATEN();
        /// <summary>
        /// Gets the authorization service configuration .
        /// </summary>
        public CONATHZ ConfigurationAuthorization { get; protected set; } = new CONATHZ();


        /// <summary>
        /// Gets or sets the certificate module.
        /// </summary>
        public IApiCertificateModule CertificateModule { get; set; }
        /// <summary>
        /// Gets or sets the secret module.
        /// </summary>
        public IApiSecretModule SecretModule { get; set; }
        /// <summary>
        /// Gets or sets the Microservice identity.
        /// </summary>
        public MicroserviceId Identity { get; set; }

        #region ConnectModules(ILoggerFactory lf)
        /// <summary>
        /// Connects the services together and sets the logger for each service..
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        public override void Connect(ILoggerFactory lf)
        {
            base.Connect(lf);

            SetBase(SecretModule, lf.CreateLogger<IApiSecretModule>());

            SetBase(CertificateModule, lf.CreateLogger<IApiCertificateModule>());

            SetBase(UserSecurityModule, lf.CreateLogger<IApiUserSecurityModule>());
        }
        #endregion
        #region SetBase(IModuleBase service, ILogger logger)
        /// <summary>
        /// This method sets the base properties for the services, such as the logger and the DLL version.
        /// </summary>
        /// <param name="service">The service to set.</param>
        /// <param name="logger">The logger instance.</param>
        protected void SetBase(IApiModuleBase service, ILogger logger)
        {
            if (logger != null)
                service.Logger = logger;
        }
        #endregion

    }
}
