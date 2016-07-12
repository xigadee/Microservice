using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee.Api.Server
{
    public class ConfigWebApi: ConfigBase
    {

        public Sha512SignatureHelper SignatureProvider => new Sha512SignatureHelper("A4514A8598CC4E77B46DA51FE62D00FB");


        public string AuthAudiences => PlatformOrConfigCache("AuthAudiences");
        //{"Audiences":[{"Name":"ValueRetail.Web.User","ClientId":"e02920f4b3eb4492b26991030265a0f6","Base64Secret":"G7AIbRzz4r0NC3u5RBnYauveRoCwqm5zmBJU6PDMaa4="},{"Name":"APIClient","ClientId":"4e8ce86a9c914dd8a41c09e0709ef297","Base64Secret":"71UOU6ewhSjuJ/sp+O/1qlvw4m66LkmSsaFljtHan4M=", "CustomTokenExpirationMin":"525949"}]}

        public string AuthIssuer => PlatformOrConfigCache("AuthIssuer");
        //https://vrapibffstaging.cloudapp.net/

        public string AuthTokenLifeInMin => PlatformOrConfigCache("AuthTokenLifeInMin","30");

        public bool AuthAllowInsecureHttp => PlatformOrConfigCacheBool("AuthAllowInsecureHttp", "true");
        /// <summary>
        /// Blob logging filter level
        /// </summary>
        public IList<string> WebLogFilterLevels => PlatformOrConfigCache("WebLogFilterLevels", "All")?.Split(',').ToList() ?? new List<string>();

        public bool ServiceDisabled => PlatformOrConfigCacheBool("ServiceDisabled");

        public string Client => PlatformOrConfigCache("Client");

        public string Environment => PlatformOrConfigCache("Environment");

        public string ServiceBusConnection => PlatformOrConfigCache("ServiceBusConnection");

        public string EntityRedisCacheConnection => PlatformOrConfigCache("EntityRedisCacheConnection");

        public string StorageAccountName => PlatformOrConfigCache("StorageAccountName");

        public string StorageAccountAccessKey => PlatformOrConfigCache("StorageAccountAccessKey");

        public StorageCredentials StorageCredentials
        {
            get
            {
                if (string.IsNullOrEmpty(StorageAccountName) || string.IsNullOrEmpty(StorageAccountAccessKey))
                    return null;

                return new StorageCredentials(StorageAccountName, StorageAccountAccessKey);
            }
        }

        public string LogStorageAccountName => PlatformOrConfigCache("LogStorageAccountName", StorageAccountName);

        public string LogStorageAccountAccessKey => PlatformOrConfigCache("LogStorageAccountAccessKey", StorageAccountAccessKey);

        public StorageCredentials LogStorageCredentials
        {
            get
            {
                if (string.IsNullOrEmpty(LogStorageAccountName) || string.IsNullOrEmpty(LogStorageAccountAccessKey))
                    return StorageCredentials;

                return new StorageCredentials(LogStorageAccountName, LogStorageAccountAccessKey);
            }
        }
    }
}