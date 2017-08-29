//#region Copyright
//// Copyright Hitachi Consulting
//// 
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// 
////    http://www.apache.org/licenses/LICENSE-2.0
//// 
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// limitations under the License.
//#endregion

//using System;

//namespace Xigadee
//{
//    public static partial class AzureExtensionMethods
//    {
//        /// <summary>
//        /// This is the default encryption key settings key.
//        /// </summary>
//        [ConfigSettingKey("EncryptionKey")]
//        public const string KeyEncryptionKey = "EncryptionKey";

//        [ConfigSettingKey("EncryptionKey")]
//        public const string KeyEncryptionKeySize = "EncryptionKeySize";

//        [ConfigSetting("Encryption")]
//        public static string EncryptionKey(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEncryptionKey);

//        [ConfigSetting("Encryption")]
//        public static int? EncryptionKeySize(this IEnvironmentConfiguration config) => string.IsNullOrEmpty(config.PlatformOrConfigCache(KeyEncryptionKeySize)) ? default (int?) : config.PlatformOrConfigCacheInt(KeyEncryptionKeySize);

//        [ConfigSetting("Encryption")]
//        public static AesEncryptionHandler AesEncryption(this IEnvironmentConfiguration config)
//        {
//            return string.IsNullOrEmpty(config.EncryptionKey()) ? null : new AesEncryptionHandler(Convert.FromBase64String(config.EncryptionKey()), false, config.EncryptionKeySize());
//        }

//        [ConfigSetting("Encryption")]
//        public static AesEncryptionHandler AesEncryptionWithCompression(this IEnvironmentConfiguration config)
//        {
//            return string.IsNullOrEmpty(config.EncryptionKey()) ? null : new AesEncryptionHandler(Convert.FromBase64String(config.EncryptionKey()), true, config.EncryptionKeySize());
//        }

//        /// <summary>
//        /// This config override sets the AES encryption key for the Microservice.
//        /// </summary>
//        /// <param name="pipeline">The incoming pipeline.</param>
//        /// <param name="EncryptionKey">The Base64 encoded encryption key..</param>
//        /// <param name="EncryptionKeySize">The AES encryption key size.</param>
//        /// <returns>The pass-through of the pipeline.</returns>
//        public static P ConfigOverrideSetEncryption<P>(this P pipeline, string EncryptionKey, int? EncryptionKeySize = null)
//            where P : IPipeline
//        {
//            pipeline.ConfigurationOverrideSet(KeyEncryptionKey, EncryptionKey);
//            if (EncryptionKeySize.HasValue)
//                pipeline.ConfigurationOverrideSet(KeyEncryptionKeySize, EncryptionKeySize?.ToString());
//            return pipeline;
//        }
//    }
//}
