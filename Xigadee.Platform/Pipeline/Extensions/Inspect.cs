﻿#region Copyright
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
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension allows for specific parts of the Microservice to be inspected or altered.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="msInspect">An action to inspect the Microservice</param>
        /// <param name="cfInspect">An action to inspect the configuration.</param>
        /// <returns>The passthrough of the pipeline.</returns>
        public static IPipeline Inspect(this IPipeline pipeline
            , Action<IMicroservice> msInspect = null
            , Action<IEnvironmentConfiguration> cfInspect = null)
        {
            msInspect?.Invoke(pipeline.Service);
            cfInspect?.Invoke(pipeline.Configuration);

            return pipeline;
        }
    }
}
