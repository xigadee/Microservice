using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method changes the default Microservice task manager policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyTaskManager<P>(this P pipeline
            , Action<TaskManagerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.TaskManager, pipeline.Configuration);

            return pipeline;
        }

        /// <summary>
        /// This extension method changes the default Microservice task manager policy to support async IO.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="maxPriorityLevel">The maximum priority level. The default is 3.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyTaskManagerForAsyncIO<P>(this P pipeline, int maxPriorityLevel = 3) where P : IPipeline
        {
            pipeline.AdjustPolicyTaskManager((p,c) =>
            {
                //p.
            });

            return pipeline;
        }

        /// <summary>
        /// This extension method changes the default Microservice task manager policy to support async IO.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="maxPriorityLevel">The maximum priority level. The default is 3.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyTaskManagerForDebug<P>(this P pipeline, int maxPriorityLevel = 3) where P : IPipeline
        {
            pipeline.AdjustPolicyTaskManager((t, c) =>
            {
                t.ConcurrentRequestsMin = 1;
                t.ConcurrentRequestsMax = 4;
                t.TransmissionPayloadTraceEnabled = true;
            });

            return pipeline;
        }
    }
}
