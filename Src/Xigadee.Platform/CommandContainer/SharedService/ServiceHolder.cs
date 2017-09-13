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

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold a service by the Shared Service Container.
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <seealso cref="Xigadee.ServiceHolder" />
    public class ServiceHolder<I> : ServiceHolder
        where I: class
    {
        private I mInstance;

        private Lazy<I> mInstanceCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHolder{I}"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="lazyInstance">The lazy service reference.</param>
        /// <param name="serviceName">Name of the service.</param>
        public ServiceHolder(I instance, Lazy<I> lazyInstance, string serviceName = null)
        {
            mInstance = instance;
            mInstanceCreator = lazyInstance;
            StatisticsInternal.Name = serviceName ?? typeof(I).Name;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        public I Service 
        { 
            get 
            {
                //Increment the access statistics.
                StatisticsInternal.Increment();

                if (mInstance == default(I))
                {
                    mInstance = mInstanceCreator.Value;
                }

                return mInstance;
            } 
        }
    }

    /// <summary>
    /// This class holds the specific reference for a Shared Service.
    /// </summary>
    public class ServiceHolder: StatisticsBase<ServiceHolderStatistics>
    {
        /// <summary>
        /// Statistics to recalculate.
        /// </summary>
        /// <param name="stats">The stats.</param>
        protected override void StatisticsRecalculate(ServiceHolderStatistics stats)
        {
        }
    }
}
