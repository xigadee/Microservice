using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This method can be used to call out the pipeline flow to an external method.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <param name="method">The method to call.</param>
        /// <param name="condition">A boolean condition for the call out. If not set then this is true.</param>
        /// <returns>Returns the original Pipeline.</returns>
        public static P CallOut<P>(this P pipe
            , Action<P> method
            , Func<IEnvironmentConfiguration, bool> condition = null
            )
            where P : IPipelineBase
        {
            if (condition?.Invoke(pipe.ToPipeline().Configuration) ?? true)
                method?.Invoke(pipe);

            return pipe;
        }

    }
}
