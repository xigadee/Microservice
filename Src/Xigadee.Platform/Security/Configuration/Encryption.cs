using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This is the default encryption handler id settings key.
        /// </summary>
        [ConfigSettingKey("EncryptionKey")]
        public const string KeyEncryptionId = "EncryptionId";
        /// <summary>
        /// This is the default encryption key settings key.
        /// </summary>
        [ConfigSettingKey("EncryptionKey")]
        public const string KeyEncryptionKey = "EncryptionKey";
        /// <summary>
        /// This is the default encryption key size settings key.
        /// </summary>
        [ConfigSettingKey("EncryptionKey")]
        public const string KeyEncryptionKeySize = "EncryptionKeySize";


        [ConfigSetting("Encryption")]
        public static string EncryptionId(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEncryptionId, "transport");


        [ConfigSetting("Encryption")]
        public static string EncryptionKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEncryptionKey);

        [ConfigSetting("Encryption")]
        public static int? EncryptionKeySize(this IEnvironmentConfiguration config) => string.IsNullOrEmpty(config.PlatformOrConfigCache(KeyEncryptionKeySize)) ? default (int?) : config.PlatformOrConfigCacheInt(KeyEncryptionKeySize);

        [ConfigSetting("Encryption")]
        public static AesEncryptionHandler AesEncryption(this IEnvironmentConfiguration config)
        {
            return string.IsNullOrEmpty(config.EncryptionKey()) ? null : 
                new AesEncryptionHandler(config.EncryptionId()
                    , Convert.FromBase64String(config.EncryptionKey()), false, config.EncryptionKeySize());
        }

        [ConfigSetting("Encryption")]
        public static AesEncryptionHandler AesEncryptionWithCompression(this IEnvironmentConfiguration config)
        {
            return string.IsNullOrEmpty(config.EncryptionKey()) ? null : 
                new AesEncryptionHandler(config.EncryptionId()
                    , Convert.FromBase64String(config.EncryptionKey()), true, config.EncryptionKeySize());
        }

        /// <summary>
        /// This config override sets the AES encryption key for the Microservice.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="EncryptionKey">The Base64 encoded encryption key..</param>
        /// <param name="EncryptionKeySize">The AES encryption key size.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P ConfigOverrideSetEncryption<P>(this P pipeline, string EncryptionKey, int? EncryptionKeySize = null)
            where P : IPipeline
        {
            pipeline.ConfigurationOverrideSet(KeyEncryptionKey, EncryptionKey);
            if (EncryptionKeySize.HasValue)
                pipeline.ConfigurationOverrideSet(KeyEncryptionKeySize, EncryptionKeySize?.ToString());
            return pipeline;
        }
    }
}
