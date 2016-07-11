using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Owin;

namespace Xigadee
{


    /// <summary>
    /// This is the populator for the WebApi.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class PopulatorWebApiUnity<M,C>: PopulatorWebApiBase<M, C>
        where M : Microservice, new()
        where C : ConfigBase, new()
    {
        /// <summary>
        /// This is the Unity container used within the application.
        /// </summary>
        public IUnityContainer Unity { get; }

        /// <summary>
        /// This constructor creates the Unity container.
        /// </summary>
        public PopulatorWebApiUnity()
        {
            Unity = new UnityContainer();
        }

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

        public virtual void Start(IAppBuilder app)
        {
            //AreaRegistration.RegisterAllAreas();
            var config = new HttpConfiguration();
            
            app.UseWebApi(config);

            Populate();


            Service.Start();

            //ConfigureAuth(app);

        }

        /// <summary>
        /// This method starts the microservice.
        /// </summary>
        public override void Start()
        {
            try
            {
                base.Start();

                // Register the log container so that we can log to the same loggers as the microservce code
                Unity.RegisterInstance(Service.Logger);

                // Register the config to ensure that the azure cloud settings can be pulled out of config not just the web.config settings - used by owin auth
                Unity.RegisterInstance(Config);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
