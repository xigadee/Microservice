#region using
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This container holds the current tasks being processed on the system and calculates the availabile slots for the supported priority levels.
    /// </summary>
    public class TaskManager: ServiceBase<TaskManagerStatistics>, IServiceLogger
    {
        #region Declarations
        /// <summary>
        /// This is a collection of the current active processes on the system.
        /// </summary>
        private ConcurrentDictionary<string, TaskManagerProcessContext> mProcesses;
        /// <summary>
        /// This is the message pump that loops around the various options.
        /// </summary>
        private Thread mMessagePump;
        /// <summary>
        /// This boolean property indicates whether the message pump is active.
        /// </summary>
        private bool MessagePumpActive { get; set; }
        /// <summary>
        /// This is the manual reset lock that is triggered when a job completes.
        /// </summary>
        private ManualResetEventSlim mPauseCheck;
        /// <summary>
        /// This is the setting policy for the task manager.
        /// </summary>
        private TaskManagerPolicy mPolicy;
        /// <summary>
        /// This is the CpuStats holder. It is used to report statistics and to hold trigger autotune events.
        /// </summary>
        private CpuStats mCpuStats = new CpuStats();

        protected TaskAvailability mAvailability;

        //public class AutoTuneStats
        //{
        //    public int TasksMaxConcurrent { get; set; } = 0;

        //    public int TasksMinConcurrent { get; set; } = 0;

        //    public int OverloadTasksConcurrent { get; set; } = 0;

        //    public long ProcessorCurrentMissCount = 0;

        //    public DateTime? LastProcessorTime { get; set; } = null;
        //}

        //private AutoTuneStats mAutotuneStats = new AutoTuneStats();

        /// <summary>
        /// This is the priority task queue.
        /// </summary>
        private QueueTrackerContainer<QueueTracker> mTasksQueue;
        /// <summary>
        /// This dictionary holds active jobs.
        /// </summary>
        private ConcurrentDictionary<Guid, TaskTracker> mTaskRequests;
        /// <summary>
        /// This function contains a reference to the Dispather which is used to process a task.
        /// </summary>
        private Func<TransmissionPayload, Task> Dispatcher;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public TaskManager(int levels
            , Func<TransmissionPayload, Task> dispatcher
            , TaskManagerPolicy policy = null
            ) : base(nameof(TaskManager))
        {
            if (dispatcher == null)
                throw new ArgumentNullException($"{nameof(TaskManager)}: dispatcher can not be null");

            mPolicy = policy ?? new TaskManagerPolicy();

            mPauseCheck = new ManualResetEventSlim();

            mAvailability = new TaskAvailability(levels, policy.ConcurrentRequestsMax);

            mTasksQueue = new QueueTrackerContainer<QueueTracker>(levels);

            mProcesses = new ConcurrentDictionary<string, TaskManagerProcessContext>();

            Dispatcher = dispatcher;

            if (!mPolicy.ProcessKillOverrunGracePeriod.HasValue)
                mPolicy.ProcessKillOverrunGracePeriod = TimeSpan.FromSeconds(15);

            mTaskRequests = new ConcurrentDictionary<Guid, TaskTracker>();
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override starts the message pump.
        /// </summary>
        protected override void StartInternal()
        {
            mMessagePump = new Thread(new ParameterizedThreadStart(ProcessLoop));

            LoopReset();

            mMessagePump.Start();
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override stops the message pump.
        /// </summary>
        protected override void StopInternal()
        {
            MessagePumpActive = false;

            LoopSet();

            mMessagePump.Join();
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method sets the statistics.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            try
            {
                mStatistics.Cpu = mCpuStats;

                mStatistics.AutotuneActive = mPolicy.AutotuneEnabled;

                mStatistics.Availability = mAvailability.Statistics;

                mStatistics.TaskCount = mTaskRequests?.Count ?? 0;

                if (mTaskRequests != null)
                    mStatistics.Running = mTaskRequests.Values
                        .Where((t) => t.ProcessSlot.HasValue)
                        .OrderBy((t) => t.ProcessSlot.Value)
                        .Select((t) => t.Debug)
                        .ToArray();

                if (mTasksQueue != null) mStatistics.Queues = mTasksQueue.Statistics;

            }
            catch (Exception ex)
            {
                mStatistics.Ex = ex;
            }
        }
        #endregion

        #region --> ExecuteOrEnqueue(ServiceBase service, TransmissionPayload payload)
        /// <summary>
        /// This method takes incoming messages from the initiators. It is the method set on the Dispatcher property.
        /// </summary>
        /// <param name="service">The calling service.</param>
        /// <param name="payload">The payload to process.</param>
        public virtual void ExecuteOrEnqueue(IService service, TransmissionPayload payload)
        {
            ExecuteOrEnqueue(payload, service.GetType().Name);
        }
        /// <summary>
        /// This method takes incoming messages from the initiators.
        /// </summary>
        /// <param name="payload">The payload to process.</param>
        /// <param name="callerName">This is the name of the calling party. It is primarily used for debug and trace reasons.</param>
        public virtual void ExecuteOrEnqueue(TransmissionPayload payload, string callerName)
        {
            TaskTracker tracker = TrackerCreateFromPayload(payload, callerName);
            ExecuteOrEnqueue(tracker);
        }
        #endregion

        #region TrackerCreateFromPayload(TransmissionPayload payload, string caller)
        /// <summary>
        /// This private method builds the payload consistently for the incoming payload.
        /// </summary>
        /// <param name="payload">The payload to add to a tracker.</param>
        /// <param name="caller">The caller reference.</param>
        /// <returns>Returns a tracker of type payload.</returns>
        public static TaskTracker TrackerCreateFromPayload(TransmissionPayload payload, string caller)
        {
            if (payload == null || payload.Message == null)
                throw new ArgumentNullException("Payload or Payload message cannot be null.");

            int priority = payload.Message.ChannelPriority;

            return new TaskTracker(TaskTrackerType.Payload, payload.MaxProcessingTime)
            {
                IsLongRunning = false,
                Priority = priority,
                Name = payload.Message.ToKey(),
                Context = payload,
                Caller = caller
            };
        }
        #endregion

        #region ProcessRegister(string name, int priority, Action execute)
        /// <summary>
        /// This method registers a process to be polled as part of the process loop.
        /// </summary>
        /// <param name="name">The process name. If this is already used, then it will be replaced.</param>
        /// <param name="ordinal">The order the registered processes are polled higher is first.</param>
        /// <param name="execute">The execute action.</param>
        public void ProcessRegister(string name, int ordinal, ITaskManagerProcess process)
        {
            ProcessRegister<object>(name, ordinal, process);
        }

        public void ProcessRegister<C>(string name, int ordinal, ITaskManagerProcess process, C context = default(C))
        {
            var holder = new TaskManagerProcessContext<C>(name) { Ordinal = ordinal, Process = process, Context = context };
            process.Submit = ExecuteOrEnqueue;
            mProcesses.AddOrUpdate(name, holder, (n, o) => holder);
        }
        #endregion
        #region ProcessUnregister(string name)
        /// <summary>
        /// This method removes a process from the Process collection.
        /// </summary>
        /// <param name="name">The process name.</param>
        public void ProcessUnregister(string name)
        {
            TaskManagerProcessContext value;
            mProcesses.TryRemove(name, out value);
            value.Process.Submit = null;
        }
        #endregion
        #region ProcessExecute(ProcessHolder process)
        /// <summary>
        /// This method executes a particular process syncronously, and catches any exceptions that might be thrown.
        /// </summary>
        /// <param name="context">The process to execute.</param>
        private void ProcessExecute(TaskManagerProcessContext context)
        {
            try
            {
                if (context.Process.CanProcess())
                    context.Process.Process(mAvailability);
            }
            catch (Exception ex)
            {
                Logger?.LogException($"ProcessExecute failed: {context.Name}", ex);
            }
        }
        #endregion

        #region ProcessLoop(object state)
        /// <summary>
        /// This is the message loop that receives messages.
        /// </summary>
        /// <param name="state">This is not used.</param>
        private void ProcessLoop(object state)
        {
            MessagePumpActive = true;

            try
            {
                //Loop to infinity until an exception is called for the thread or MessagePumpActive is set to false.
                while (MessagePumpActive)
                {
                    //Pause 50ms if set or until another process signals continue.
                    LoopPause();

                    try
                    {

                        //Reset the reset event to pause the thread.
                        //However this will be set if any new tasks are enqueued in the following code.
                        LoopReset();

                        //Timeout any overdue tasks.
                        TaskTimedoutCancel();

                        //Process waiting messages.
                        DequeueTasksAndExecute();

                        //Execute any registered processes
                        mProcesses.Values
                            .OrderByDescending((p) => p.Ordinal)
                            .ForEach((p) => ProcessExecute(p));
                    }
                    catch (Exception tex)
                    {
                        Logger?.LogException("ProcessLoop unhandled exception", tex);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                //OK, we're shutting down.
                MessagePumpActive = false;
                //LogMessage(LoggingLevel.Info, "Thread aborting, shutting down", "Messaging");
            }
            catch (Exception ex)
            {
                Logger?.LogException("TaskManager (Unhandled)", ex);
            }
            finally
            {
                //Cancel any remaining tasks as the loop is leaving.
                TasksCancel();
            }
        }
        #endregion

        #region Loop methods
        private void LoopPause()
        {
            mPauseCheck.Wait(mPolicy.LoopPauseTimeInMs);
        }

        private void LoopReset()
        {
            mPauseCheck.Reset();
        }

        private void LoopSet()
        {
            mPauseCheck.Set();
        }
        #endregion

        #region BulkheadReserve(int level, int slotCount)
        /// <summary>
        /// This method reserves slots for particular priority levels.
        /// This ensures that long running lower priority processes do not starve the system of the ability to process
        /// higher priority requests.
        /// </summary>
        /// <param name="level">The level to reserve.</param>
        /// <param name="slotCount">The slot count. Set this to zero if you wish to clear the reserve.</param>
        /// <returns>Returns true if the reservation has been set.</returns>
        public bool BulkheadReserve(int level, int slotCount)
        {
            return mAvailability.BulkheadReserve(level, slotCount);
        }
        #endregion

        #region --> ExecuteOrEnqueue(TaskTracker tracker)
        /// <summary>
        /// This method is used to execute a tracker on enqueue until there are task slots available.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        public void ExecuteOrEnqueue(TaskTracker tracker)
        {
            if (tracker.IsInternal)
            {
                ExecuteTask(tracker);
                return;
            }

            mTasksQueue.Enqueue(tracker);

            DequeueTasksAndExecute();
        }
        #endregion
        #region --> DequeueTasksAndExecute()
        /// <summary>
        /// This method processes the tasks that resides in the queue, dequeuing the highest priority first.
        /// </summary>
        public void DequeueTasksAndExecute()
        {
            try
            {
                foreach (var dequeueTask in mTasksQueue.Dequeue(mAvailability.Count))
                    ExecuteTask(dequeueTask);
            }
            catch (Exception ex)
            {
                Logger?.LogException("DequeueTasksAndExecute", ex);
            }
        }
        #endregion

        #region ExecuteTask(TaskTracker tracker)
        /// <summary>
        /// This method sets the task on to a Task for execution and calls the end method on completion.
        /// </summary>
        /// <param name="tracker">The tracker to enqueue.</param>
        private void ExecuteTask(TaskTracker tracker)
        {
            var tOpts = tracker.IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None;

            if (mTaskRequests.TryAdd(tracker.Id, tracker))
            {
                mAvailability.Increment(tracker);

                tracker.UTCExecute = DateTime.UtcNow;

                try
                {
                    var task = Task.Factory.StartNew
                    (
                          async () => await ExecuteTaskCreate(tracker), tracker.Cts.Token, tOpts, TaskScheduler.Default
                    ).Unwrap();

                    task.ContinueWith((t) => ExecuteTaskComplete(tracker, task.IsCanceled || task.IsFaulted, t.Exception));
                }
                catch (Exception ex)
                {
                    ExecuteTaskComplete(tracker, true, ex);
                }
            }
            else
                Logger?.LogMessage(LoggingLevel.Error, $"Task could not be enqueued: {tracker.Id}-{tracker.Caller}");
        }
        #endregion
        #region ExecuteTaskCreate(TaskTracker tracker)
        /// <summary>
        /// This method creates the necessary task.
        /// </summary>
        /// <param name="tracker">The tracker to create a task.</param>
        /// <returns>Returns a task for the job.</returns>
        private Task ExecuteTaskCreate(TaskTracker tracker)
        {
            //Internal tasks should not block other incoming tasks and they are passthrough requests from another task.
            tracker.ExecuteTickCount = mStatistics.ActiveIncrement();

            var payload = tracker.Context as TransmissionPayload;
            if (payload != null)
            {
                payload.Cancel = tracker.Cts.Token;
                tracker.ExecuteTask = Dispatcher(payload);
            }
            else
                tracker.ExecuteTask = tracker.Execute(tracker.Cts.Token);

            return tracker.ExecuteTask;
        }
        #endregion
        #region ExecuteTaskComplete(TaskTracker tracker, bool failed, Exception tex)
        /// <summary>
        /// This method is called after a task has completed.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <param name="failed">A boolean value indicating whether the task has failed.</param>
        /// <param name="tex">Any exception that was caught as the task executed.</param>
        private void ExecuteTaskComplete(TaskTracker tracker, bool failed, Exception tex)
        {
            try
            {
                TaskTracker outTracker;
                if (mTaskRequests.TryRemove(tracker.Id, out outTracker))
                {
                    mAvailability.Decrement(outTracker);

                    try
                    {
                        if (outTracker.ExecuteComplete != null)
                            outTracker.ExecuteComplete(tracker, failed, tex);
                    }
                    catch (Exception ex)
                    {
                        //We shouldn't throw an exception here, but let's check just in case.
                        Logger?.LogException("ExecuteTaskComplete/ExecuteComplete", ex);
                    }

                    if (outTracker.ExecuteTickCount.HasValue)
                        mStatistics.ActiveDecrement(outTracker.ExecuteTickCount.Value);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogException($"Task {tracker.Id} has faulted when completing: {ex.Message}", ex);
            }

            //Check if there are any queued tasks that can fill up the empty slot that is now available.
            try
            {
                //Signal the poll loop to proceed in case it is waiting, this will check for any pending tasks that require processing.
                LoopSet();
            }
            catch (Exception)
            {
                //We do not want to throw an exception here.
            }

            try
            {
                if (failed)
                {
                    mStatistics.ErrorIncrement();

                    if (tex != null && tex is AggregateException)
                        foreach (Exception ex in ((AggregateException)tex).InnerExceptions)
                            Logger?.LogException(string.Format("Task exception {0}-{1}", tracker.Id, tracker.Caller), ex);
                }
            }
            catch { }
        }
        #endregion


        #region --> TaskTimedoutKill()
        /// <summary>
        /// This method is used to kill the overrun tasks when it has exceeded the configured cancel time.
        /// </summary>
        /// <returns></returns>
        public async Task TaskTimedoutKill()
        {
            if (mTaskRequests.IsEmpty)
                return;

            var cancelled = mTaskRequests.Values
                .Where((t) => t.IsCancelled)
                .ToList();

            if (cancelled != null && cancelled.Count > 0)
            {
                cancelled
                    .Where((t) => t.CancelledTime.HasValue && ((DateTime.UtcNow - t.CancelledTime.Value) > mPolicy.ProcessKillOverrunGracePeriod))
                    .ForEach((t) => TaskKill(t));
            }
        }
        #endregion
        #region --> TaskTimedoutCancel()
        /// <summary>
        /// This method stops all timedout tasks.
        /// </summary>
        public void TaskTimedoutCancel()
        {
            if (mTaskRequests.IsEmpty)
                return;

            var expired = mTaskRequests.Values
                .Where((t) => t.HasExpired)
                .ToList();

            if (expired != null && expired.Count > 0)
            {
                expired.ForEach((t) => TaskCancel(t));
            }
        }
        #endregion
        #region --> TasksCancel()
        /// <summary>
        /// This method stops all current jobs.
        /// </summary>
        public void TasksCancel()
        {
            if (mTaskRequests.IsEmpty)
                return;

            var expired = mTaskRequests.Values.ToList();

            if (expired.Count > 0)
                expired.ForEach((t) => TaskCancel(t));
        }
        #endregion

        #region TaskCancel(TaskTracker tracker)
        /// <summary>
        /// This method cancels the specific tracker.
        /// </summary>
        /// <param name="tracker">The tracker to cancel.</param>
        private void TaskCancel(TaskTracker tracker)
        {
            if (tracker.IsCancelled)
                return;

            try
            {
                mStatistics.TimeoutRegister(1);
                tracker.Cancel();
            }
            catch (Exception ex)
            {
                Logger?.LogException("TaskCancel exception", ex);
            }
        }
        #endregion
        #region TaskKill(TaskTracker tracker)
        private object mSyncKill = new object();
        /// <summary>
        /// This process marks a process a killed.
        /// </summary>
        /// <param name="tracker">The tracker itself.</param>
        private void TaskKill(TaskTracker tracker)
        {
            if (tracker.IsKilled)
                return;

            lock (mSyncKill)
            {
                if (tracker.IsKilled)
                    return;

                tracker.IsKilled = true;

                mAvailability.Decrement(tracker, true);
            }
        }
        #endregion

        #region Autotune()
        /// <summary>
        /// This method is used to reduce or increase the processes currently active based on the target CPU percentage.
        /// </summary>
        public virtual async Task Autotune()
        {
            try
            {
                float? current = await mCpuStats.SystemProcessorUsagePercentage(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

                //if (!current.HasValue && mAutotuneStats.ProcessorCurrentMissCount < 5)
                //{
                //    Interlocked.Increment(ref mAutotuneStats.ProcessorCurrentMissCount);
                //    return;
                //}

                //if (!mPolicy.AutotuneEnabled)
                //    return;

                //mAutotuneStats.ProcessorCurrentMissCount = 0;
                //float processpercentage = current.HasValue ? (float)current.Value : 0.01F;

                ////Do we need to scale down
                //if ((processpercentage > mPolicy.ProcessorTargetLevelPercentage)
                //    || (mAutotuneStats.TasksMaxConcurrent > mPolicy.ConcurrentRequestsMin))
                //{
                //    Interlocked.Decrement(ref mAutotuneStats.TasksMaxConcurrent);
                //    if (mAutotuneStats.TasksMaxConcurrent < 0)
                //        mAutotuneStats.TasksMaxConcurrent = 0;
                //}
                ////Do we need to scale up
                //if ((processpercentage <= mPolicy.ProcessorTargetLevelPercentage)
                //    && (mAutotuneStats.TasksMaxConcurrent < mPolicy.ConcurrentRequestsMax))
                //{
                //    Interlocked.Increment(ref mAutotuneStats.TasksMaxConcurrent);
                //    if (mAutotuneStats.TasksMaxConcurrent > mPolicy.ConcurrentRequestsMax)
                //        mAutotuneStats.TasksMaxConcurrent = mPolicy.ConcurrentRequestsMax;
                //}

                //int AutotuneOverloadTasksCurrent = mAutotuneStats.TasksMaxConcurrent / 10;

                //if (AutotuneOverloadTasksCurrent > mPolicy.OverloadProcessLimitMax)
                //{
                //    mAutotuneStats.OverloadTasksConcurrent = mPolicy.OverloadProcessLimitMax;
                //}
                //else if (AutotuneOverloadTasksCurrent < mPolicy.OverloadProcessLimitMin)
                //{
                //    mAutotuneStats.OverloadTasksConcurrent = mPolicy.OverloadProcessLimitMin;
                //}
                //else
                //{
                //    mAutotuneStats.OverloadTasksConcurrent = AutotuneOverloadTasksCurrent;
                //}
            }
            catch (Exception ex)
            {
                //Autotune should not throw an exceptions
                Logger?.LogException("Autotune threw an exception.", ex);
            }
        }
        #endregion

        #region Logger
        /// <summary>
        /// This is the system wide logger reference.
        /// </summary>
        public ILoggerExtended Logger
        {
            get; set;
        }
        #endregion


    }
}
