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
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method can be used to inspect the pipeline components.
        /// </summary>
        /// <typeparam name="C">The channel pipeline.</typeparam>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="msAssign">The Microservice action.</param>
        /// <param name="cfAssign">The configuration action.</param>
        /// <param name="cnAssign">The channel action.</param>
        /// <returns>The pipline.</returns>
        public static C Inspect<C,P>(this C pipeline
            , Action<IMicroservice> msAssign = null
            , Action<IEnvironmentConfiguration> cfAssign = null
            , Action<Channel> cnAssign = null)
            where C:ChannelPipelineBase<P>
            where P:IPipeline
        {
            msAssign?.Invoke(pipeline.Pipeline.Service);
            cfAssign?.Invoke(pipeline.Pipeline.Configuration);
            cnAssign?.Invoke(pipeline.Channel);

            return pipeline;
        }


    }
}
