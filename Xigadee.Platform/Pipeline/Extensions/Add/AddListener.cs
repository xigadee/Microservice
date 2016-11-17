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
        public static MicroservicePipeline AddListener(this MicroservicePipeline pipeline, IListener listener)
        {
            pipeline.Service.RegisterListener(listener);

            return pipeline;
        }

        public static MicroservicePipeline AddListener<S>(this MicroservicePipeline pipeline, Func<IEnvironmentConfiguration, S> creator, Action<S> action = null)
            where S : IListener
        {
            var listener = creator(pipeline.Configuration);

            action?.Invoke(listener);

            pipeline.AddListener(listener);

            return pipeline;
        }
    }
}
