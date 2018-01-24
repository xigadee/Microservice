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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the container used to hold the Shared Service instance collection.
    /// </summary>
    public class SharedServiceContainer: StatisticsBase<SharedServiceStatistics>, ISharedService
    {
        #region Declarations
        /// <summary>
        /// The internal container.
        /// </summary>
        private ConcurrentDictionary<Type, ServiceHolder> mContainer;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SharedServiceContainer()
        {
            mContainer = new ConcurrentDictionary<Type, ServiceHolder>();
        }
        #endregion

        /// <summary>
        /// This is the shared service statistics.
        /// </summary>
        /// <param name="stats">The statistics</param>
        protected override void StatisticsRecalculate(SharedServiceStatistics stats)
        {
            stats.Services = mContainer.Values.Select((v) => v.StatisticsRecalculated).ToList();
        }

        public bool RegisterService<I>(I instance, string serviceName = null)
            where I : class
        {
            return RegisterServiceInternal<I>(instance, null, serviceName);
        }

        public bool RegisterService<I>(Lazy<I> instance, string serviceName = null)
            where I : class
        {
            return RegisterServiceInternal<I>(default(I), instance, serviceName);
        }

        private bool RegisterServiceInternal<I>(I instance, Lazy<I> lazyInstance, string serviceName = null)
            where I : class
        {
            if (instance == default(I) && lazyInstance == null)
                throw new ArgumentNullException("RegisterService - Cannot be null");

            Type key = typeof(I);

            if (mContainer.ContainsKey(key))
                return false;

            var container = new ServiceHolder<I>(instance, lazyInstance, serviceName);

            mContainer.AddOrUpdate(key, container, (k, o) => container);

            return true;
        }

        public bool RemoveService<I>()
            where I : class
        {
            Type key = typeof(I);
            if (!mContainer.ContainsKey(key))
                return false;

            ServiceHolder value;
            return mContainer.TryRemove(key, out value);
        }

        public I GetService<I>()
            where I : class
        {
            Type key = typeof(I);
            if (!mContainer.ContainsKey(key))
                return default(I);

            ServiceHolder<I> container = (ServiceHolder<I>)mContainer[key];

            return container.Service;
        }

        public bool HasService<I>()
            where I : class
        {
            Type key = typeof(I);

            return mContainer.ContainsKey(key);
        }

    }
}
