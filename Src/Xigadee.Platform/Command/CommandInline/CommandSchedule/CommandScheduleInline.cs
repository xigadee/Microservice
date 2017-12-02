using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This command provides simple inline functionality to register a schedule through an action.
    /// </summary>
    public class CommandScheduleInline : CommandBase
    {
        #region Declarations
        Func<CommandScheduleInlineContext, Task> mCommand;
        string mReferenceId;
        ScheduleTimerConfig mTimerConfig;
        bool mIsLongRunning;
        #endregion

        /// <summary>
        /// This is the constructor for registering a manual schedule.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <param name="config">The time configuration.</param>
        /// <param name="referenceId">The optional reference id for tracking.</param>
        /// <param name="isLongRunning">Specifies whether this is a long running process.</param>
        /// <param name="policy">The command policy.</param>
        public CommandScheduleInline(Func<CommandScheduleInlineContext, Task> command
            , ScheduleTimerConfig config = null
            , string referenceId = null
            , bool isLongRunning = false
            , CommandPolicy policy = null) :base(policy)
        {
            mCommand = command ?? throw new ArgumentNullException("command", "The command function cannot be null");

            mTimerConfig = config;
            mReferenceId = referenceId;
            mIsLongRunning = isLongRunning;
        }

        /// <summary>
        /// This override registers the manual schedule.
        /// </summary>
        protected override void JobSchedulesManualRegister()
        {
            base.JobSchedulesManualRegister();

            JobScheduleRegister(
                async (schedule, token) => await mCommand(
                    new CommandScheduleInlineContext(schedule, token, PayloadSerializer, Collector, SharedServices, OriginatorId, Outgoing)
                    )
                , mTimerConfig
                , null
                , mReferenceId
                , mIsLongRunning);
        }
    }
}
