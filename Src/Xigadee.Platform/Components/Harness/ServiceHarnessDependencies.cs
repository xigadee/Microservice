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
    /// This is the service harness dependencies.
    /// </summary>
    public class ServiceHarnessDependencies
    {
        /// <summary>
        /// This is the default constructor that connects the services.
        /// </summary>
        public ServiceHarnessDependencies()
        {
            ((ServiceHarnessScheduler)Scheduler).Collector = Collector;
            ((ServiceHarnessScheduler)Scheduler).Start();

            ((ServiceHarnessResourceContainer)ResourceTracker).Collector = Collector;
            ((ServiceHarnessResourceContainer)ResourceTracker).SharedServices = SharedService;
            ((ServiceHarnessResourceContainer)ResourceTracker).Start();
        }

        /// <summary>
        /// This method sets the object required services.
        /// </summary>
        /// <param name="service">The service.</param>
        public virtual void Configure(object service)
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
        /// <summary>
        /// This is the resource tracker for the harness.
        /// </summary>
        public virtual IResourceTracker ResourceTracker => new ServiceHarnessResourceContainer();
    }
}
