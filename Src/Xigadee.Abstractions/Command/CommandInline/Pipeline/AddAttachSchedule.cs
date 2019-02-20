using System;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This extension adds the in-line command to the pipeline
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="function">The command schedule function.</param>
        /// <param name="timerConfig">The schedule timer configuration.</param>
        /// <param name="referenceId">The optional command reference id</param>
        /// <param name="isLongRunning">Specifies whether the schedule will be long running.</param>
        /// <param name="startupPriority">The command start-up priority.</param>
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
