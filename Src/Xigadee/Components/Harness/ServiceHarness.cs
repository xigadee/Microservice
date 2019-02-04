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

namespace Xigadee
{
    /// <summary>
    /// This class is used to construct a test harness around a service to allow for unit testing.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public abstract class ServiceHarness<S> : ServiceHarness<S, ServiceHarnessDependencies>
            where S : class, IService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencies"></param>
        public ServiceHarness(ServiceHarnessDependencies dependencies = null):base(dependencies)
        {

        }
    }

    /// <summary>
    /// This class is used to construct a test harness around a service to allow for unit testing.
    /// </summary>
    /// <typeparam name="S">The service type.</typeparam>
    /// <typeparam name="D">The dependency type.</typeparam>
    public abstract class ServiceHarness<S, D> : IDisposable
        where S : class, IService
        where D : ServiceHarnessDependencies, new()
    {
        /// <summary>
        /// The optional service creator function.
        /// </summary>
        protected Func<S> mServiceCreator = null;

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="dependencies">The optional dependency parameter.</param>
        /// <param name="serviceCreator">This is the optional function that can be passed to create the service.</param>
        public ServiceHarness(D dependencies = null, Func<S> serviceCreator = null)
        {
            mServiceCreator = serviceCreator;
            Dependencies = dependencies?? new D();

            Service = Create();
            Service.StatusChanged += Service_StatusChanged;
            Configure(Service);
        }

        /// <summary>
        /// This internal service.
        /// </summary>
        public S Service { get; }

        /// <summary>
        /// This is the dependencies class. This class can be shared with another harness.
        /// </summary>
        public D Dependencies { get; }

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
        /// <summary>
        /// This base method should be used to create the service.
        /// </summary>
        /// <returns></returns>
        protected virtual S Create()
        {
            return (mServiceCreator ?? ServiceHarnessHelper.DefaultCreator<S>())();
        }
        /// <summary>
        /// This method should be used to provide additional configuration before starting the services.
        /// </summary>
        /// <param name="service">The service.</param>
        protected virtual void Configure(S service)
        {
            Dependencies.Configure(service);
        }

        #region Service Status
        /// <summary>
        /// Gets the current command status.
        /// </summary>
        public ServiceStatus? CurrentStatus { get; protected set; }
        /// <summary>
        /// Gets the status history.
        /// </summary>
        public Queue<StatusChangedEventArgs> StatusHistory { get; } = new Queue<StatusChangedEventArgs>();
        /// <summary>
        /// This method can be used to monitor status changes.
        /// </summary>
        /// <param name="sender">The sender service.</param>
        /// <param name="e">The change event.</param>
        protected virtual void Service_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            CurrentStatus = e.StatusNew;
            StatusHistory.Enqueue(e);
        } 
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }


}
