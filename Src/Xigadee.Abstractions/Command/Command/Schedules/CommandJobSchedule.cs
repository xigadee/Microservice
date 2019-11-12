using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the command schedule generated for the specific command requirement.
    /// </summary>
    public class CommandJobSchedule: Schedule
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandJobSchedule"/> class.
        /// </summary>
        public CommandJobSchedule() : base()
        {

        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="execute">The execution function.</param>
        /// <param name="timerConfig">The timer poll configuration.</param>
        /// <param name="context">The optional schedule context</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="isLongRunning">A boolean flag that specifies whether the process is long running.</param>
        /// <param name="tearUp">The set up action.</param>
        /// <param name="tearDown">The clear down action.</param>
        /// <param name="isMasterJob">Indicates whether this schedule is associated to a master job.</param>
        /// <param name="executionPriority">This is the priority that the job should run as.</param>
        public CommandJobSchedule(Func<Schedule, CancellationToken, Task> execute
            , ScheduleTimerConfig timerConfig
            , object context = null
            , string name = null
            , bool isLongRunning = false
            , Action<Schedule> tearUp = null
            , Action<Schedule> tearDown = null
            , bool isMasterJob = false
            , int? executionPriority = null
            ) : base(execute, timerConfig, name, context, isLongRunning, executionPriority)
        {
            Initialise(execute, timerConfig, context, name, isLongRunning, tearUp, tearDown, isMasterJob, executionPriority);
        } 
        #endregion

        #region Initialise(...)
        /// <summary>
        /// Initialises the schedule.
        /// </summary>
        /// <param name="execute">The execution function.</param>
        /// <param name="timerConfig">The timer poll configuration.</param>
        /// <param name="context">The optional schedule context</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="isLongRunning">A boolean flag that specifies whether the process is long running.</param>
        /// <param name="tearUp">The set up action.</param>
        /// <param name="tearDown">The clear down action.</param>
        /// <param name="isMasterJob">Indicates whether this schedule is associated to a master job.</param>
        /// <param name="executionPriority">The schedule execution priority.</param>
        public virtual void Initialise(Func<Schedule, CancellationToken, Task> execute
            , ScheduleTimerConfig timerConfig
            , object context = null
            , string name = null
            , bool isLongRunning = false
            , Action<Schedule> tearUp = null
            , Action<Schedule> tearDown = null
            , bool isMasterJob = false
            , int? executionPriority = null)
        {
            base.Initialise(execute, timerConfig, name, context, isLongRunning ,executionPriority);

            TearUp = tearUp;
            TearDown = tearDown;
            IsMasterJob = isMasterJob;
        }
        #endregion

        /// <summary>
        /// Gets a value that indicates whether this schedule is associated to a master job.
        /// </summary>
        public bool IsMasterJob { get; protected set; }
        /// <summary>
        /// Gets the initialise action.
        /// </summary>
        public Action<Schedule> TearUp { get; protected set;}
        /// <summary>
        /// Gets the clean-up action.
        /// </summary>
        public Action<Schedule> TearDown { get; protected set; }
        /// <summary>
        /// Gets or sets the command identifier.
        /// </summary>
        public Guid CommandId { get; set; }
        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// This is the debug message used for the statistics.
        /// </summary>
        public override string Debug
        {
            get
            {
                return $"Command=[{CommandId.ToString("N")}] {(IsMasterJob?"MasterJob":"Job")}:'{Name ?? Id.ToString("N").ToUpperInvariant()}' Active={Active} [ShouldExecute={ShouldExecute}] @ {NextExecuteTime} Run={ExecutionCount}";
            }
        }

    }
}
