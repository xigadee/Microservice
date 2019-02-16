namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This is a helper method that identifies the current pipeline. It is useful for autocomplete identification. 
        /// This command does not do anything.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <returns>The pipeline.</returns>
        public static void Actual_SecurityPipeline<C>(this C pipe)
            where C: IPipelineSecurity
        {
        }

    }
}
