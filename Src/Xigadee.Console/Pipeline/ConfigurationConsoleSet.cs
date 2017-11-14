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
        /// This method adds an override setting from the console arguments and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The Microservice pipeline.</param>
        /// <param name="args">The console arguments.</param>
        /// <param name="strStart">The switch start character.</param>
        /// <param name="strDelim">The delimiter character.</param>
        /// <param name="throwErrors">Throws an error if duplicate keys are found or the values are in an incorrect format.</param>
        /// <param name="fnInclude">This function can be used to filter specific keys.</param>
        /// <param name="priority">This is the default priority for the arguments, which is 1000.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P ConfigurationSetFromConsoleArgs<P>(this P pipeline, string[] args, string strStart = @"/", string strDelim = @":", bool throwErrors = false
            , Func<string,string,bool> fnInclude = null, int priority = 1000)
            where P : IPipeline
        {
            if (args == null)
            {
                if (throwErrors)
                    throw new ArgumentNullException("args", "Arguments cannot be null.");
                return pipeline;
            }

            var settings = args.CommandArgsParse(strStart, strDelim, throwErrors);

            return pipeline.ConfigurationSetFromConsoleArgs(settings, fnInclude, priority);
        }
        /// <summary>
        /// This method adds an override setting and clears the cache.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="pipeline">The Microservice pipeline.</param>
        /// <param name="settings">The settings key value pair.</param>
        /// <param name="fnInclude">This function can be used to filter specific keys.</param>
        /// <param name="priority">This is the default priority for the arguments, which is 1000.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P ConfigurationSetFromConsoleArgs<P>(this P pipeline, Dictionary<string,string> settings
            , Func<string, string, bool> fnInclude = null, int priority = 1000)
            where P : IPipeline
        {
            if (settings == null)
                throw new ArgumentNullException("settings", "Settings cannot be null.");

            if (settings.Count > 0)
            {
                if (fnInclude == null)
                    fnInclude = (k, v) => true;

                ConfigResolverMemory cfResolver = new ConfigResolverMemory();

                settings
                    .Where((k) => fnInclude(k.Key, k.Value))
                    .ForEach((k) => cfResolver.Add(k.Key, k.Value));

                pipeline.Configuration.ResolverSet(priority, cfResolver);

                pipeline.Configuration.CacheFlush();
            }

            return pipeline;
        }
    }
}
