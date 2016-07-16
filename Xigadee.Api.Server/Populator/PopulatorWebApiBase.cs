using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Owin;
using Xigadee;
namespace Xigadee
{
    /// <summary>
    /// This class provides the basic set of method to set up a BFF WebApi layer.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="C"></typeparam>
    public abstract class PopulatorWebApiBase: PopulatorWebApiBase<MicroserviceWebApi>
    {
        #region Constructor
        /// <summary>
        /// This constructor creates the Unity container.
        /// </summary>
        public PopulatorWebApiBase()
        {
            ApiConfig = new HttpConfiguration();
        }
        #endregion

        #region ApiConfig
        /// <summary>
        /// This is the Unity container used within the application.
        /// </summary>
        public HttpConfiguration ApiConfig { get; }
        #endregion

        /// <summary>
        /// This method starts the microservice.
        /// </summary>
        public virtual void Start(IAppBuilder app, Func<string, string, string> settingsResolver = null)
        {
            try
            {
                Populate(settingsResolver);
                Start();

                RegisterWebApiServices();

                app.UseWebApi(ApiConfig);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        protected virtual void RegisterWebApiServices()
        {
            ApiConfig.Filters.Add(new WebApiCorrelationIdFilter());
        }
    }

    /// <summary>
    /// This class provides the basic set of method to set up a BFF WebApi layer.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="C"></typeparam>
    public abstract class PopulatorWebApiBase<M>: PopulatorWebApiBase<M, ConfigWebApi>
        where M : Microservice, new()
    {

    }

    /// <summary>
    /// This class provides the basic set of method to set up a BFF WebApi layer.
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <typeparam name="C"></typeparam>
    public abstract class PopulatorWebApiBase<M, C>: PopulatorBase<M, C>
        where M : Microservice, new()
        where C : ConfigWebApi, new()
    {

    }
}
