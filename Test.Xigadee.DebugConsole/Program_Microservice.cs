using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void MicroserviceClientStart()
        {
            sServerContext.Client.Service.StatusChanged += ClientStatusChanged;

            sServerContext.Client.Populate(ResolveClientSetting, true);
            sServerContext.Client.Start();
        }

        static void MicroserviceServerStart()
        {
            sServerContext.Server.OnRegister += Server_OnRegister;

            sServerContext.Server.Service.StatusChanged += ServerStatusChanged;

            sServerContext.Server.Service.StartRequested += ServerStartRequested;
            sServerContext.Server.Service.StopRequested += ServerStopRequested;

            sServerContext.Server.Populate(ResolveServerSetting, true);
            sServerContext.Server.Start();
        }

        static string ResolveServerSetting(string key, string value)
        {
            if (sServerSettings.ContainsKey(key))
                return sServerSettings[key];

            return null;
        }

        static string ResolveClientSetting(string key, string value)
        {
            if (sClientSettings.ContainsKey(key))
                return sServerSettings[key];

            return null;
        }

        static void MicroserviceLoadSettings()
        {

        }


        private static void Server_OnRegister(object sender, CommandRegisterEventArgs e)
        {
            ICacheManager<Guid, MondayMorningBlues> cacheManager = null;

            if (sServerContext.ServerCacheEnabled)
            {
                cacheManager = RedisCacheHelper.Default<Guid, MondayMorningBlues>(e.Config.RedisCacheConnection());
            }

            switch (sServerContext.PersistenceType)
            {
                case PersistenceOptions.Sql:
                    sServerContext.Server.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesSql(e.Config.SqlConnection()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.Blob:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesBlob(e.Config.StorageCredentials()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.DocumentDb:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesDocDb(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.DocumentDbSdk:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesDocDb(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.Memory:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesMemory(
                          MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;

                case PersistenceOptions.RedisCache:
                    e.Service.RegisterCommand(
                        new PersistenceMondayMorningBluesRedis(e.Config.RedisCacheConnection()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
            }
        }
    }
}
