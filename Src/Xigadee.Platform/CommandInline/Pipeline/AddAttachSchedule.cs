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
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension adds the inline command to the pipeline
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="function">The command schedule function.</param>
        /// <param name="timerConfig">The schedule timer configuration.</param>
        /// <param name="referenceId">The optional command reference id</param>
        /// <param name="isLongRunning">Specifies whether the schedule will be long running.</param>
        /// <param name="startupPriority">The command startup priority.</param>
        /// <param name="channelIncoming">The incoming channel. This is optional if you pass channel information in the header.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddSchedule<P>(this P pipeline
            , Func<CommandScheduleInlineContext, Task> function
            , ScheduleTimerConfig timerConfig = null
            , string referenceId = null
            , bool isLongRunning = false
            , int startupPriority = 100
            , IPipelineChannelIncoming<P> channelIncoming = null
            )
            where P : IPipeline
        {
            var command = new CommandScheduleInline(function, timerConfig, referenceId, isLongRunning);

            pipeline.AddCommand(command, startupPriority, null, channelIncoming);

            return pipeline;
        }
    }
}
