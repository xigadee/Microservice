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
        /// This extension method can be used to quickly add a debug memory based data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="collector">The collector as an output parameter.</param>
        /// <param name="supportMap">The support map can be used to filter the types of events that you wish to filter. Leave this null to support all types.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddDebugMemoryDataCollector<P>(this P pipeline
            , out DebugMemoryDataCollector collector
            , DataCollectionSupport? supportMap = null)
            where P : IPipeline
        {
            DebugMemoryDataCollector collectorInt = null;
            pipeline.AddDataCollector((c) => collectorInt = new DebugMemoryDataCollector(supportMap));
            collector = collectorInt;
            return pipeline;
        }
        /// <summary>
        /// This extension method can be used to quickly add a debug memory based data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="action">This action is called to allow the collector to be assigned externally.</param>
        /// <param name="supportMap">The support map can be used to filter the types of events that you wish to filter. Leave this null to support all types.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddDebugMemoryDataCollector<P>(this P pipeline
            , Action<DebugMemoryDataCollector> action
            , DataCollectionSupport? supportMap = null)
            where P : IPipeline
        {
            DebugMemoryDataCollector collector = new DebugMemoryDataCollector(supportMap);
            pipeline.AddDataCollector(collector);
            action?.Invoke(collector);
            return pipeline;
        }
    }
}
