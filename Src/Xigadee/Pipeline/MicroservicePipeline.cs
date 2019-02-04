using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// The Microservice pipeline is used by extension methods to create a simple channel based service configuration.
    /// </summary>
    public class MicroservicePipeline: IPipeline
    {
        /// <summary>
        /// This is the default constructor for the pipeline.
        /// </summary>
        /// <param name="name">The Microservice name.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="description">An optional description of the Microservice.</param>
        /// <param name="policy">The policy settings collection.</param>
        /// <param name="properties">Any additional property key/value pairs.</param>
        /// <param name="config">The environment config object</param>
        /// <param name="assign">The action can be used to assign items to the microservice.</param>
        /// <param name="configAssign">This action can be used to adjust the config settings.</param>
        /// <param name="addDefaultJsonPayloadSerializer">This property specifies that the default Json 
        /// payload serializer should be added to the Microservice, set this to false to disable this.</param>
        /// <param name="addDefaultPayloadCompressors">This method ensures the Gzip and Deflate compressors are added to the Microservice.</param>
        /// <param name="serviceVersionId">This is the version id of the calling assembly as a string.</param>
        /// <param name="serviceReference">This is a reference type used to identify the version id of the root assembly.</param>
        public MicroservicePipeline(string name = null
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
            )
        {
            Configuration = config ?? new ConfigBase();
            configAssign?.Invoke(Configuration);

            serviceVersionId = serviceVersionId ?? serviceReference?.Assembly.GetName().Version.ToString();

            Service = new Microservice(name, serviceId, description, policy, properties, serviceVersionId);
            assign?.Invoke(Service);

            if (addDefaultJsonPayloadSerializer)
                this.AddPayloadSerializerDefaultJson();

            if (addDefaultPayloadCompressors)
                this.AddPayloadCompressorsDefault();
        }


        /// <summary>
        /// This is the default pipeline.
        /// </summary>
        /// <param name="service">The microservice.</param>
        /// <param name="config">The microservice configuration.</param>
        public MicroservicePipeline(IMicroservice service, IEnvironmentConfiguration config)
        {
            if (service == null)
                throw new ArgumentNullException("service cannot be null");
            if (config == null)
                throw new ArgumentNullException("config cannot be null");

            Service = service;
            Configuration = config;
        }

        /// <summary>
        /// This is the microservice.
        /// </summary>
        public virtual IMicroservice Service { get; protected set;}

        /// <summary>
        /// This is the microservice configuration.
        /// </summary>
        public virtual IEnvironmentConfiguration Configuration { get; protected set;}

    }
}
