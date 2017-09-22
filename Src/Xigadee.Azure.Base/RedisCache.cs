#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion


namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
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
