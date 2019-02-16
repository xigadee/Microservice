namespace Xigadee
{
    /// <summary>
    /// This static class holds many key and configuration shortcuts.
    /// </summary>
    public static class ConfigBaseHelperService
    {
        /// <summary>
        /// The Environment key.
        /// </summary>
        [ConfigSettingKey("Service")]
        public const string KeyEnvironment = "Environment";
        /// <summary>
        /// The Client key.
        /// </summary>
        [ConfigSettingKey("Service")]
        public const string KeyClient = "Client";
        /// <summary>
        /// The ServiceDisabled key.
        /// </summary>
        [ConfigSettingKey("Service")]
        public const string KeyServiceDisabled = "ServiceDisabled";

        /// <summary>
        /// This is the configuration shortcut that checks whether the service is marked as disabled.
        /// </summary>
        /// <param name="config">The configuration collection.</param>
        /// <returns>The setting value.</returns>
        [ConfigSetting("Service")]
        public static bool ServiceDisabled(this IEnvironmentConfiguration config) => config.PlatformOrConfigCacheBool(KeyServiceDisabled);
        /// <summary>
        /// This is a shortcut to the configuration setting for the client name.
        /// </summary>
        /// <param name="config">The configuration collection.</param>
        /// <returns>The setting value.</returns>
        [ConfigSetting("Service")]
        public static string Client(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyClient);
        /// <summary>
        /// This is the shortcut for the configuration setting for the environment name.
        /// </summary>
        /// <param name="config">The configuration collection.</param>
        /// <returns>The setting value.</returns>
        [ConfigSetting("Service")]
        public static string Environment(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEnvironment);

    }

}
