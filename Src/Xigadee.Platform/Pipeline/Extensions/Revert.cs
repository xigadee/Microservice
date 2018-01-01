namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method reverts a pipeline extension to the parent pipeline.
        /// </summary>
        /// <typeparam name="P">The pipeline extension type.</typeparam>
        /// <param name="cpipe">The pipeline extension.</param>
        /// <returns>The parent Microservice pipeline.</returns>
        public static P Revert<P>(this IPipelineExtension<P> cpipe)
            where P : class, IPipeline
        {
            return cpipe.Pipeline;
        }
    }
}
