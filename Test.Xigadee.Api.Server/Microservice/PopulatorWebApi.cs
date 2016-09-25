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

#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.Azure;
using Microsoft.Practices.Unity;
using Xigadee;

#endregion
namespace Test.Xigadee.Api.Server
{
    public static class CoreChannels
    {
        public const string Internal = "internalchannel";
        public const string ResponseBff = "response-bff";
        public const string RequestCore = "request-core";
        public const string MasterJob = "masterjob";
        public const string Interservice = "interservice";
    }

    public class PopulatorWebApi: PopulatorWebApiUnity<TestMicroserviceApi, TestConfigApi>
    {

        public readonly ResourceProfile mResourceBlobStorage = new ResourceProfile("Blob");

        public PopulatorWebApi(bool local = false)
        {
        }

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            //Batch Entities
            RegisterCommand<IRepositoryAsync<Guid, Blah>, ProviderBlahAsyncLocal>(new ProviderBlahAsyncLocal());
            RegisterCommand<IRepositoryAsync<Guid, MondayMorningBlues>, ProviderMondayMorningBluesAsyncLocal>(new ProviderMondayMorningBluesAsyncLocal());
            RegisterCommand<IRepositoryAsync<ComplexKey, ComplexEntity>, ProviderComplexEntityAsyncLocal>(new ProviderComplexEntityAsyncLocal());

            Service.RegisterCommand(new PersistenceBlahMemory()
            { ChannelId = CoreChannels.Internal, StartupPriority = 99 });

            Service.RegisterCommand(new PersistenceComplexEntityMemory()
            { ChannelId = CoreChannels.Internal, StartupPriority = 99 });

            Service.RegisterCommand(new PersistenceMondayMorningBluesMemory()
            { ChannelId = CoreChannels.Internal, StartupPriority = 99 });

        }

        protected override void RegisterEventSources()
        {
            base.RegisterEventSources();

            Service.RegisterEventSource(new AzureStorageEventSource(
                Config.LogStorageCredentials()
                , Service.Name
                , resourceProfile: mResourceBlobStorage));
        }

        protected override void RegisterLogging()
        {
            base.RegisterLogging();

            Service.RegisterLogger(new AzureStorageLogger(
                Config.LogStorageCredentials()
                , Service.Name
                , resourceProfile: mResourceBlobStorage));
        }

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            //Service.RegisterListener(new AzureSBTopicListener(
            //      CoreChannels.MasterJob
            //    , Config.ServiceBusConnection()
            //    , CoreChannels.MasterJob
            //    , ListenerPartitionConfig.Init(2)));

            //Service.RegisterSender(new AzureSBTopicSender(
            //      CoreChannels.MasterJob
            //    , Config.ServiceBusConnection()
            //    , CoreChannels.MasterJob
            //    , SenderPartitionConfig.Init(2)));

            //Service.RegisterSender(new AzureSBQueueSender(
            //      CoreChannels.RequestCore
            //    , Config.ServiceBusConnection()
            //    , CoreChannels.RequestCore
            //    , SenderPartitionConfig.Init(0, 1)));

            //Service.RegisterListener(new AzureSBTopicListener(
            //      CoreChannels.ResponseBff
            //    , Config.ServiceBusConnection()
            //    , CoreChannels.ResponseBff
            //    , new[] { new ListenerPartitionConfig(1, 2, false) }
            //    , listenOnOriginatorId: true
            //    ));
        }

    }
}