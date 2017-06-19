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
        static void ServerInit(MicroservicePersistenceWrapper<Guid, MondayMorningBlues> wrapper)
        {
            wrapper.Pipeline
                //.ConfigurationClear()
                .ConfigurationSetFromConsoleArgs(sContext.Switches);
        }

        static void ServerConfig(MicroservicePersistenceWrapper<Guid, MondayMorningBlues> wrapper)
        {
            PersistenceClient<Guid, MondayMorningBlues> persistence = null;

            //if ((sContext.RedisCache & RedisCacheMode.Server) > 0)
            //{
            //    cacheManager = RedisCacheHelper.Default<Guid, MondayMorningBlues>(e.Config.RedisCacheConnection());
            //}

            wrapper.Pipeline
                .AddDebugMemoryDataCollector((c) => wrapper.Collector = c)
                .AddChannelIncoming("internalIn", boundaryLoggingEnabled:true)
                    .CallOut(PersistenceCommandSet)
                    //.AttachResourceProfile(new ResourceProfile("TrackIt"))
                    //.AttachAzureServiceBusQueueListener("Myqueue")
                    //.AttachCommand(new PersistenceBlahMemory())
                    .AttachPersistenceClient(out persistence)
                    .Revert()
                .AddChannelOutgoing("internalOut", internalOnly: true)
                    ////.AppendBoundaryLogger(bLogger)
                    //.CallOut((c) => cpipeOut = c)
                    .Revert();

            wrapper.PersistenceServer = persistence;
        }

        static void ClientConfig(MicroservicePersistenceWrapper<Guid, MondayMorningBlues> wrapper)
        {
            PersistenceClient<Guid, MondayMorningBlues> persistence = null;

            wrapper.Pipeline
                .ConfigurationSetFromConsoleArgs(sContext.Switches)
                .AddDebugMemoryDataCollector((c) => wrapper.Collector = c)
                .AddChannelIncoming("internalOut")
                    .AttachPersistenceClient(out persistence, "internalIn")
                    .Revert()
                .AddChannelOutgoing("internalIn", internalOnly: true)
                    .Revert();

            wrapper.PersistenceClient = persistence;
        }

        static void PersistenceCommandSet(IPipelineChannelIncoming<MicroservicePipeline> cpipe)
        {
            var config = cpipe.ToConfiguration();

            switch (sContext.PersistenceType)
            {        
                case PersistenceOptions.Sql:
                    cpipe.AttachCommand(new PersistenceMondayMorningBluesSql(config.SqlConnection(), MondayMorningBluesHelper.VersionPolicyHelper));
                    break;
                case PersistenceOptions.Blob:
                    cpipe.AttachPersistenceManagerAzureBlobStorage(
                        (MondayMorningBlues k) => k.Id
                        , (s) => new Guid(s)
                        , keySerializer: (g) => g.ToString("N").ToUpperInvariant()
                        , versionPolicy: MondayMorningBluesHelper.VersionPolicyHelper
                        , referenceMaker: MondayMorningBluesHelper.ToReferences
                        );
                    break;
                case PersistenceOptions.DocumentDb:
                    cpipe.AttachPersistenceManagerDocumentDb(
                        (MondayMorningBlues k) => k.Id
                        , (s) => new Guid(s)
                        , versionPolicy: MondayMorningBluesHelper.VersionPolicyHelper
                        , referenceMaker: MondayMorningBluesHelper.ToReferences
                        ); break;
                case PersistenceOptions.DocumentDbSdk:
                    cpipe.AttachPersistenceManagerDocumentDbSdk(
                        (MondayMorningBlues k) => k.Id
                        , (s) => new Guid(s)
                        , versionPolicy: MondayMorningBluesHelper.VersionPolicyHelper
                        , referenceMaker: MondayMorningBluesHelper.ToReferences
                        );
                    break;
                case PersistenceOptions.Memory:
                    cpipe.AttachPersistenceManagerHandlerMemory(
                          (MondayMorningBlues k) => k.Id
                        , (s) => new Guid(s)
                        , versionPolicy: MondayMorningBluesHelper.VersionPolicyHelper
                        , referenceMaker: MondayMorningBluesHelper.ToReferences);
                    break;
                case PersistenceOptions.RedisCache:
                    cpipe.AttachPersistenceManagerRedisCache(
                          (MondayMorningBlues k) => k.Id
                        , (s) => new Guid(s)
                        , versionPolicy: MondayMorningBluesHelper.VersionPolicyHelper
                        , referenceMaker: MondayMorningBluesHelper.ToReferences);
                    break;
            }
        }
    }
}
