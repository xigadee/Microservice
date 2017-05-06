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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This class is used to construct a test harness around a service to allow for unit testing.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public abstract class ServiceHarness<S> : IDisposable
        where S : class, IService
    {
        public S Service { get; }

        public ServiceHarness()
        {
            ((ServiceHarnessScheduler)Scheduler).Collector = Collector;
            ((ServiceHarnessScheduler)Scheduler).Start();

            ((ServiceHarnessResourceContainer)ResourceTracker).Collector = Collector;
            ((ServiceHarnessResourceContainer)ResourceTracker).SharedServices = SharedService;
            ((ServiceHarnessResourceContainer)ResourceTracker).Start();

            Service = Create();
            Service.StatusChanged += Service_StatusChanged;
            Configure(Service);
        }

        /// <summary>
        /// This method starts the service. Override to add additional steps.
        /// </summary>
        public virtual void Start()
        {
            Service.Start();
        }
        /// <summary>
        /// This method stops the service.
        /// </summary>
        public virtual void Stop()
        {
            Service.Stop();
        }

        protected abstract S Create();

        protected virtual void Configure(S service)
        {
            if (service is IRequireDataCollector)
                ((IRequireDataCollector)service).Collector = Collector;

            if (service is IRequireScheduler)
                ((IRequireScheduler)service).Scheduler = Scheduler;

            if (service is IRequireSharedServices)
                ((IRequireSharedServices)service).SharedServices = SharedService;

            if (service is IRequireServiceOriginator)
                ((IRequireServiceOriginator)service).OriginatorId = OriginatorId;

            if (service is IRequirePayloadSerializer)
                ((IRequirePayloadSerializer)service).PayloadSerializer = PayloadSerializer;

        }

        protected virtual void Service_StatusChanged(object sender, StatusChangedEventArgs e)
        {
        }

        /// <summary>
        /// This is the stub Payload serializer.
        /// </summary>
        public virtual IPayloadSerializationContainer PayloadSerializer => new ServiceHarnessSerializationContainer();
        /// <summary>
        /// This is the example originator id.
        /// </summary>
        public virtual MicroserviceId OriginatorId => new MicroserviceId(GetType().Name, Guid.NewGuid().ToString("N").ToUpperInvariant());
        /// <summary>
        /// This is the stub data collector.
        /// </summary>
        public virtual IDataCollection Collector => new ServiceHarnessDataCollection();
        /// <summary>
        /// This is the stub scheduler
        /// </summary>
        public virtual IScheduler Scheduler => new ServiceHarnessScheduler();
        /// <summary>
        /// This is the stub shared service container.
        /// </summary>
        public virtual ISharedService SharedService => new ServiceHarnessSharedService();

        public virtual IResourceTracker ResourceTracker => new ServiceHarnessResourceContainer();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Service.StatusChanged -= Service_StatusChanged;
                }

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }


}
