#region using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is used to define a schedule for the SchedulerContainer.
    /// </summary>
    [DebuggerDisplay("{Debug}")]
    public class Schedule:IEquatable<Schedule>
    {
        #region Declarations
        private bool mShouldExecute;
        private Func<Schedule, CancellationToken, Task> mExecute;
        private long mExecutionCount = 0;
        private long mExecuteActiveSkipCount = 0;
        private ScheduleTimerConfig mTimerConfig = null;
        #endregion
        #region Constructor    
        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// The schedule is disabled until the initialise function is called.
        /// </summary>
        public Schedule()
        {
            mShouldExecute = false;
            Statistics = new MessagingStatistics();
            Statistics.ComponentId = Id;
            mTimerConfig = new ScheduleTimerConfig(enforceSetting:false);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        /// <param name="execute">The async schedule function.</param>
        /// <param name="name">The schedule name.</param>
        /// <param name="context">The context.</param>
        /// <param name="timerConfig">The optional timer configuration.</param>
        /// <param name="isLongRunning">Specifies whether the schedule is a long running process.</param>
        public Schedule(Func<Schedule, CancellationToken, Task> execute, ScheduleTimerConfig timerConfig, string name = null, object context = null, bool isLongRunning = false) :this()
        {
            Initialise(execute, timerConfig, name, context);
            Statistics.Name = name;
        }
        #endregion

        #region Initialise(Func<Schedule, CancellationToken, Task> execute, string name = null, object context = null, ScheduleTimerConfig timerConfig = null, bool isLongRunning = false)
        /// <summary>
        /// Initialises the schedule.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="name">The name.</param>
        /// <param name="context">The context.</param>
        /// <param name="timerConfig">The optional timer configuration.</param>
        /// <param name="isLongRunning">Specifies whether the schedule is a long running process.</param>
        public virtual void Initialise(Func<Schedule, CancellationToken, Task> execute
            , ScheduleTimerConfig timerConfig
            , string name = null
            , object context = null
            , bool isLongRunning = false)
        {
            mExecute = execute ?? throw new ArgumentNullException("execute", $"{GetType().Name}/{nameof(Initialise)}");

            mShouldExecute = true;

            Name = name;

            Context = context;

            mTimerConfig = timerConfig ?? throw new ArgumentNullException("timerConfig");

            IsLongRunning = isLongRunning;
        }
        #endregion

        #region StatusIsInitialPoll
        /// <summary>
        /// Indicates whether this instance is first scheduled poll.
        /// </summary>
        public bool StatusIsInitialPoll => mExecutionCount == 1;
        #endregion
        #region StatusRequiresTimerConfiguration
        /// <summary>
        /// This will be set to true when the schedule is first called and the timer config has not been set.
        /// If the timer schedule is not changed before leaving the method, it will not be called again.
        /// </summary>
        public bool StatusRequiresTimerConfiguration => StatusIsInitialPoll && (mTimerConfig?.IsUnset??true); 
        #endregion

        #region ScheduleType
        /// <summary>
        /// Gets the type of the schedule. By default, this is based on the class name, but it can be overridden.
        /// </summary>
        public virtual string ScheduleType { get { return GetType().Name; } } 
        #endregion
        #region Name
        /// <summary>
        /// This is the override for the name to bring it to the top .
        /// </summary>
        public string Name { get; protected set; }
        #endregion
        #region Id
        /// <summary>
        /// This is the time Id.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();
        #endregion

        #region Context
        /// <summary>
        /// Gets or sets the context which can be used to store data between scheduled calls.
        /// </summary>
        public object Context { get; set; }
        #endregion

        /// <summary>
        /// When this is set to disabled the schedule will not be executed.
        /// This is typically set by a process that wishes to stop polling.
        /// </summary>
        public bool Disabled { get; set; } = false;

        #region Active
        /// <summary>
        /// Indicates whether the schedule is currently executing.
        /// </summary>
        public bool Active { get; protected set; }
        #endregion
        #region WillRunIn
        /// <summary>
        /// This is a debug property to identify when the schedule will next run.
        /// </summary>
        public string WillRunIn
        {
            get
            {
                if (Active || !NextExecuteTime.HasValue)
                    return null;
                var span = NextExecuteTime.Value - DateTime.UtcNow;

                return StatsCounter.LargeTime(span);
            }
        }
        #endregion

        #region Start()
        /// <summary>
        /// Resets the schedule for the start of execution.
        /// </summary>
        public virtual bool Start()
        {
            if (Disabled)
                return false;

            if (Active)
            {
                ExecutingSoSkip();
                return false;
            }

            Interlocked.Increment(ref mExecutionCount);
            ExecuteException = null;
            Active = true;

            return true;
        }
        #endregion

        #region Execute()
        /// <summary>
        /// This method executes the schedule.
        /// </summary>
        public async Task Execute(CancellationToken token)
        {
            int start = Statistics.ActiveIncrement();

            try
            {
                LastExecuteTime = DateTime.UtcNow;
                await mExecute(this, token);
            }
            finally
            {
                Statistics.ActiveDecrement(start);
            }
        }
        #endregion
        #region LastExecuteTime
        /// <summary>
        /// This is the next execute time in UTC.
        /// </summary>
        public DateTime? LastExecuteTime{get;protected set;}
        #endregion

        #region Stop(...)
        /// <summary>
        /// Completes the specified poll.
        /// </summary>
        /// <param name="success">Specifies whether the poll was a success.</param>
        /// <param name="recalculate">Specifies whether to recalculate the next poll.</param>
        /// <param name="isException">Specifies whether the poll failed..</param>
        /// <param name="lastEx">The last exception.</param>
        /// <param name="exceptionTime">The exception time.</param>
        public virtual void Stop(bool success
            , bool recalculate = true
            , bool isException = false
            , Exception lastEx = null
            , DateTime? exceptionTime = null)
        {
            if (!Active)
                return;

            if (!success)
                Statistics.ErrorIncrement();

            if (isException)
                ExecuteException = new ExceptionHolder(lastEx, exceptionTime);

            if (recalculate)
                Recalculate();

            Active = false;
        }
        #endregion

        #region InitialInterval
        /// <summary>
        /// This is the initial wait as a TimeSpan before the first timer event fires.
        /// </summary>
        public TimeSpan? InitialInterval
        {
            get { return mTimerConfig.InitialInterval; }
            set { mTimerConfig.InitialInterval = value; }
        }
        #endregion
        #region UTCPollTime
        /// <summary>
        /// This is the specific date-time that the schedule should use for it's next execution.
        /// </summary>
        public DateTime? UTCPollTime
        {
            get { return mTimerConfig.UTCPollTime; }
            set { mTimerConfig.UTCPollTime = value; }
        }
        #endregion
        #region Interval
        /// <summary>
        /// This is the repeat frequency that the schedule should fire.
        /// </summary>
        public TimeSpan? Interval
        {
            get { return mTimerConfig.Interval; }
            set { mTimerConfig.Interval = value; }
        }
        #endregion

        #region ExecutionCount
        /// <summary>
        /// This is the number of times the schedule has been executed.
        /// </summary>
        public long ExecutionCount
        {
            get
            {
                return mExecutionCount;
            }
        }
        #endregion

        #region ExecutingSoSkip()
        /// <summary>
        /// This method is called when the job is active but the ShouldPoll condition has been met.
        /// </summary>
        public void ExecutingSoSkip()
        {
            Interlocked.Increment(ref mExecuteActiveSkipCount);
        }
        #endregion
        #region ExecuteActiveSkipCount
        /// <summary>
        /// This is the count of the number of time the schedule was meant to run but 
        /// was still active so was skipped
        /// </summary>
        public long ExecuteActiveSkipCount { get { return mExecuteActiveSkipCount; } }
        #endregion

        #region ExecuteException
        /// <summary>
        /// Gets the last execution exception.
        /// </summary>
        public ExceptionHolder ExecuteException { get; set; }
        #endregion

        #region Recalculate(bool force = false)
        /// <summary>
        /// This method recalculates the next time the event should fire.
        /// </summary>
        /// <param name="force">This parameter forces the schedule to recalculate even if the next poll time is in the future.</param>
        public virtual void Recalculate(bool force = false)
        {
            try
            {
                //Check whether we have a time set already, if so quit.
                if (!force && NextExecuteTime.HasValue && NextExecuteTime.Value > DateTime.UtcNow)
                    return;

                if (UTCPollTime.HasValue)
                {
                    NextExecuteTime = UTCPollTime.Value;
                    InitialInterval = null;
                    UTCPollTime = null;
                    return;
                }

                if (InitialInterval.HasValue)
                {
                    NextExecuteTime = DateTime.UtcNow.Add(InitialInterval.Value);
                    InitialInterval = null;
                    UTCPollTime = null;
                    return;
                }

                if (Interval.HasValue)
                {
                    NextExecuteTime = DateTime.UtcNow.Add(Interval.Value);
                    return;
                }

                NextExecuteTime = null;
            }
            catch (Exception ex)
            {
                throw new ScheduleRecalculateException($"Schedule recalculation failed for {ScheduleType}:'{Name ?? Id.ToString("N").ToUpperInvariant()}' Active={Active} -> InitialTime:'{UTCPollTime}' InitialWait:'{InitialInterval}' Frequency:'{Interval}'", ex);
            }

        }
        #endregion
        #region NextExecuteTime
        /// <summary>
        /// This is the next execute time in UTC.
        /// </summary>
        public DateTime? NextExecuteTime { get; protected set; }
        #endregion

        #region ShouldExecute
        /// <summary>
        /// This boolean property calculates whether the schedule should execute.
        /// You can set this to false to disable the timer event.
        /// </summary>
        public bool ShouldExecute
        {
            get
            {
                if (Disabled)
                    return false;

                //One time hit for the first execution.
                if (mExecutionCount == 0 && !InitialInterval.HasValue && !UTCPollTime.HasValue && !Interval.HasValue)
                    return true;

                var compareTime = DateTime.UtcNow;

                bool result = !Active
                    && mShouldExecute
                    && NextExecuteTime.HasValue
                    && (NextExecuteTime.Value <= compareTime);

                return result;
            }
            set
            {
                mShouldExecute = value;
                if (value)
                    Recalculate();
            }
        }
        #endregion

        #region IsInternal
        /// <summary>
        /// Identifies whether the schedule is internal and should skip any process queueing.
        /// </summary>
        public bool IsInternal { get; set; }
        #endregion
        #region ExecutionPriority
        /// <summary>
        /// Gets or sets the task execution priority. If this is null, then the default set in the scheduler priority will be used instead. If IsInternal=true then this property is ignored.
        /// </summary>
        public int? ExecutionPriority { get; set; } = null; 
        #endregion
        #region IsLongRunning
        /// <summary>
        /// A helper method that lets the task scheduler that the process might be long running.
        /// </summary>
        public bool IsLongRunning { get; set; }
        #endregion

        #region Statistics
        /// <summary>
        /// Gets the statistics.
        /// </summary>
        public MessagingStatistics Statistics { get; }
        #endregion

        #region Equals(Schedule other)
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> Id parameter; otherwise, false.
        /// </returns>
        public bool Equals(Schedule other)
        {
            return other != null && other.Id == Id;
        } 
        #endregion

        #region Debug
        /// <summary>
        /// This is the debug message used for the statistics.
        /// </summary>
        public virtual string Debug
        {
            get
            {
                return $"{ScheduleType}:'{Name ?? Id.ToString("N").ToUpperInvariant()}' Disabled={Disabled} Active={Active} [ShouldExecute={ShouldExecute}] @ {NextExecuteTime} Run={ExecutionCount}";
            }
        }
        #endregion
    }
}
