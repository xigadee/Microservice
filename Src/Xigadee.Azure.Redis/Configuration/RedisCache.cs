
namespace Xigadee
{
    public static partial class AzureRedisCacheHelper
    {
        /// <summary>
        /// The reserved keyword.
        /// </summary>
        public const string RedisCache = "RedisCache";

        /// <summary>
        /// The Redis cache connection key
        /// </summary>
        [ConfigSettingKey(RedisCache)]
        public const string KeyRedisCacheConnection = "RedisCacheConnection";
        /// <summary>
        /// Retrieves the Redis cache connection value
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="throwExceptionIfNotFound">if set to <c>true</c> [throw exception if not found].</param>
        /// <returns>The connection value.</returns>
        [ConfigSetting(RedisCache)]
        public static string RedisCacheConnection(this IEnvironmentConfiguration config, bool throwExceptionIfNotFound = false) 
            => config.PlatformOrConfigCache(KeyRedisCacheConnection, throwExceptionIfNotFound: throwExceptionIfNotFound);

        /// <summary>
        /// This extension allows the Redis cache connection values to be manually set as override parameters.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="redisCacheConnection">The Redis cache connection.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P ConfigOverrideSetRedisCacheConnection<P>(this P pipeline, string redisCacheConnection)
            where P : IPipeline
        {
            pipeline.ConfigurationOverrideSet(KeyRedisCacheConnection, redisCacheConnection);
            return pipeline;
        }
    }
}
