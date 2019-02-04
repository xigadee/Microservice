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

using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMenuServerPersistenceSettings = new Lazy<ConsoleMenu>(
            () =>
            {
                var menu = new ConsoleMenu(
                    "Persistence storage options"
                    //, new ConsoleOption("Sql based Persistence"
                    //    , (m, o) =>
                    //    {
                    //        sSettings.PersistenceType = PersistenceOptions.Sql;
                    //    }
                    //    , enabled: (m, o) => sServer.Config?.CanResolve(ConfigBaseHelperSql.KeySqlConnection) ?? false
                    //    , selected: (m, o) => sSettings.PersistenceType == PersistenceOptions.Sql
                    //)
                    //, new ConsoleOption("DocumentDb based Persistence"
                    //    , (m, o) =>
                    //    {
                    //        sSettings.PersistenceType = PersistenceOptions.DocumentDb;
                    //    }
                    //    , enabled: (m, o) => sServer.Config?.CanResolve(ConfigBaseHelperDocumentDb.KeyDocDBAccountName, ConfigBaseHelperDocumentDb.KeyDocDBAccountAccessKey) ?? false
                    //    , selected: (m, o) => sSettings.PersistenceType == PersistenceOptions.DocumentDb
                    //)
                    //, new ConsoleOption("DocumentDb Sdk based Persistence"
                    //    , (m, o) =>
                    //    {
                    //        sSettings.PersistenceType = PersistenceOptions.DocumentDbSdk;
                    //    }
                    //    , enabled: (m, o) => sServer.Config?.CanResolve(ConfigBaseHelperDocumentDb.KeyDocDBAccountName, ConfigBaseHelperDocumentDb.KeyDocDBAccountAccessKey) ?? false
                    //    , selected: (m, o) => sSettings.PersistenceType == PersistenceOptions.DocumentDbSdk
                    //)
                    //, new ConsoleOption("Blob storage based Persistence"
                    //    , (m, o) =>
                    //    {
                    //        sSettings.PersistenceType = PersistenceOptions.Blob;
                    //    }
                    //    , enabled: (m, o) => sServer.Config?.CanResolve(AzureExtensionMethods.KeyAzureStorageAccountName, AzureExtensionMethods.KeyAzureStorageAccountAccessKey) ?? false
                    //    , selected: (m, o) => sSettings.PersistenceType == PersistenceOptions.Blob
                    //)
                    //, new ConsoleOption("Redis Cache based Persistence"
                    //    , (m, o) =>
                    //    {
                    //        sSettings.PersistenceType = PersistenceOptions.RedisCache;
                    //    }
                    //    , enabled: (m, o) => sServer.Config?.CanResolve(AzureExtensionMethods.KeyRedisCacheConnection) ?? false
                    //    , selected: (m, o) => sSettings.PersistenceType == PersistenceOptions.RedisCache
                    //)
                    , new ConsoleOption("Memory based Persistence"
                        , (m, o) =>
                        {
                            sSettings.PersistenceType = PersistenceOptions.Memory;
                        }
                        , enabled: (m, o) => true
                        , selected: (m, o) => sSettings.PersistenceType == PersistenceOptions.Memory
                    )
                    //, new ConsoleOption("Client RedisCache Persistence extension enabled"
                    //    , (m, o) =>
                    //    {
                    //        sClient.RepositoryRedisCacheEnabled = !sClient.RepositoryRedisCacheEnabled;
                    //    }
                    //    , enabled: (m, o) => sClient?.Config?.CanResolve(AzureExtensionMethods.KeyRedisCacheConnection) ?? false
                    //    , selected: (m, o) => sClient.RepositoryRedisCacheEnabled
                    //)
                    //, new ConsoleOption("Server RedisCache Persistence extension enabled"
                    //    , (m, o) =>
                    //    {
                    //        sServer.RepositoryRedisCacheEnabled = !sServer.RepositoryRedisCacheEnabled;
                    //    }
                    //    , enabled: (m, o) => sServer?.Config?.CanResolve(AzureExtensionMethods.KeyRedisCacheConnection) ?? false
                    //    , selected: (m, o) => sServer.RepositoryRedisCacheEnabled
                    //)
                    );

                menu.ContextInfoInherit = false;
                menu.AddInfoMessage("Note: Persistence options are disabled if config settings are not present.");

                return menu;

            });
    }
}
