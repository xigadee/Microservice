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
        /// This extension method adds a listener to the collection.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="pipeline"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public static P AddListener<P>(this P pipeline, IListener listener)
            where P : IPipeline
        {
            pipeline.Service.Communication.RegisterListener(listener);

            return pipeline;
        }

        public static P AddListener<P,S>(this P pipeline
            , Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where P : IPipeline
            where S : IListener
        {
            var listener = creator(pipeline.Configuration);

            action?.Invoke(listener);

            pipeline.AddListener(listener);

            return pipeline;
        }
    }
}
