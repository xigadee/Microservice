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
        /// This method adds the encryption handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// This is will be used when assigning the handler to a channel or collector.</param>
        /// <param name="handler">The handler instance.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddEncryptionHandler<P>(this P pipeline
            , string identifier
            , IEncryptionHandler handler
            , Action<IEncryptionHandler> action = null)
            where P : IPipeline
        {
            action?.Invoke(handler);

            pipeline.Service.Security.RegisterEncryptionHandler(identifier, handler);

            return pipeline;
        }

        /// <summary>
        /// This method adds the encryption handler to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="identifier">The encryption type identifier. 
        /// <param name="creator">This function is used to create the handler from the configuration collection.</param>
        /// <param name="action">The action on the handler.</param>
        /// <returns>The pipeline.</returns>
        public static P AddEncryptionHandler<P>(this P pipeline
            , string identifier
            , Func<IEnvironmentConfiguration, IEncryptionHandler> creator
            , Action<IEncryptionHandler> action = null)
            where P : IPipeline
        {
            var handler = creator(pipeline.Configuration);

            action?.Invoke(handler);

            pipeline.Service.Security.RegisterEncryptionHandler(identifier, handler);

            return pipeline;
        }
    }
}
