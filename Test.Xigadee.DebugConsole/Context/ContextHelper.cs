using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    static class ContextHelper
    {
        public static void SetPersistenceCacheOption(this Context c, string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "server":
                    c.ServerCacheEnabled = true;
                    break;
                case "client":
                    c.ClientCacheEnabled = true;
                    break;
                case "clientserver":
                    c.ServerCacheEnabled = c.ClientCacheEnabled = true;
                    break;
            }
        }

        public static void SetPersistenceOption(this Context c, string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "sql":
                    c.PersistenceType = PersistenceOptions.Sql;
                    break;
                case "blob":
                    c.PersistenceType = PersistenceOptions.Blob;
                    break;
                case "docdbsdk":
                    c.PersistenceType = PersistenceOptions.DocumentDbSdk;
                    break;
                case "docdb":
                    c.PersistenceType = PersistenceOptions.DocumentDb;
                    break;
                case "redis":
                    c.PersistenceType = PersistenceOptions.RedisCache;
                    break;
                case "memory":
                    c.PersistenceType = PersistenceOptions.Memory;
                    break;
                default:
                    c.PersistenceType = PersistenceOptions.DocumentDb;
                    break;
            }
        }

    }

}
