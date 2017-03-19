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
                //sContext.Server.

                sContext.Server.Service.Events.StartRequested += ServerStartRequested;
                sContext.Server.Service.Events.StopRequested += ServerStopRequested;
                sContext.Server.Service.Events.ComponentStatusChange += ServiceComponentStatusChange;
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
            sContext.Server.Service.Events.ComponentStatusChange -= ServiceComponentStatusChange;
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
                    sContext.Server.Service.Commands.Register(
                        new PersistenceMondayMorningBluesSql(e.Config.SqlConnection()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.Blob:
                    e.Service.Commands.Register(
                        new PersistenceMondayMorningBluesBlob(e.Config.StorageCredentials()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.DocumentDb:
                    e.Service.Commands.Register(
                        new PersistenceMondayMorningBluesDocDb(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.DocumentDbSdk:
                    e.Service.Commands.Register(
                        new PersistenceMondayMorningBluesDocDbSdk(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.Memory:
                    e.Service.Commands.Register(
                        new PersistenceMondayMorningBluesMemory(
                          MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;

                case PersistenceOptions.RedisCache:
                    e.Service.Commands.Register(
                        new PersistenceMondayMorningBluesRedis(e.Config.RedisCacheConnection()
                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                        { ChannelId = Channels.TestB });
                    break;
            }
        }


        private static void ServerStopRequested(object sender, StopEventArgs e)
        {

        }

        private static void ServiceComponentStatusChange(object sender, MicroserviceStatusEventArgs e)
        {
            var serv = sender as Microservice;

            sMenuMain.Value.AddInfoMessage($"{serv.Id.Name} {e.Debug()}", true);

        }

        private static void ServerStartRequested(object sender, StartEventArgs e)
        {
            //e.ConfigurationOptions.ConcurrentRequestsMax = 4;
            //e.ConfigurationOptions.ConcurrentRequestsMin = 1;
            //e.ConfigurationOptions.StatusLogFrequency = TimeSpan.FromSeconds(15);

            //e.ConfigurationOptions.OverloadProcessLimit = 2;
            //if (e.ConfigurationOptions.OverloadProcessLimit == 0)
            //    e.ConfigurationOptions.OverloadProcessLimit = 11;        
        }
    }
}
