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
    public static partial class AzureExtensionMethods
    {
        /// <summary>
        /// Adds the Application Insights data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="collector">The collector.</param>
        /// <returns>The pipeline.</returns>
        public static P AddApplicationInsightsDataCollector<P>(this P pipeline, ApplicationInsightsDataCollector collector)
            where P: IPipeline
        {
            pipeline.ToMicroservice().DataCollection.Register(collector);

            return pipeline;
        }
        /// <summary>
        /// Adds the Application Insights data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="L">The collector type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The creator function.</param>
        /// <param name="action">The post-creation action.</param>
        /// <returns>The pipeline.</returns>
        public static P AddApplicationInsightsDataCollector<P,L>(this P pipeline
            , Func<IEnvironmentConfiguration, L> creator, Action<L> action = null)
            where P : IPipeline
            where L : ApplicationInsightsDataCollector
        {
            var collector = creator(pipeline.ToConfiguration());

            action?.Invoke(collector);

            pipeline.ToMicroservice().DataCollection.Register(collector);

            return pipeline;
        }
        /// <summary>
        /// Adds the Application Insights data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="action">The post-creation action.</param>
        /// <returns>The pipeline.</returns>
        public static P AddApplicationInsightsDataCollector<P>(this P pipeline
            , Action<ApplicationInsightsDataCollector> action = null)
             where P : IPipeline
        {
           
            return pipeline.AddApplicationInsightsDataCollector(
                ec => new ApplicationInsightsDataCollector(ec.ApplicationInsightsKey()
                , ec.ApplicationInsightsLoggingLevel())
                , action);
        }
    }
}
