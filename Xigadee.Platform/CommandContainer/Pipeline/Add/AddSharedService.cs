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
    /// <summary>
    /// These extensions allow services to be registered as part of a pipeline
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This pipeline extension is used to add a service to the Microservice shared service container.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="creator">A function that takes in the pipeline configuration and returns an instance of the service.</param>
        /// <param name="serviceName">The optional service name</param>
        /// <param name="action">An optional action to access the service on assignment.</param>
        /// <returns>Returns the pipeliene.</returns>
        public static P AddSharedService<P,I>(this P pipeline
            , Func<IEnvironmentConfiguration, I> creator, string serviceName = null, Action<I> action = null) where I : class
            where P : IPipeline
        {
            var service = creator(pipeline.Configuration);

            action?.Invoke(service);

            if (!pipeline.Service.Commands.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
        /// <summary>
        /// This pipeline extension is used to add a service to the Microservice shared service container.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="service">The service instance to add.</param>
        /// <param name="serviceName">The optional service name</param>
        /// <param name="action">An optional action to access the service on assignment.</param>
        /// <returns>Returns the pipeliene.</returns>
        public static P AddSharedService<P,I>(this P pipeline
            , I service, string serviceName = null, Action<I> action = null) where I : class
            where P : IPipeline
        {
            action?.Invoke(service);

            if (!pipeline.Service.Commands.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
        /// <summary>
        /// This pipeline extension is used to add a service to the Microservice shared service container.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="I">The interface type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="creator">A lazy creator for the service that is called when the service is first accessed.</param>
        /// <param name="serviceName">The optional service name</param>
        /// <returns>Returns the pipeliene.</returns>
        public static P AddSharedService<P,I>(this P pipeline
            , Lazy<I> creator, string serviceName = null) where I : class
            where P : IPipeline
        {
            if (!pipeline.Service.Commands.SharedServices.RegisterService<I>(creator, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
    }
}
