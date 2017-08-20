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
        /// Adds the default Json payload serializer to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The pipeline</returns>
        public static P AddPayloadSerializerDefaultJson<P>(this P pipeline)
            where P : IPipeline
        {
            var component = pipeline.Service.Serialization.RegisterPayloadSerializer(new JsonContractSerializer());

            return pipeline;
        }
        /// <summary>
        /// Adds the payload serializer.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="serializer">The serializer to add.</param>
        /// <returns>The pipeline</returns>
        public static P AddPayloadSerializer<P>(this P pipeline, IPayloadSerializer serializer)
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadSerializer(serializer);

            return pipeline;
        }
        /// <summary>
        /// Adds the payload serializer.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The serializer creator function.</param>
        /// <returns>The pipeline</returns>
        public static P AddPayloadSerializer<P>(this P pipeline
            , Func<IEnvironmentConfiguration, IPayloadSerializer> creator)
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadSerializer(creator(pipeline.Configuration));

            return pipeline;
        }
    }
}
