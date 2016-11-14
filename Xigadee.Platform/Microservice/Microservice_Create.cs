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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //TaskManager
    public partial class Microservice
    {
        #region Create<C> ...
        /// <summary>
        /// This method is used to build a pipeline used to configure the Microservice
        /// </summary>
        /// <typeparam name="C">The config type.</typeparam>
        /// <param name="assign">This is an action that can be used to make changes to the underlying microservice.</param>
        /// <param name="configAssign">This action can be used to modify the configuration.</param>
        /// <param name="serviceName">The friendly service name</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="policy">A set of policy collections that override the default settings.</param>
        /// <returns>Returns a pipeline that can be used to configure a microservice.</returns>
        public static MicroservicePipeline Create<C>(
              Action<Microservice> assign = null
            , Action<C> configAssign = null
            , string serviceName = null
            , string serviceId = null
            , IEnumerable<PolicyBase> policy = null
            )
            where C : ConfigBase, new()
        {
            var service = new Microservice(serviceName, serviceId, policy);

            C config = new C();
            //if (resolver != null)
            //{
            //    config.Resolver = resolver;
            //    config.ResolverFirst = resolverFirst;
            //}

            assign?.Invoke(service);
            configAssign?.Invoke(config);

            return new MicroservicePipeline(service, config);
        }
        #endregion
        #region Create ...
        /// <summary>
        /// This method is used to build a pipeline used to configure the Microservice
        /// </summary>
        /// <param name="assign">This is an action that can be used to make changes to the underlying microservice.</param>
        /// <param name="configAction">This action can be used to modify the configuration.</param>
        /// <param name="serviceName">The friendly service name</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="policy">A set of policy collections that override the default settings.</param>
        /// <returns>Returns a pipeline that can be used to configure a microservice.</returns>
        public static MicroservicePipeline Create(
              Action<Microservice> assign = null
            , Action<ConfigBase> configAction = null
            , string serviceName = null
            , string serviceId = null
            , IEnumerable<PolicyBase> policy = null)
        {
            return Create<ConfigBase>(
                  assign: assign
                , configAssign: configAction
                , serviceName: serviceName
                , serviceId: serviceId
                , policy: policy
                );
        } 
        #endregion
    }
}
