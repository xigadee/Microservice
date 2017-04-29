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
        /// This method adds a command initiator to the Microservice incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachCommandInitiator<C>(this C cpipe
            , out CommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            )
            where C:IPipelineChannelIncoming<IPipeline>
        {
            cpipe.Pipeline.AddCommandInitiator(out command
                , startupPriority, defaultRequestTimespan, cpipe);

            return cpipe;
        }

        /// <summary>
        /// This method adds a command initiator to the Microservice incoming channel.
        /// </summary>
        /// <typeparam name="C">The pipeline type.</typeparam>
        /// <param name="cpipe">The incoming channel pipeline.</param>
        /// <param name="command">The command initiator output.</param>
        /// <param name="startupPriority">The start up priority. The default is 90.</param>
        /// <param name="defaultRequestTimespan">The default request timespan.</param>
        /// <returns>The pipeline.</returns>
        public static C AttachICommandInitiator<C>(this C cpipe
            , out ICommandInitiator command
            , int startupPriority = 90
            , TimeSpan? defaultRequestTimespan = null
            )
            where C : IPipelineChannelIncoming<IPipeline>
        {
            cpipe.Pipeline.AddICommandInitiator(out command
                , startupPriority, defaultRequestTimespan, cpipe);

            return cpipe;
        }
    }
}