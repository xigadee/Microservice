using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using Xigadee;
namespace Test.Xigadee.Api.Server
{
    public static class Service
    {
        static PopulatorWebApi sPopulator;

        #region Constructor
        static Service()
        {
            sPopulator = new PopulatorWebApi();
        }
        #endregion

        #region Unity,Telemetry,Config
        /// <summary>
        /// The system wide Unity container.
        /// </summary>
        public static IUnityContainer Unity { get { return sPopulator.Unity; } }
        /// <summary>
        /// This is the system configuration.
        /// </summary>
        public static ConfigWebApi Config { get { return sPopulator.Config; } }
        #endregion

        /// <summary>
        /// This method initialises the Microservice.
        /// </summary>
        public static void Initialise()
        {
            try
            {
                sPopulator.Populate();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This method starts the microservice.
        /// </summary>
        public static void Start()
        {
            try
            {
                sPopulator.Start();

                // Register the log container so that we can log to the same loggers as the microservce code
                Unity.RegisterInstance(sPopulator.Service.Logger);

                // Register the config to ensure that the azure cloud settings can be pulled out of config not just the web.config settings - used by owin auth
                Unity.RegisterInstance(sPopulator.Config);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
