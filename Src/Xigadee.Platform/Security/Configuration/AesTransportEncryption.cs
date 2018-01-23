namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        [ConfigSettingKey("AesTransportEncryption")]
        public const string KeyAesTransportEncryptionKey = "AesTransportEncryptionKey";
        [ConfigSettingKey("AesTransportEncryption")]
        public const string KeyAesTransportEncryptionKeySize = "AesTransportEncryptionKeySize";
        [ConfigSettingKey("AesTransportEncryption")]
        public const string KeyAesTransportEncryptionUseCompression = "AesTransportEncryptionUseCompression";

        [ConfigSetting("AesTransportEncryption")]
        public static string AesTransportEncryptionKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyAesTransportEncryptionKey, throwExceptionIfNotFound:true);
        [ConfigSetting("AesTransportEncryption")]
        public static bool AesTransportEncryptionUseCompression(this IEnvironmentConfiguration config) => config.PlatformOrConfigCacheBool(KeyAesTransportEncryptionKey, "true");
        [ConfigSetting("AesTransportEncryption")]
        public static int AesTransportEncryptionKeySize(this IEnvironmentConfiguration config) => config.PlatformOrConfigCacheInt(KeyAesTransportEncryptionKeySize, 128);


    }
}
