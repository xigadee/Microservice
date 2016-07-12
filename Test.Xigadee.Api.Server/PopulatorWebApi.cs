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
        public static string ResponseBff = "response-bff";
        public static string RequestCore = "request-core";
        public static string MasterJob = "masterjob";
        public static string Interservice = "interservice";
    }

    public class PopulatorWebApi: PopulatorBase<MicroserviceWebApi, ConfigWebApi>
    {

        public readonly ResourceProfile mResourceBlobStorage = new ResourceProfile("Blob");

        public PopulatorWebApi()
        {
            Unity = new UnityContainer();
        }

        public IUnityContainer Unity { get; }

        private void RegisterCommand<I, P>() where P : I, ICommand, new()
        {
            RegisterCommand<I, P>(new P());
        }

        protected override void ConfigInitialise(Func<string, string, string> resolver, bool resolverFirst)
        {
            if (resolver == null)
            {
                resolver = (k, v) =>
                {
                    string value = null;

                    try
                    {
                        if (k != null)
                        {
                            value = CloudConfigurationManager.GetSetting(k);

                            if (value == null)
                                value = ConfigurationManager.AppSettings[k];
                        }
                    }
                    catch (Exception)
                    {
                        // Unable to retrieve from azure
                        return null;
                    }

                    return value;
                };
            }

            base.ConfigInitialise(resolver, true);
        }


        private void RegisterCommand<I, P>(P instance) where P : I, ICommand
        {
            try
            {
                Service.RegisterCommand(instance);
                Unity.RegisterInstance<I>(instance);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            ////Batch Entities
            //RegisterCommand<IRepositoryAsync<Guid, Batch>, ProviderBatchAsync>(new ProviderBatchAsync());
            //RegisterCommand<IRepositoryAsync<Guid, BatchConfig>, ProviderBatchConfigAsync>(new ProviderBatchConfigAsync());
            ////Customer Entities
            //RegisterCommand<IRepositoryAsync<Guid, Customer>, ProviderCustomerAsync>(new ProviderCustomerAsync());
            //RegisterCommand<IRepositoryAsync<Guid, CustomerEmailChange>, ProviderCustomerEmailChangeMappingAsync>(new ProviderCustomerEmailChangeMappingAsync());
            //RegisterCommand<IRepositoryAsync<Guid, CustomerReset>, ProviderCustomerResetAsync>(new ProviderCustomerResetAsync());
            //RegisterCommand<IRepositoryAsync<Guid, CustomerSecurity>, ProviderCustomerSecurityAsync>(new ProviderCustomerSecurityAsync());
            //RegisterCommand<IRepositoryAsync<Guid, CustomerStatus>, ProviderCustomerStatusAsync>(new ProviderCustomerStatusAsync());
            ////Role Entities
            //RegisterCommand<IRepositoryAsync<string, Role>, ProviderRoleAsync>(new ProviderRoleAsync());
            //RegisterCommand<IRepositoryAsync<string, UserRole>, ProviderUserRoleAsync>(new ProviderUserRoleAsync());
            ////Workflow Entities
            //RegisterCommand<IRepositoryAsync<Guid, Workflow>, ProviderWorkflowAsync>(new ProviderWorkflowAsync());
            //RegisterCommand<IRepositoryAsync<Guid, WorkflowContext>, ProviderWorkflowContextAsync>(new ProviderWorkflowContextAsync());
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

            Service.RegisterListener(new AzureSBTopicListener(
                  CoreChannels.MasterJob
                , Config.ServiceBusConnection()
                , CoreChannels.MasterJob
                , ListenerPartitionConfig.Init(2)));

            Service.RegisterSender(new AzureSBTopicSender(
                  CoreChannels.MasterJob
                , Config.ServiceBusConnection()
                , CoreChannels.MasterJob
                , SenderPartitionConfig.Init(2)));

            Service.RegisterSender(new AzureSBQueueSender(
                  CoreChannels.RequestCore
                , Config.ServiceBusConnection()
                , CoreChannels.RequestCore
                , SenderPartitionConfig.Init(0, 1)));

            Service.RegisterListener(new AzureSBTopicListener(
                  CoreChannels.ResponseBff
                , Config.ServiceBusConnection()
                , CoreChannels.ResponseBff
                , new[] { new ListenerPartitionConfig(1, 2, false) }
                , listenOnOriginatorId: true
                ));
        }

        protected virtual IEnumerable<ListenerPartitionConfig> PrimaryChannelConfig(decimal weighting = 1.2m)
        {
            var p0 = new ListenerPartitionConfig(0
                , payloadMaxProcessingTime: TimeSpan.FromMinutes(4.5)
                , fabricMaxMessageLock: TimeSpan.FromMinutes(4.8)
                );
            p0.SupportsRateLimiting = true;

            yield return p0;

            var p1 = new ListenerPartitionConfig(1
                , payloadMaxProcessingTime: TimeSpan.FromMinutes(4.5)
                , fabricMaxMessageLock: TimeSpan.FromMinutes(4.8)
                );
            p1.SupportsRateLimiting = false;
            p1.PriorityWeighting = weighting;

            yield return p1;
        }
    }
}