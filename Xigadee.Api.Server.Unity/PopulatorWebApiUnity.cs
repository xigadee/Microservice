using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;
using Owin;
using Unity.WebApi;

namespace Xigadee
{


    /// <summary>
    /// This is the populator for the WebApi.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class PopulatorWebApiUnity<M, C>: PopulatorWebApiBase
        where M : Microservice, new()
        where C : ConfigWebApi, new()
    {
        #region Constructor
        /// <summary>
        /// This constructor creates the Unity container.
        /// </summary>
        public PopulatorWebApiUnity()
        {
            //ApiConfig.
            Unity = new UnityContainer();

            ApiConfig.DependencyResolver = new UnityDependencyResolver(Unity);
        }
        #endregion

        #region Unity
        /// <summary>
        /// This is the Unity container used within the application.
        /// </summary>
        public IUnityContainer Unity { get; }
        #endregion

        #region RegisterCommand...
        /// <summary>
        /// This method is used to register an API command.
        /// </summary>
        /// <typeparam name="I">The command interface.</typeparam>
        /// <typeparam name="P">The concrete instance.</typeparam>
        protected virtual void RegisterCommand<I, P>()
            where P : I, ICommand, new()
        {
            RegisterCommand<I, P>(new P());
        }
        /// <summary>
        /// This method is used to register an API command with Unity.
        /// </summary>
        /// <typeparam name="I">The command interface.</typeparam>
        /// <typeparam name="P">The concrete instance.</typeparam>
        /// <param name="instance">An instance of the concrete class.</param>
        protected virtual void RegisterCommand<I, P>(P instance)
            where P : I, ICommand
        {
            try
            {
                Service.RegisterCommand(instance);
                Unity.RegisterInstance<I>(instance);
            }
            catch (Exception ex)
            {
                //Trace.TraceError(ex.Message);
                throw;
            }
        }
        #endregion

        protected override void RegisterWebApiServices()
        {
            base.RegisterWebApiServices();

            // Register the log container so that we can log to the same loggers as the microservce code
            Unity.RegisterInstance(new LoggerInternal(Service));

            // Register the config to ensure that the azure cloud settings can be pulled out of config not just the web.config settings - used by owin auth
            Unity.RegisterInstance(Config);
        }

        protected class LoggerInternal: ILoggerExtended
        {
            private readonly Microservice mService;

            public LoggerInternal(Microservice service)
            {
                mService = service;
            }

            public async Task Log(LogEvent logEvent)
            {
                if (mService.Logger != null)
                    await mService.Logger.Log(logEvent);
            }

            public void LogException(Exception ex)
            {
                mService.Logger?.LogException(ex);
            }

            public void LogException(string message, Exception ex)
            {
                mService.Logger?.LogException(message, ex);
            }

            public void LogMessage(string message)
            {
                mService.Logger?.LogMessage(message);
            }

            public void LogMessage(LoggingLevel level, string message)
            {
                mService.Logger?.LogMessage(level, message);
            }

            public void LogMessage(LoggingLevel level, string message, string category)
            {
                mService.Logger?.LogMessage(level, message, category);
            }
        }
    }
}
