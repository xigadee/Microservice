﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Xigadee
{
    /// <summary>
    /// This is the default start up context.
    /// </summary>
    public class ApiStartUpContext : ApiStartUpContextRoot//, IApiStartupContext
    {

        #region Environment

        /// <summary>
        /// This is the host container.
        /// </summary>
        public virtual HostContainer HostContainer { get; protected set; }

        public override IConfiguration Configuration { get => HostContainer?.Configuration; set => throw new NotSupportedException(); }

        #endregion

        #region CXA => Initialize(IHostingEnvironment env)
        /// <summary>
        /// Initializes the context.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        public virtual void Initialize(HostContainer cont)
        {
            HostContainer = cont;

            Initialize();
        }
        #endregion
        #region 1.Build()
        /// <summary>
        /// Builds and sets the default configuration using the appsettings.json file and the appsettings.{Environment.EnvironmentName}.json file.
        /// </summary>
        protected override void Build()
        {
            var builder = new ConfigurationBuilder();

            builder
                .SetBasePath(HostContainer.HostEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{HostContainer.HostEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        #endregion

        //2. Bind
        #region BindConfigApplication()
        /// <summary>
        /// This in the application config binding.
        /// </summary>
        protected override void BindConfigApplication()
        {
            ConfigApplication = new ConfigApplication();
            if (!string.IsNullOrWhiteSpace(BindNameConfigApplication))
            {
                Configuration.Bind(BindNameConfigApplication, ConfigApplication);

                ConfigApplication.Connections = Configuration.GetSection("ConnectionStrings").GetChildren().ToDictionary((e) => e.Key, (e) => e.Value);
            }

        } 
        #endregion
        #region BindConfigMicroservice()
        /// <summary>
        /// This is the microservice config binding.
        /// </summary>
        protected override void BindConfigMicroservice()
        {
            ConfigMicroservice = new ConfigMicroservice();
            if (UseMicroservice)
            {
                if (!string.IsNullOrEmpty(BindNameConfigMicroservice))
                    Configuration.Bind(BindNameConfigMicroservice, ConfigMicroservice);
            }
        } 
        #endregion
        #region BindConfigHealthCheck()
        /// <summary>
        /// This is the config health check creation and binding.
        /// </summary>
        protected override void BindConfigHealthCheck()
        {
            ConfigHealthCheck = new ConfigHealthCheck();
            if (!string.IsNullOrWhiteSpace(BindNameConfigHealthCheck))
            {
                Configuration.Bind(BindNameConfigHealthCheck, ConfigHealthCheck);
            }
        }
        #endregion
        #region ServiceIdentitySet()
        /// <summary>
        /// This method sets the service identity for the application.
        /// This is primarily used for logging and contains the various parameters needed
        /// to identity this instance when debugging and logging.
        /// </summary>
        protected override void ServiceIdentitySet()
        {
            //Set the Microservice Identity
            string instanceId = System.Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
            string siteName = System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");

            var url = string.IsNullOrEmpty(siteName) ? "http://localhost" : $"https://{siteName}.azurewebsites.net/";

            var ass = GetType().Assembly;

            ServiceIdentity = new ApiServiceIdentity(Id
                , System.Environment.MachineName
                , HostContainer.HostEnvironment.ApplicationName
                , ass.GetName().Version.ToString()
                , url, instanceId
                , HostContainer.HostEnvironment.EnvironmentName);

        } 
        #endregion


        #region CXC => Connect(ILoggerFactory lf)
        /// <summary>
        /// Connects the application components and registers the relevant services.
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        public override void Connect(ILoggerFactory lf)
        {
            Logger = lf.CreateLogger<IApiStartupContextBase>();
        }
        #endregion



    }
}