using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Unity.WebApi;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static Dictionary<string, string> sServerSettings = new Dictionary<string, string>();

        static void MicroserviceServerStart()
        {
            try
            {
                sContext.Server.OnRegister += Server_OnRegister;

                sContext.Server.StatusChanged += StatusChanged;

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

        static void MicroserviceServerStop()
        {
            sContext.Server.Stop();
            sContext.Server.StatusChanged -= StatusChanged;
        }

        static string ResolveServerSetting(string key, string value)
        {
            if (sServerSettings.ContainsKey(key))
                return sServerSettings[key];

            return null;
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


        private static void ServerStopRequested(object sender, StopEventArgs e)
        {

        }

        private static void ServerStartRequested(object sender, StartEventArgs e)
        {
            e.ConfigurationOptions.ConcurrentRequestsMax = 4;
            e.ConfigurationOptions.ConcurrentRequestsMin = 1;
            e.ConfigurationOptions.StatusLogFrequency = TimeSpan.FromSeconds(15);

            //e.ConfigurationOptions.OverloadProcessLimit = 2;
            //if (e.ConfigurationOptions.OverloadProcessLimit == 0)
            //    e.ConfigurationOptions.OverloadProcessLimit = 11;        
        }
    }
}
