using System;

namespace Xigadee
{
    /// <summary>
    /// These extensions allow services to be registered as part of a pipeline
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This pipeline extension is used to add a service to the Microservice shared service container.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="creator">A function that takes in the pipeline configuration and returns an instance of the service.</param>
        /// <param name="serviceName">The optional service name</param>
        /// <param name="action">An optional action to access the service on assignment.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddSharedService<P,I>(this P pipeline
            , Func<IEnvironmentConfiguration, I> creator, string serviceName = null, Action<I> action = null) where I : class
            where P : IPipeline
        {
            var service = creator(pipeline.Configuration);

            action?.Invoke(service);

            if (!pipeline.Service.Commands.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
        /// <summary>
        /// This pipeline extension is used to add a service to the Microservice shared service container.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="service">The service instance to add.</param>
        /// <param name="serviceName">The optional service name</param>
        /// <param name="action">An optional action to access the service on assignment.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddSharedService<P,I>(this P pipeline
            , I service, string serviceName = null, Action<I> action = null) where I : class
            where P : IPipeline
        {
            action?.Invoke(service);

            if (!pipeline.Service.Commands.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
        /// <summary>
        /// This pipeline extension is used to add a service to the Microservice shared service container.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="creator">A lazy creator for the service that is called when the service is first accessed.</param>
        /// <param name="serviceName">The optional service name</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddSharedService<P,I>(this P pipeline
            , Lazy<I> creator, string serviceName = null) where I : class
            where P : IPipeline
        {
            if (!pipeline.Service.Commands.SharedServices.RegisterService<I>(creator, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
    }
}
