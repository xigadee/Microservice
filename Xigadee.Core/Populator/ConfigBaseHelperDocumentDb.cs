using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{

    public static class ConfigBaseHelperDocumentDb
    {
        public const string KeyDocDBAccountName = "DocDBAccountName";
        public const string KeyDocDBAccountAccessKey = "DocDBAccountAccessKey";
        public const string KeyDocDBDatabaseName = "DocDBDatabaseName";
        public const string KeyDocDBCollectionName = "DocDBCollectionName";


        public static DocumentDbConnection DocDBConnection(this ConfigBase config) => DocumentDbConnection.ToConnection(config.DocDBAccountName(), config.DocDBAccountAccessKey());

        public static string DocDBAccountName(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBAccountName);

        public static string DocDBAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBAccountAccessKey);

        public static string DocDBDatabaseName(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBDatabaseName);

        public static string DocDBCollectionName(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBCollectionName);
    }
}
