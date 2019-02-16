namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {

        /// <summary>
        /// This method returns true if the names key exists.
        /// </summary>
        /// <param name="Configuration">The configuration collection.</param>
        /// <param name="key">The key to check.</param>
        /// <param name="errorMessage"></param>
        public static void ConfigAssertKeyPresent(this IEnvironmentConfiguration Configuration, string key, string errorMessage = null)
        {
            if (Configuration.CanResolve(key))
                return;

            throw new ConfigAssertKeyNotPresentException(key,errorMessage);
        }
    }
}
