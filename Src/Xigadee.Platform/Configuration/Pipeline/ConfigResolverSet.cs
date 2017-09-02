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
        /// Sets a configuration resolver for the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="priority">The resolver priority.</param>
        /// <param name="resolver">The resolver class.</param>
        /// <param name="assign">The pre-assignment method.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">The pipeline cannot be null</exception>
        public static P ConfigResolverSet<P>(this P pipeline, int priority, ConfigResolver resolver, Action<ConfigResolver> assign = null)
            where P : IPipeline
        {
            if (pipeline == null)
                throw new ArgumentNullException("pipeline cannot be null");

            assign?.Invoke(resolver);

            pipeline.Configuration.ResolverSet(priority, resolver);

            return pipeline;
        }

        /// <summary>
        /// Sets a configuration resolver for the Microservice for a resolver of type R.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="R">The resolver type that supports a new() constructor.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="priority">The resolver priority.</param>
        /// <param name="assign">The pre-assignment method.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">pipeline cannot be null</exception>
        public static P ConfigResolverSet<P, R>(this P pipeline, int priority, Action<R> assign = null)
            where P : IPipeline
            where R : ConfigResolver, new()
        {
            if (pipeline == null)
                throw new ArgumentNullException("pipeline cannot be null");

            var resolver = new R();

            assign?.Invoke(resolver);

            pipeline.Configuration.ResolverSet(priority, resolver);

            return pipeline;
        }

        /// <summary>
        /// Sets a configuration resolver for the Microservice for a resolver of type R.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="R">The resolver type that supports a new() constructor.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="priority">The resolver priority.</param>
        /// <param name="resolver">The new resolver instance.</param>
        /// <param name="assign">The pre-assignment method.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">pipeline cannot be null</exception>
        public static P ConfigResolverSet<P, R>(this P pipeline, int priority, out R resolver, Action<R> assign = null)
            where P : IPipeline
            where R : ConfigResolver, new()
        {
            if (pipeline == null)
                throw new ArgumentNullException("pipeline cannot be null");

            resolver = new R();

            assign?.Invoke(resolver);

            pipeline.Configuration.ResolverSet(priority, resolver);

            return pipeline;
        }
    }
}
