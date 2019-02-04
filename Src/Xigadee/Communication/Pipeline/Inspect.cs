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
        /// If the extension method is attached to a channel pipeline, the channel will also be returned, otherwise this will be null.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="inspect">The action that receives a tuple that contains the Microservice, the configuration action, and the channel.</param>
        /// <returns>The pipeline.</returns>
        public static P Inspect<P>(this P pipeline
            , Action<(IMicroservice ms, IEnvironmentConfiguration config, Channel channel)> inspect)
            where P : IPipeline
        {
            if (inspect == null)
                throw new ArgumentNullException("inspect", "inspect cannot be null");

            Channel channel = null;
            if (pipeline is IPipelineChannel)
                channel = ((IPipelineChannel)pipeline).Channel;

            inspect((pipeline.Service, pipeline.Configuration, channel));

            return pipeline;
        }
    }
}
