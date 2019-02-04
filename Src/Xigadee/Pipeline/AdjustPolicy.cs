using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method changes the default Microservice policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyMicroservice<P>(this P pipeline
            , Action<MicroservicePolicy, IEnvironmentConfiguration> msAssign = null) where P: IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.Microservice, pipeline.Configuration);

            return pipeline;
        }

        /// <summary>
        /// This extension method changes the default Microservice resource tracking policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyResourceTracker<P>(this P pipeline
            , Action<ResourceContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.ResourceMonitor, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice command container policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyCommandContainer<P>(this P pipeline
            , Action<CommandContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.CommandContainer, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice communication policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyCommunication<P>(this P pipeline
            , Action<CommunicationContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.Communication, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice scheduler policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyScheduler<P>(this P pipeline
            , Action<SchedulerContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.Scheduler, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice security policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyServiceHandler<P>(this P pipeline
            , Action<ServiceHandlerContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.ServiceHandlers, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice data collection policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyDataCollection<P>(this P pipeline
            , Action<DataCollectionContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policies.DataCollection, pipeline.Configuration);

            return pipeline;
        }
    }
}
