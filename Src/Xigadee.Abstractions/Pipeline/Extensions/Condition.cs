using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This method can be used to call out the pipeline flow to an external method.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <param name="condition">A boolean condition for the call out.</param>
        /// <param name="whenTrue">The condition true action.</param>
        /// <param name="whenFalse">The condition false action.</param>
        /// <returns>Returns the original Pipeline.</returns>
        public static P Condition<P>(this P pipe, Func<IEnvironmentConfiguration, bool> condition
            , Action<P> whenTrue = null
            , Action<P> whenFalse = null
            )
            where P : IPipelineBase
        {
            if (condition == null)
                throw new ArgumentNullException("condition", "condition cannot be null for this pipeline extension.");

            bool success = condition(pipe.ToConfiguration());

            if (success)
                whenTrue?.Invoke(pipe);
            else
                whenFalse?.Invoke(pipe);

            return pipe;
        }


    }
}
