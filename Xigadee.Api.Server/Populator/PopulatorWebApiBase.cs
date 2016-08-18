using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using Owin;
using Xigadee;
namespace Xigadee
{
    /// <summary>
    /// This class provides the basic set of method to set up a BFF WebApi layer.
    /// </summary>
    /// <typeparam name="M">The microsercice type.</typeparam>
    /// <typeparam name="C">The config type.</typeparam>
    public abstract class PopulatorWebApiBase<M, C>: PopulatorBase<M, C>
        where M : Microservice, new()
        where C : ConfigWebApi, new()
    {

        /// <summary>
        /// This is the filter that rejects requests until the core service is fully started.
        /// </summary>
        protected readonly WebApiServiceUnavailableFilter mRejectIfServiceNotStartedFilter;

        #region Constructor
        /// <summary>
        /// This constructor creates the Unity container.
        /// </summary>
        public PopulatorWebApiBase(HttpConfiguration config = null, Func<string, string, string> resolver = null, bool resolverFirst = true)
        {
            ApiConfig = config ?? new HttpConfiguration();
            mRejectIfServiceNotStartedFilter = new WebApiServiceUnavailableFilter();
            ApiConfig.Filters.Add(mRejectIfServiceNotStartedFilter);

            ConfigInitialise(resolver, resolverFirst);
        }
        #endregion

        #region ApiConfig
        /// <summary>
        /// This is the Unity container used within the application.
        /// </summary>
        public HttpConfiguration ApiConfig { get; protected set;}
        #endregion


        protected override void ServiceStatusChanged(object sender, StatusChangedEventArgs e)
        {
            mRejectIfServiceNotStartedFilter.StatusCurrent = e.StatusNew;
        }

        /// <summary>
        /// This method starts the microservice.
        /// </summary>
        public virtual void Start(IAppBuilder app, Func<string, string, string> settingsResolver = null)
        {
            try
            {
                Populate(settingsResolver);

                ApiConfig.Filters.Add(mRejectIfServiceNotStartedFilter);

                RegisterWebApiServices();

                RegisterCorrsPolicy();

                app.UseWebApi(ApiConfig);

                ApiConfig.EnsureInitialized();

                Task.Run(() =>Start());

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// This override sets the necessary request 
        /// </summary>
        protected virtual void RegisterWebApiServices()
        {
            ApiConfig.Filters.Add(new WebApiCorrelationIdFilter());

            ApiConfig.Filters.Add(new WebApiVersionHeaderFilter());

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //Service.ApiConfig.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //config.Filters.Add(CreateBlobLoggingFilter());


        }

        /// <summary>
        /// This sets the default Cors policy.
        /// </summary>
        protected virtual void RegisterCorrsPolicy()
        {
            //config.DependencyResolver = 
            ApiConfig.EnableCors(new InternalCorrsPolicy(CorsPolicyGet));
        }

        protected virtual async Task<CorsPolicy> CorsPolicyGet(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new CorsPolicy() { AllowAnyHeader = true, AllowAnyMethod = true, AllowAnyOrigin = true };
        }

        #region Class -> InternalCorrsPolicy
        private class InternalCorrsPolicy: ICorsPolicyProvider
        {
            private Func<HttpRequestMessage, CancellationToken, Task<CorsPolicy>> mRedirect;

            public InternalCorrsPolicy(Func<HttpRequestMessage, CancellationToken, Task<CorsPolicy>> Redirect)
            {
                mRedirect = Redirect;
            }

            public async Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await mRedirect(request, cancellationToken);
            }
        } 
        #endregion
    }
}
