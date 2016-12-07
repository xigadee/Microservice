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
using Microsoft.Practices.Unity;

namespace Xigadee
{
    public static partial class UnityWebApiExtensionMethods
    {
        public static E AttachCommandUnity<E,I,C>(this E cpipe
            , Func<IEnvironmentConfiguration, C> creator
            , int startupPriority = 100
            , Action<C> assign = null
            )
            where E : IPipelineChannelIncoming<IPipelineWebApiUnity>
            where C : I, ICommand
        {
            C command = creator(cpipe.Pipeline.Configuration);

            return cpipe.AttachCommandUnity<E, I, C>(command, startupPriority, assign);
        }

        public static E AttachCommandUnity<E,I,C>(this E cpipe
            , C command
            , int startupPriority = 100
            , Action<C> assign = null
            )
            where E : IPipelineChannelIncoming<IPipelineWebApiUnity>
            where C : I, ICommand
        {
            var pipeline = cpipe.Pipeline as IPipelineWebApiUnity;

            if (pipeline == null)
                throw new UnityWebApiMicroservicePipelineInvalidException();

            cpipe.AttachCommand(command, startupPriority, assign);

            pipeline.Unity.RegisterInstance<I>(command);

            return cpipe;
        }

    }
}
