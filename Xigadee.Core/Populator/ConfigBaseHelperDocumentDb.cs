using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{

    public static class ConfigBaseHelperDocumentDb
    {
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBAccountName = "DocDBAccountName";
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBAccountAccessKey = "DocDBAccountAccessKey";
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBDatabaseName = "DocDBDatabaseName";
        [ConfigSettingKey("DocumentDb")]
        public const string KeyDocDBCollectionName = "DocDBCollectionName";


        [ConfigSetting("DocumentDb")]
        public static DocumentDbConnection DocDBConnection(this ConfigBase config) => DocumentDbConnection.ToConnection(config.DocDBAccountName(), config.DocDBAccountAccessKey());

        [ConfigSetting("DocumentDb")]
        public static string DocDBAccountName(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBAccountName);

        [ConfigSetting("DocumentDb")]
        public static string DocDBAccountAccessKey(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBAccountAccessKey);

        [ConfigSetting("DocumentDb")]
        public static string DocDBDatabaseName(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBDatabaseName);

        [ConfigSetting("DocumentDb")]
        public static string DocDBCollectionName(this ConfigBase config) => config.PlatformOrConfigCache(KeyDocDBCollectionName);
    }
}
