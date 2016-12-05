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

namespace Xigadee
{
    /// <summary>
    /// The Microservice pipeline is used by extension methods to create a simple channel based service configuration.
    /// </summary>
    public class MicroservicePipeline: IPipeline
    {
        /// <summary>
        /// This is the default constructor for the pipeline.
        /// </summary>
        /// <param name="name">The Microservice name.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="policy">The policy settings collection.</param>
        /// <param name="properties">Any additional property key/value pairs.</param>
        /// <param name="config">The environment config object</param>
        /// <param name="assign">The action can be used to assign items to the microservice.</param>
        /// <param name="configAssign">This action can be used to adjust the config settings.</param>
        /// <param name="addDefaultJsonPayloadSerializer">This property specifies that the default Json 
        /// payload serializer should be added to the Microservice, set this to false to disable this.</param>
        public MicroservicePipeline(string name = null
            , string serviceId = null
            , IEnumerable<PolicyBase> policy = null
            , IEnumerable<Tuple<string, string>> properties = null
            , IEnvironmentConfiguration config = null
            , Action<IMicroservice> assign = null
            , Action<IEnvironmentConfiguration> configAssign = null
            , bool addDefaultJsonPayloadSerializer = true
            )
        {
            Configuration = config ?? new ConfigBase();
            configAssign?.Invoke(Configuration);

            Service = new Microservice(name, serviceId, policy, properties);
            assign?.Invoke(Service);

            if (addDefaultJsonPayloadSerializer)
                this.AddPayloadSerializerDefaultJson();

        }

        /// <summary>
        /// This is the default pipeline.
        /// </summary>
        /// <param name="service">The microservice.</param>
        /// <param name="config">The microservice configuration.</param>
        public MicroservicePipeline(IMicroservice service, IEnvironmentConfiguration config)
        {
            if (service == null)
                throw new ArgumentNullException("service cannot be null");
            if (config == null)
                throw new ArgumentNullException("config cannot be null");

            Service = service;
            Configuration = config;
        }

        /// <summary>
        /// This is the microservice.
        /// </summary>
        public virtual IMicroservice Service { get; protected set;}

        /// <summary>
        /// This is the microservice configuration.
        /// </summary>
        public virtual IEnvironmentConfiguration Configuration { get; protected set;}

    }
}
