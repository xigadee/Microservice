using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension allows for specific parts of the Microservice to be inspected or altered.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="msInspect">An action to inspect the Microservice</param>
        /// <param name="cfInspect">An action to inspect the configuration.</param>
        /// <returns>The pass through of the pipeline.</returns>
        public static P Inspect<P>(this P pipeline
            , Action<IMicroservice> msInspect = null
            , Action<IEnvironmentConfiguration> cfInspect = null)
            where P: IPipeline
        {
            msInspect?.Invoke(pipeline.Service);
            cfInspect?.Invoke(pipeline.Configuration);

            return pipeline;
        }
    }
}
