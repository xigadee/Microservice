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
        /// This is the default constructor.
        /// </summary>
        /// <param name="dependencies">The optional dependency parameter.</param>
        public ServiceHarness(D dependencies = null)
        {
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
        protected abstract S Create();
        /// <summary>
        /// This method should be used to provide additional configuration before starting the services.
        /// </summary>
        /// <param name="service">The service.</param>
        protected virtual void Configure(S service)
        {
            Dependencies.Configure(service);
        }
        /// <summary>
        /// This method can be used to monitor status changes.
        /// </summary>
        /// <param name="sender">The sender service.</param>
        /// <param name="e">The change event.</param>
        protected virtual void Service_StatusChanged(object sender, StatusChangedEventArgs e)
        {
        }

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
