using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This start-up class is responsible for managing the ASP.NET Pipline and interleaving it with the
    /// Xigadee service framework.
    /// </summary>
    /// <typeparam name="CTX">The startup context.</typeparam>
    /// <typeparam name="HE">The hosting environment.</typeparam>
    public abstract class ApiStartUpRoot<CTX, HE>
        where CTX : ApiStartUpContextRoot<HE>, new()
        where HE : HostingContainerBase
    {
        #region A=>Constructor
        /// <summary>
        /// Initializes a new instance of the API application class.
        /// </summary>
        /// <param name="host">The environment host.</param>
        protected ApiStartUpRoot(HE host)
        {
            Host = host;
            Initialize();
        }
        #endregion

        #region Host
        /// <summary>
        /// This is the hosting environment.
        /// </summary>
        public HE Host { get; protected set; }
        #endregion
        #region Context
        /// <summary>
        /// Gets or sets the Api application context.
        /// </summary>
        public CTX Context { get; protected set; }
        #endregion

        #region A. Initialize()
        /// <summary>
        /// This method initialize the initial services.
        /// </summary>
        protected virtual void Initialize()
        {
            ContextCreate();

            ContextInitialize();
        }
        #endregion
        #region A1. ContextCreate()
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextCreate()
        {
            Context = new CTX();
        }
        #endregion
        #region A2. ContextInitialize() -> CXA ->
        /// <summary>
        /// Initializes the context
        /// </summary>
        protected virtual void ContextInitialize() => Context.Initialize(Host);
        #endregion

        #region C=>Configure(IApplicationBuilder app)
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public virtual void Configure(IApplicationBuilder app) => Context.ConfigureApplication(app);
        #endregion
    }
}
