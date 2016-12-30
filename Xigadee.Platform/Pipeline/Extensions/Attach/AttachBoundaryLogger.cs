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
using Xigadee;
namespace Xigadee
{
    /// <summary>
    /// These extension methods allow the channel to auto-set a boundary Collector?.
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method adds a boundary logger to the channel.
        /// </summary>
        /// <typeparam name="L">The boundary logger</typeparam>
        /// <param name="cpipe">The pipe.</param>
        /// <param name="boundaryLogger">The boundary Collector?.</param>
        /// <param name="action">The action that is called when the logger is added.</param>
        /// <returns>Returns the pipe.</returns>
        public static C AttachBoundaryLogger<C,L>(this C cpipe
            , L boundaryLogger
            , Action<C, L> action = null
            )
            where L : IBoundaryLoggerComponent
            where C : IPipelineExtension<IPipeline>
        {
            action?.Invoke(cpipe, boundaryLogger);
            cpipe.Pipeline.Service.DataCollection.RegisterBoundaryLogger(boundaryLogger);

            return cpipe;
        }

        /// <summary>
        /// This method adds a boundary logger to the channel.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <param name="cpipe">The pipe.</param>
        /// <param name="creator">This function is used to create the boundary Collector?.</param>
        /// <param name="action">The action that is called when the logger is added.</param>
        /// <returns>Returns the pipe.</returns>
        public static C AttachBoundaryLogger<C,L>(this C cpipe
            , Func<IEnvironmentConfiguration, L> creator
            , Action<C, L> action = null
            )
            where L : IBoundaryLoggerComponent
            where C : IPipelineExtension<IPipeline>
        {
            var boundaryLogger = creator(cpipe.Pipeline.Configuration);

            action?.Invoke(cpipe, boundaryLogger);
            cpipe.Pipeline.Service.DataCollection.RegisterBoundaryLogger(boundaryLogger);

            return cpipe;

        }
    }
}
