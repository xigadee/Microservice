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
        static IRepositoryAsync<Guid, MondayMorningBlues> BuildServer(MicroservicePipeline pipeline)
        {
            IRepositoryAsync<Guid, MondayMorningBlues> persistence = null;

            //if ((sContext.RedisCache & RedisCacheMode.Server) > 0)
            //{
            //    cacheManager = RedisCacheHelper.Default<Guid, MondayMorningBlues>(e.Config.RedisCacheConnection());
            //}

            pipeline
                .ConfigurationSetFromConsoleArgs(sContext.Switches)
                .AddChannelIncoming("internalIn")
                    .CallOut(PersistenceSet)
                    //.AttachResourceProfile(new ResourceProfile("TrackIt"))
                    //.AppendBoundaryLogger(new MemoryBoundaryLogger(), (p,bl) => bLogger = bl)
                    //.AttachAzureServiceBusQueueListener("Myqueue")
                    //.AttachCommand(new PersistenceBlahMemory())
                    //.AttachCommand(new PersistenceInternalService<Guid, Blah>(), assign: (c) => persistence = c, channelResponse: cpipeOut)
                    //.CallOut((c) => cpipeIn = c)
                    .Revert()
                .AddChannelOutgoing("internalOut", internalOnly: true)
                    ////.AppendBoundaryLogger(bLogger)
                    //.CallOut((c) => cpipeOut = c)
                    .Revert();

            return persistence;
        }


        static IRepositoryAsync<Guid, MondayMorningBlues> BuildClient(MicroservicePipeline pipeline)
        {
            PersistenceMessageInitiator<Guid, MondayMorningBlues> persistence = null;


            pipeline
                .ConfigurationSetFromConsoleArgs(sContext.Switches)
                .AddChannelIncoming("internalOut")
                    .AttachPersistenceMessageInitiator(out persistence, "internalIn")
                    .Revert()
                .AddChannelOutgoing("internalIn", internalOnly: true)
                    .Revert();


            return persistence;
        }

        static void PersistenceSet(IPipelineChannelIncoming<MicroservicePipeline> cpipe)
        {
            switch (sContext.PersistenceType)
            {
                case PersistenceOptions.Sql:
                    //sContext.Server.Service.Commands.Register(
                    //    new PersistenceMondayMorningBluesSql(e.Config.SqlConnection()
                    //    , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                    //    { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.Blob:
                    //e.Service.Commands.Register(
                    //    new PersistenceMondayMorningBluesBlob(e.Config.StorageCredentials()
                    //    , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                    //    { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.DocumentDb:
                    //e.Service.Commands.Register(
                    //    new PersistenceMondayMorningBluesDocDb(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
                    //    , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                    //    { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.DocumentDbSdk:
                    //e.Service.Commands.Register(
                    //    new PersistenceMondayMorningBluesDocDbSdk(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
                    //    , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                    //    { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.Memory:
                    //e.Service.Commands.Register(
                    //    new PersistenceMondayMorningBluesMemory(
                    //      MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                    //    { ChannelId = Channels.TestB });
                    break;
                case PersistenceOptions.RedisCache:
                    //e.Service.Commands.Register(
                    //    new PersistenceMondayMorningBluesRedis(e.Config.RedisCacheConnection()
                    //    , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
                    //    { ChannelId = Channels.TestB });
                    break;
            }
        }
        //        static Dictionary<string, string> sServerSettings = new Dictionary<string, string>();

        //        static string ResolveServerSetting(string key, string value)
        //        {
        //            if (sServerSettings.ContainsKey(key))
        //                return sServerSettings[key];

        //            return null;
        //        }

        //        private static void Server_OnRegister(object sender, CommandRegisterEventArgs e)
        //        {
        //            ICacheManager<Guid, MondayMorningBlues> cacheManager = null;

        //            if (sContext.Server.RedisCacheEnabled)
        //            {
        //                cacheManager = RedisCacheHelper.Default<Guid, MondayMorningBlues>(e.Config.RedisCacheConnection());
        //            }

        //            switch (sContext.PersistenceType)
        //            {
        //                case PersistenceOptions.Sql:
        //                    sContext.Server.Service.Commands.Register(
        //                        new PersistenceMondayMorningBluesSql(e.Config.SqlConnection()
        //                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
        //                        { ChannelId = Channels.TestB });
        //                    break;
        //                case PersistenceOptions.Blob:
        //                    e.Service.Commands.Register(
        //                        new PersistenceMondayMorningBluesBlob(e.Config.StorageCredentials()
        //                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
        //                        { ChannelId = Channels.TestB });
        //                    break;
        //                case PersistenceOptions.DocumentDb:
        //                    e.Service.Commands.Register(
        //                        new PersistenceMondayMorningBluesDocDb(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
        //                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
        //                        { ChannelId = Channels.TestB });
        //                    break;
        //                case PersistenceOptions.DocumentDbSdk:
        //                    e.Service.Commands.Register(
        //                        new PersistenceMondayMorningBluesDocDbSdk(e.Config.DocDBConnection(), e.Config.DocDBDatabaseName()
        //                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
        //                        { ChannelId = Channels.TestB });
        //                    break;
        //                case PersistenceOptions.Memory:
        //                    e.Service.Commands.Register(
        //                        new PersistenceMondayMorningBluesMemory(
        //                          MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
        //                        { ChannelId = Channels.TestB });
        //                    break;

        //                case PersistenceOptions.RedisCache:
        //                    e.Service.Commands.Register(
        //                        new PersistenceMondayMorningBluesRedis(e.Config.RedisCacheConnection()
        //                        , MondayMorningBluesHelper.VersionPolicyHelper, cacheManager)
        //                        { ChannelId = Channels.TestB });
        //                    break;
        //            }
        //        }
    }
}
