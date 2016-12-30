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
        public static P AddDataCollector<P>(this P pipeline
            , IDataCollectorComponent collector)
            where P:IPipeline
        {
            pipeline.Service.DataCollection.Register(collector);

            return pipeline;
        }

        public static P AddDataCollector<P,L>(this P pipeline
            , Func<IEnvironmentConfiguration, L> creator
            , Action<L> action = null)
            where P : IPipeline
            where L : IDataCollectorComponent
        {
            var collector = creator(pipeline.Configuration);

            action?.Invoke(collector);

            pipeline.Service.DataCollection.Register(collector);

            return pipeline;
        }

    }
}
