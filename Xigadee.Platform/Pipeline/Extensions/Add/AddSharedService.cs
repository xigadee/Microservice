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
    /// <summary>
    /// These extensions allow services to be registered as part of a pipeline
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        public static MicroservicePipeline AddSharedService<I>(this MicroservicePipeline pipeline
            , Func<IEnvironmentConfiguration, I> creator, string serviceName = null, Action<I> action = null) where I : class
        {
            var service = creator(pipeline.Configuration);

            action?.Invoke(service);

            if (!pipeline.Service.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }

        public static MicroservicePipeline AddSharedService<I>(this MicroservicePipeline pipeline
            , I service, string serviceName = null, Action<I> action = null) where I : class
        {
            action?.Invoke(service);

            if (!pipeline.Service.SharedServices.RegisterService<I>(service, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }

        public static MicroservicePipeline AddSharedService<I>(this MicroservicePipeline pipeline
            , Lazy<I> creator, string serviceName = null) where I : class
        {
            if (!pipeline.Service.SharedServices.RegisterService<I>(creator, serviceName))
                throw new SharedServiceRegistrationException(typeof(I).Name, serviceName);

            return pipeline;
        }
    }
}
