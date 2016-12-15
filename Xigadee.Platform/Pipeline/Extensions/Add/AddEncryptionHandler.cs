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
        public static P AddSymmetricEncryptionHandler<P>(this P pipeline
            , string identifier
            , IEncryptionHandler method
            , Action<IEncryptionHandler> action = null)
            where P : IPipeline
        {
            //var collector = creator(pipeline.Configuration);

            //action?.Invoke(collector);

            //pipeline.Service.RegisterDataCollector(collector);

            return pipeline;
        }

        public static P AddSymmetricEncryptionHandler<P>(this P pipeline
            , string identifier
            , Func<IEnvironmentConfiguration, IEncryptionHandler> creator
            , Action<IEncryptionHandler> action = null)
            where P : IPipeline
        {
            var handler = creator(pipeline.Configuration);

            action?.Invoke(handler);

            pipeline.Service.RegisterSymmetricEncryptionHandler(identifier, handler);

            return pipeline;
        }
    }
}
