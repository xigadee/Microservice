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
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method changes the default Microservice policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyMicroservice<P>(this P pipeline
            , Action<MicroservicePolicy, IEnvironmentConfiguration> msAssign = null) where P: IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.Microservice, pipeline.Configuration);

            return pipeline;
        }

        /// <summary>
        /// This extension method changes the default Microservice resource tracking policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyResourceTracker<P>(this P pipeline
            , Action<ResourceContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.ResourceTracker, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice command container policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyCommandContainer<P>(this P pipeline
            , Action<CommandContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.CommandContainer, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice communication policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyCommunication<P>(this P pipeline
            , Action<CommunicationPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.Communication, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice scheduler policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyScheduler<P>(this P pipeline
            , Action<SchedulerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.Scheduler, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice security policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicySecurity<P>(this P pipeline
            , Action<SecurityContainerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.Security, pipeline.Configuration);

            return pipeline;
        }
        /// <summary>
        /// This extension method changes the default Microservice data collection policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyDataCollection<P>(this P pipeline
            , Action<DataCollectionPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.DataCollection, pipeline.Configuration);

            return pipeline;
        }
    }
}
