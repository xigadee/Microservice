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
    /// These extension methods allow the channel to auto-set a boundary logger.
    /// </summary>
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method adds a boundary logger to the channel.
        /// </summary>
        /// <param name="pipeline">The pipe.</param>
        /// <param name="boundaryLogger">The boundary logger.</param>
        /// <returns>Returns the pipe.</returns>
        public static MicroservicePipeline AddBoundaryLogger(this MicroservicePipeline pipeline
            , IBoundaryLoggerComponent boundaryLogger)
        {
            pipeline.Service.RegisterBoundaryLogger(boundaryLogger);

            return pipeline;
        }

        /// <summary>
        /// This method adds a boundary logger to the channel.
        /// </summary>
        /// <typeparam name="L">The boundary logger</typeparam>
        /// <param name="pipeline">The pipe.</param>
        /// <param name="boundaryLogger">The boundary logger.</param>
        /// <param name="action">The action that is called when the logger is added.</param>
        /// <returns>Returns the pipe.</returns>
        public static MicroservicePipeline AddBoundaryLogger<L>(this MicroservicePipeline pipeline
            , L boundaryLogger
            , Action<MicroservicePipeline, L> action = null
            )
            where L : IBoundaryLoggerComponent
        {

            action?.Invoke(pipeline, boundaryLogger);
            pipeline.AddBoundaryLogger(boundaryLogger);

            return pipeline;
        }

        /// <summary>
        /// This method adds a boundary logger to the channel.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <param name="cpipe">The pipe.</param>
        /// <param name="creator">This function is used to create the boundary logger.</param>
        /// <param name="action">The action that is called when the logger is added.</param>
        /// <returns>Returns the pipe.</returns>
        public static MicroservicePipeline AddBoundaryLogger<L>(this MicroservicePipeline cpipe
            , Func<IEnvironmentConfiguration, L> creator
            , Action<MicroservicePipeline, L> action = null
            )
            where L : IBoundaryLoggerComponent
        {
            var bLogger = creator(cpipe.Configuration);

            return cpipe.AddBoundaryLogger(bLogger, action);
        }
    }
}
