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
        public static string Internal = "internalchannel";
        public static string ResponseBff = "response-bff";
        public static string RequestCore = "request-core";
        public static string MasterJob = "masterjob";
        public static string Interservice = "interservice";
    }

    public class PopulatorWebApi: PopulatorWebApiUnity<MicroserviceWebApi, ConfigWebApi>
    {

        public readonly ResourceProfile mResourceBlobStorage = new ResourceProfile("Blob");

        public PopulatorWebApi()
        {
        }


        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            //Batch Entities
            RegisterCommand<IRepositoryAsync<Guid, Blah>, ProviderBlahAsyncLocal>(new ProviderBlahAsyncLocal());
            RegisterCommand<IRepositoryAsync<Guid, MondayMorningBlues>, ProviderMondayMorningBluesAsyncLocal>(new ProviderMondayMorningBluesAsyncLocal());
            RegisterCommand<IRepositoryAsync<ComplexKey, ComplexEntity>, ProviderComplexEntityAsyncLocal>(new ProviderComplexEntityAsyncLocal());

            Service.RegisterCommand(new PersistenceBlahMemory(versionPolicy: new VersionPolicy<Blah>(entityVersionAsString: (e) => e.VersionId.ToString("N"), entityVersionUpdate: (e) => e.VersionId = Guid.NewGuid()))
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

        //protected virtual IEnumerable<ListenerPartitionConfig> PrimaryChannelConfig(decimal weighting = 1.2m)
        //{
        //    var p0 = new ListenerPartitionConfig(0
        //        , payloadMaxProcessingTime: TimeSpan.FromMinutes(4.5)
        //        , fabricMaxMessageLock: TimeSpan.FromMinutes(4.8)
        //        );
        //    p0.SupportsRateLimiting = true;

        //    yield return p0;

        //    var p1 = new ListenerPartitionConfig(1
        //        , payloadMaxProcessingTime: TimeSpan.FromMinutes(4.5)
        //        , fabricMaxMessageLock: TimeSpan.FromMinutes(4.8)
        //        );
        //    p1.SupportsRateLimiting = false;
        //    p1.PriorityWeighting = weighting;

        //    yield return p1;
        //}
    }
}