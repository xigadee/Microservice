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
            sContext.Client.Service.StatusChanged += ClientStatusChanged;

            sContext.Client.Populate(ResolveClientSetting, true);
            sContext.Client.Start();
        }

        static void MicroserviceServerStart()
        {
            try
            {
                sContext.Server.OnRegister += Server_OnRegister;

                sContext.Server.Service.StatusChanged += ServerStatusChanged;

                sContext.Server.Service.StartRequested += ServerStartRequested;
                sContext.Server.Service.StopRequested += ServerStopRequested;

                sContext.Server.Populate(ResolveServerSetting, true);

                sContext.Server.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

            if (sContext.ServerCacheEnabled)
            {
                cacheManager = RedisCacheHelper.Default<Guid, MondayMorningBlues>(e.Config.RedisCacheConnection());
            }

            switch (sContext.PersistenceType)
            {
                case PersistenceOptions.Sql:
                    sContext.Server.Service.RegisterCommand(
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
                        new PersistenceMondayMorningBluesDocDbSdk(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
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
