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
    public static partial class UnityWebApiExtensionMethods
    {
        public static IPipelineWebApiUnity ToUnityWebApiMicroservicePipeline<P>(this P pipeline)
            where P : IPipelineWebApiUnity
        {
            var webpipe = pipeline as IPipelineWebApiUnity;

            if (webpipe == null)
                throw new ArgumentOutOfRangeException("The incoming pipeline does not implement IPipelineWebApiUnity");

            return webpipe;
        }
    }
}
