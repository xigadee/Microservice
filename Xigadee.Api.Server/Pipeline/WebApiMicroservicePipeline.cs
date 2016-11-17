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
using System.Web.Http;

namespace Xigadee
{
    /// <summary>
    /// This extension pipeline is used by the Web Api pipeline.
    /// </summary>
    public class WebApiMicroservicePipeline: MicroservicePipeline
    {
        #region Constructor
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
        /// <param name="httpConfig">The http configuration.</param>
        public WebApiMicroservicePipeline(string name = null
            , string serviceId = null
            , IEnumerable<PolicyBase> policy = null
            , IEnumerable<Tuple<string, string>> properties = null
            , IEnvironmentConfiguration config = null
            , Action<IMicroservice> assign = null
            , Action<IEnvironmentConfiguration> configAssign = null
            , bool addDefaultJsonPayloadSerializer = true
            , HttpConfiguration httpConfig = null
            ) :base(name, serviceId, policy, properties, config, assign, configAssign, addDefaultJsonPayloadSerializer)
        {
            HttpConfig = httpConfig ?? new HttpConfiguration();
        }

        /// <summary>
        /// This is the default pipeline.
        /// </summary>
        /// <param name="service">The microservice.</param>
        /// <param name="config">The microservice configuration.</param>
        /// <param name="httpConfig">The http configuration.</param>
        public WebApiMicroservicePipeline(IMicroservice service
            , IEnvironmentConfiguration config
            , HttpConfiguration httpConfig = null):base(service, config)
        {
            HttpConfig = httpConfig ?? new HttpConfiguration();
        }
        #endregion

        #region HttpConfig
        /// <summary>
        /// This is the http configuration class used for the Web Api instance.
        /// </summary>
        public HttpConfiguration HttpConfig { get; protected set; }
        #endregion
    }
}
