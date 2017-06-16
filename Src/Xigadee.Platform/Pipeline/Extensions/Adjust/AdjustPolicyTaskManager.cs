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
        /// This extension method changes the default Microservice task manager policy.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The assignment function.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyTaskManager<P>(this P pipeline
            , Action<TaskManagerPolicy, IEnvironmentConfiguration> msAssign = null) where P : IPipeline
        {
            msAssign?.Invoke(pipeline.Service.Policy.TaskManager, pipeline.Configuration);

            return pipeline;
        }

        /// <summary>
        /// This extension method changes the default Microservice task manager policy to support Aysnc IO.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="maxPriorityLevel">The maximum priority level. The default is 3.</param>
        /// <returns>Returns the pipeline</returns>
        public static P AdjustPolicyTaskManagerForAsyncIO<P>(this P pipeline, int maxPriorityLevel = 3) where P : IPipeline
        {
            pipeline.AdjustPolicyTaskManager((p,c) =>
            {

            });

            return pipeline;
        }
    }
}
