using System;
using System.Collections.Generic;
using System.Linq;

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
