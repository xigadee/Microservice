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
                .ConfigurationSetFromConsoleArgs(sSettings.Switches)
                //.Condition((c) => c.AesTransportEncryptionUseCompression
                ;
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
                    .CallOut(ServerPersistenceCommandSet)
                    .CallOut(ServerCommunicationSet)
                    //.AttachResourceProfile(new ResourceProfile("TrackIt"))
                    //.AttachAzureServiceBusQueueListener("Myqueue")
                    .AttachPersistenceClient(out persistence)
                    .Revert()
                .AddChannelOutgoing("internalOut", internalOnly: true)
                    ////.AppendBoundaryLogger(bLogger)
                    //.CallOut((c) => cpipeOut = c)
                    .Revert();

            wrapper.Persistence = persistence;
        }

        /// <summary>
        /// This method adds the appropriate persistence command.
        /// </summary>
        /// <param name="cpipe">The pipeline.</param>
        static void ServerCommunicationSet(IPipelineChannelIncoming<MicroservicePipeline> cpipe)
        {
            var config = cpipe.ToConfiguration();

            switch (sSettings.CommunicationType)
            {
                case CommunicationOptions.Local:
                    break;
                case CommunicationOptions.Tcp:
                    cpipe.AttachTcpTlsListener();
                    break;
                case CommunicationOptions.Tls:
                    break;
                //case CommunicationOptions.AzureServiceBus:
                //    cpipe.AttachAzureServiceBusTopicListener();
                //    break;
                case CommunicationOptions.AzureBlobQueue:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        /// <summary>
        /// This method adds the appropriate persistence command.
        /// </summary>
        /// <param name="cpipe">The pipeline.</param>
        static void ServerPersistenceCommandSet(IPipelineChannelIncoming<MicroservicePipeline> cpipe)
        {
            var config = cpipe.ToConfiguration();

            switch (sSettings.PersistenceType)
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
