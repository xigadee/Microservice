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
using System.Linq;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {

        /// <summary>
        /// This extension method changes the default Microservice communication listener policy to balance load between multiple listener clients.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="fabric">The fabric to configure from the pipeline.</param>
        /// <returns>Returns the pipeline</returns>
        public static P FabricConfigure<P>(this P pipeline, ManualFabricBridge fabric) where P : IPipeline
        {
            return pipeline;
        }
    }
}
