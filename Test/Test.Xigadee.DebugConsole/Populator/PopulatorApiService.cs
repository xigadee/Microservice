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
using Microsoft.Owin.Hosting;
using Xigadee;

namespace Test.Xigadee
{
    public class PopulatorApiService: IConsolePersistence
    {
        public Microservice Service
        {
            get
            {
                return null;
            }
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public void Populate(Func<string, string, string> resolver = null, bool resolverFirst = false)
        {

        }

        public Uri ApiUri { get; set; }

        /// <summary>
        /// This is the Api instancer service.
        /// </summary>
        public IDisposable ApiServer { get; set; }

        public ServiceStatus Status { get; private set; } = ServiceStatus.Created;

        private void OnStatusChanged(ServiceStatus status, string message = null)
        {

            var oldStatus = Status;
            Status = status;

            StatusChanged?.Invoke(this, new StatusChangedEventArgs() { StatusNew = status, StatusOld = oldStatus, Message = message });
        }

        public void Start()
        {
            try
            {
                StartOptions options = new StartOptions();
                options.Urls.Add(ApiUri.ToString());
                OnStatusChanged(ServiceStatus.Starting, $" @ {ApiUri.ToString()}");

                Persistence = new ApiProviderAsyncV2<Guid, MondayMorningBlues>(new Uri($"{ApiUri.AbsoluteUri}v1"));
                ApiServer = WebApp.Start<Test.Xigadee.Api.Server.Startup>(options);

                OnStatusChanged(ServiceStatus.Running);

            }
            catch (Exception ex)
            {
                OnStatusChanged(ServiceStatus.Stopped, $" Error: {ex.Message}");

                ApiServer = null;
            }
        }

        public void Stop()
        {
            OnStatusChanged(ServiceStatus.Stopping);

            Persistence = null;
            ApiServer.Dispose();
            ApiServer = null;

            OnStatusChanged(ServiceStatus.Stopped);
        }

        public IRepositoryAsync<Guid, MondayMorningBlues> Persistence
        {
            get; private set;
        }

        public string Name
        {
            get
            {
                return "ApiService";
            }
        }
    }
}
