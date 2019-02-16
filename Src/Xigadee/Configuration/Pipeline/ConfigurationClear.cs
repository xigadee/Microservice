namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method can be used to clear the default configuration settings, specifically the AppConfig settings that are inherited automatically.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <returns>Returns the original Pipeline.</returns>
        public static P ConfigurationClear<P>(this P pipe)
            where P : IPipeline
        {

            pipe.Configuration.ResolversClear();

            return pipe;
        }

    }
}
