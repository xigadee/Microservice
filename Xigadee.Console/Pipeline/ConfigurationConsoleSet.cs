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
    public static partial class ConsolePipelineExtensions
    {
        /// <summary>
        /// This method adds an override setting and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="key">The key.</param>
        /// <param name="value"></param>
        /// <returns>Returns the pipeline.</returns>
        public static P ConfigurationConsoleSet<P>(this P pipeline, string[] args, string strStart = @"/", string strDelim = @":", bool throwErrors = false
            , Func<string,string,bool> fnInclude = null, int priority = 1000)
            where P : IPipeline
        {
            var settings = args.CommandArgsParse(strStart, strDelim, throwErrors);

            if (fnInclude == null)
                fnInclude = (k,v) => true;

            ConfigResolverMemory cfResolver = new ConfigResolverMemory();

            settings
                .Where((k) => fnInclude(k.Key, k.Value))
                .ForEach((k) => cfResolver.Add(k.Key, k.Value));

            pipeline.Configuration.ResolverSet(priority, cfResolver);

            pipeline.Configuration.CacheFlush();

            return pipeline;
        }
    }
}
