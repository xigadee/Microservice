using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the shared configuration class for both Microservices.
    /// </summary>
    public class ConfigConsole: ConfigBase
    {
        public string ServiceBusConnection => PlatformOrConfigCache("ServiceBusConnection");

        public string RedisCacheName => PlatformOrConfigCache("RedisCacheName");
        public string RedisCachePort => PlatformOrConfigCache("RedisCachePort", "6380");
        public string RedisCacheKey => PlatformOrConfigCache("RedisCacheKey");

        public string StorageName => PlatformOrConfigCache("StorageName");
        public string StorageKey => PlatformOrConfigCache("StorageKey");

        public string RedisCacheConnection
        {
            get
            {
                return $"{RedisCacheName}.redis.cache.windows.net:{RedisCachePort},ssl=true,password={RedisCacheKey}";
            }
        }

        public string SqlConnection => PlatformOrConfigCache("SqlConnection");

        public StorageCredentials Storage
        {
            get
            {
                return new StorageCredentials(StorageName, StorageKey);
            }
        }

        public DocumentDbConnection DocDbCredentials
        {
            get
            {
                return new DocumentDbConnection(DocumentDbName, DocumentDbKey);
            }
        }

        public string DocumentDbName => PlatformOrConfigCache("DocumentDbName");

        public string DocumentDbKey => PlatformOrConfigCache("DocumentDbKey");

        public string DocumentDbDatabase => PlatformOrConfigCache("DocumentDbDatabase");

    }
}


