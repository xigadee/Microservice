using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Xigadee
{
    /// <summary>
    /// This extension pipeline is used by the AspNetCore pipeline.
    /// </summary>
    public class AspNetCoreMicroservicePipeline: MicroservicePipeline, IPipelineAspNetCore
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor for the pipeline.
        /// </summary>
        /// <param name="app">The AspNetCore application.</param>
        /// <param name="name">The Microservice name.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="description">This is an optional Microservice description.</param>
        /// <param name="policy">The policy settings collection.</param>
        /// <param name="properties">Any additional property key/value pairs.</param>
        /// <param name="config">The environment config object</param>
        /// <param name="assign">The action can be used to assign items to the microservice.</param>
        /// <param name="configAssign">This action can be used to adjust the config settings.</param>
        /// <param name="addDefaultJsonPayloadSerializer">This property specifies that the default Json payload serializer should be added to the Microservice, set this to false to disable this.</param>
        /// <param name="addDefaultPayloadCompressors">This method ensures the Gzip and Deflate compressors are added to the Microservice.</param>
        /// <param name="serviceVersionId">This is the version id of the calling assembly as a string.</param>
        /// <param name="serviceReference">This is a reference type used to identify the version id of the root assembly.</param>
        public AspNetCoreMicroservicePipeline(IApplicationBuilder app
            , string name = null
            , string serviceId = null
            , string description = null
            , IEnumerable<PolicyBase> policy = null
            , IEnumerable<Tuple<string, string>> properties = null
            , IEnvironmentConfiguration config = null
            , Action<IMicroservice> assign = null
            , Action<IEnvironmentConfiguration> configAssign = null
            , bool addDefaultJsonPayloadSerializer = true
            , bool addDefaultPayloadCompressors = true
            , string serviceVersionId = null
            , Type serviceReference = null
            ) : base(name, serviceId, description, policy, properties, config, assign, configAssign, addDefaultJsonPayloadSerializer, addDefaultPayloadCompressors, serviceVersionId, serviceReference)
        {
            App = app ?? throw new ArgumentNullException("app");
        }

        /// <summary>
        /// This is the default pipeline.
        /// </summary>
        /// <param name="service">The microservice.</param>
        /// <param name="config">The microservice configuration.</param>
        /// <param name="app">The AspNetCore application.</param>
        public AspNetCoreMicroservicePipeline(IMicroservice service
            , IEnvironmentConfiguration config
            , IApplicationBuilder app) : base(service, config)
        {
            App = app ?? throw new ArgumentNullException("app");
        }
        #endregion

        #region App
        /// <summary>
        /// This is the AspNetCore application
        /// </summary>
        public IApplicationBuilder App { get; protected set; }
        #endregion
    }
}
