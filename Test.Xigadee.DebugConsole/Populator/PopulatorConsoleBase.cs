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
using Xigadee;

namespace Test.Xigadee
{
    internal abstract class PopulatorConsoleBase<M>: PopulatorBase<M, ConfigConsole>, IPopulatorConsole where M : Microservice, new()
    {
        public ServiceStatus Status { get { return Service?.Status ?? ServiceStatus.Stopped;} }

        public event EventHandler<CommandRegisterEventArgs> OnRegister;

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public readonly ResourceProfile mResourceDocDb = new ResourceProfile("DocDB");

        public readonly ResourceProfile mResourceBlob = new ResourceProfile("Blob");

        public IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get; protected set; }

        public virtual string Name
        {
            get
            {
                return Service.Name;
            }
        }

        protected override void RegisterCommunication()
        {
            Service.RegisterListener(new AzureSBTopicListener(
                  Channels.MasterJob
                , Config.ServiceBusConnection()
                , Channels.MasterJob
                , ListenerPartitionConfig.Init(2)));

            Service.RegisterSender(new AzureSBTopicSender(
                  Channels.MasterJob
                , Config.ServiceBusConnection()
                , Channels.MasterJob
                , SenderPartitionConfig.Init(2)));
        }

        protected override void RegisterCommands()
        {
            if (OnRegister != null)
                OnRegister(this, new CommandRegisterEventArgs(Service, Config));

            //Service.RegisterCommand(new TestMasterJob(Channels.MasterJob));
            //Service.RegisterCommand(new TestMasterJob2(Channels.MasterJob));

            //Service.RegisterCommand(new DelayedProcessingJob());
        }

        protected override void RegisterEventSources()
        {
            Service.RegisterEventSource(new AzureStorageEventSource(
                  Config.StorageCredentials()
                , Service.Name
                , resourceProfile: mResourceBlob));

        }

        protected override void RegisterLogging()
        {
            base.RegisterLogging();

            Service.RegisterLogger(new TraceEventLogger());

            Service.RegisterLogger(new AzureStorageLogger(
                  Config.StorageCredentials()
                , Service.Name
                , resourceProfile: mResourceBlob));

        }


        protected override void ServiceStatusChanged(object sender, StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }
    }


}
