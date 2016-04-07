using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{


    /// <summary>
    /// This container holds the current tasks being processed on the system and calculates the availabile slots for the supported priority levels.
    /// </summary>
    public class TaskManager: ServiceBase<TaskManagerStatistics>
    {
        #region Declarations
        private ConcurrentDictionary<string, ProcessHolder> mProcesses;
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

        private TaskManagerPolicy mPolicy;

        private CpuStats mCpuStats = new CpuStats();

        private DateTime? mAutotuneLastProcessorTime = null;

        private int mAutotuneTasksMaxConcurrent = 0;
        private int mAutotuneTasksMinConcurrent = 0;
        private int mAutotuneOverloadTasksConcurrent = 0;
        private long mAutotuneProcessorCurrentMissCount = 0;

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
        /// <summary>
        /// This is the number of priority level defined in the Task Manager.
        /// </summary>
        private int mLevels;

        private int mTasksMaxConcurrent;
        /// <summary>
        /// This is the current count of the internal running tasks.
        /// </summary>
        private TaskPrioritySettings mPriorityInternal;

        private TaskPrioritySettings[] mPriorityStatus;

        private long mProcessSlot = 0;
        /// <summary>
        /// This is the current count of the killed tasks that have been freed up because the task has failed to return.
        /// </summary>
        private int mTasksKilled = 0;

        private long mTasksKilledTotal = 0;

        private long mTasksKilledDidReturn = 0;
        /// <summary>
        /// This is the time that a process is marked as killed after it has been marked as cancelled.
        /// </summary>
        private TimeSpan mProcessKillOverrunGracePeriod;
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

            mLevels = levels;

            mPriorityInternal = new TaskPrioritySettings(TaskTracker.PriorityInternal);

            mPriorityStatus = new TaskPrioritySettings[levels];
            Enumerable.Range(0, levels).ForEach((l) => mPriorityStatus[l] = new TaskPrioritySettings(l));
            
            mTasksQueue = new QueueTrackerContainer<QueueTracker>(levels);

            mProcesses = new ConcurrentDictionary<string, ProcessHolder>();

            TasksMaxConcurrent = policy.ConcurrentRequestsMax;

            Dispatcher = dispatcher;

            mProcessKillOverrunGracePeriod = policy.ProcessKillOverrunGracePeriod ?? TimeSpan.FromSeconds(15);

            mTaskRequests = new ConcurrentDictionary<Guid, TaskTracker>();
        }
        #endregion

        #region LevelMin
        /// <summary>
        /// This is the minimum task priority level.
        /// </summary>
        public int LevelMin { get { return 0; } } 
        #endregion
        #region LevelMax
        /// <summary>
        /// This is the maximum task priority level.
        /// </summary>
        public int LevelMax { get { return mLevels - 1; } } 
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

        #region ProcessRegister(string name, int priority, Action execute)
        /// <summary>
        /// This method registers a process to be polled as part of the process loop.
        /// </summary>
        /// <param name="name">The process name. If this is already used, then it will be replaced.</param>
        /// <param name="priority">The process priority.</param>
        /// <param name="execute">The execute action.</param>
        public void ProcessRegister(string name, int priority, Action execute)
        {
            var process = new ProcessHolder() { Priority = priority, Name = name, Execute = execute };

            mProcesses.AddOrUpdate(name, process, (n, o) => process);
        }
        #endregion
        #region ProcessUnregister(string name)
        /// <summary>
        /// This method removes a process from the Process collection.
        /// </summary>
        /// <param name="name">The process name.</param>
        public void ProcessUnregister(string name)
        {
            ProcessHolder value;
            mProcesses.TryRemove(name, out value);
        }
        #endregion
        #region ProcessExecute(ProcessHolder process)
        /// <summary>
        /// This method executes a particular process syncronously, and catches any exceptions that might be thrown.
        /// </summary>
        /// <param name="process">The process to execute.</param>
        private void ProcessExecute(ProcessHolder process)
        {
            try
            {
                process.Execute();
            }
            catch (Exception ex)
            {
                LogException($"ProcessExecute failed: {process.Name}", ex);
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
                            .OrderByDescending((p) => p.Priority)
                            .ForEach((p) => ProcessExecute(p));
                    }
                    catch (Exception tex)
                    {
                        LogException("ProcessLoop unhandled exception", tex);
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
                LogException("TaskManager (Unhandled)", ex);
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
                mStatistics.TasksMaxConcurrent = mAutotuneTasksMaxConcurrent;
                mStatistics.OverloadTasksConcurrent = mAutotuneOverloadTasksConcurrent;

                if (mTaskRequests != null)
                {
                    mStatistics.Active = mTaskRequests.Count;
                    mStatistics.SlotsAvailable = TaskSlotsAvailable;
                }

                mStatistics.Killed = mTasksKilled;
                mStatistics.KilledTotal = mTasksKilledTotal;
                mStatistics.KilledDidReturn = mTasksKilledDidReturn;

                if (mPriorityStatus != null)
                    mStatistics.Levels = mPriorityStatus
                        .Union(new TaskPrioritySettings[] { mPriorityInternal })
                        .OrderByDescending((s) => s.Level)
                        .Select((s) => s.Debug)
                        .ToArray();

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

        private void LogException(string message, Exception ex = null)
        {
        }

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
            if (level < LevelMin || level > LevelMax)
                return false;

            if (slotCount < 0)
                return false;

            mPriorityStatus[level].BulkHeadReservation = slotCount;

            return true;
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
        #region --> DequeueTasksAndExecute(int? slots = null)
        /// <summary>
        /// This method processes the tasks that resides in the queue, dequeuing the highest priority first.
        /// </summary>
        public void DequeueTasksAndExecute(int? slots = null)
        {
            try
            {
                slots = slots ?? TaskSlotsAvailable;
                if (slots > 0)
                    foreach (var dequeueTask in mTasksQueue.Dequeue(slots.Value))
                        ExecuteTask(dequeueTask);
            }
            catch (Exception ex)
            {
                LogException("DequeueTasksAndExecute", ex);
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
            tracker.ProcessSlot = Interlocked.Increment(ref mProcessSlot);

            var tOpts = tracker.IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None;

            if (mTaskRequests.TryAdd(tracker.Id, tracker))
            {
                tracker.UTCExecute = DateTime.UtcNow;

                if (tracker.IsInternal)
                    Interlocked.Increment(ref mPriorityInternal.Active);
                else
                    Interlocked.Increment(ref mPriorityStatus[tracker.Priority.Value].Active);

                try
                {
                    var task = Task.Factory.StartNew(async () => await ExecuteTaskCreate(tracker)
                        , tracker.Cts.Token
                        , tOpts
                        , TaskScheduler.Default)
                    .Unwrap();

                    task.ContinueWith((t) => ExecuteTaskComplete(tracker, task.IsCanceled || task.IsFaulted, t.Exception));
                }
                catch (Exception ex)
                {
                    ExecuteTaskComplete(tracker, true, ex);
                }
            }
            else
                LogException(string.Format("Task could not be enqueued: {0}/{1}", tracker.Id, tracker.Caller));
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
                    //Remove the internal task count.
                    if (outTracker.IsInternal)
                        Interlocked.Decrement(ref mPriorityInternal.Active);
                    else
                        Interlocked.Decrement(ref mPriorityStatus[tracker.Priority.Value].Active);


                    if (tracker.IsKilled)
                    {
                        Interlocked.Decrement(ref mTasksKilled);
                        Interlocked.Increment(ref mTasksKilledDidReturn);
                    }

                    try
                    {
                        if (outTracker.ExecuteComplete != null)
                            outTracker.ExecuteComplete(tracker, failed, tex);
                    }
                    catch (Exception ex)
                    {
                        //We shouldn't throw an exception here, but let's check just in case.
                        LogException("ExecuteTaskComplete/ExecuteComplete", ex);
                    }

                    if (outTracker.ExecuteTickCount.HasValue)
                        mStatistics.ActiveDecrement(outTracker.ExecuteTickCount.Value);
                }
            }
            catch (Exception ex)
            {
                LogException(string.Format("Task {0} has faulted when completing: {1}", tracker.Id, ex.Message), ex);
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
                            LogException(string.Format("Task exception {0}-{1}", tracker.Id, tracker.Caller), ex);
                }
            }
            catch { }
        }
        #endregion

        #region TaskSlotsAvailable
        /// <summary>
        /// This figure is the number of remaining task slots available. Internal tasks are removed from the running tasks.
        /// </summary>
        public int TaskSlotsAvailable
        {
            get { return mTasksMaxConcurrent - (mTaskRequests.Count - mPriorityInternal.Active - mTasksKilled); }
        }
        #endregion
        #region TaskSlotsAvailableNet
        /// <summary>
        /// This figure is the number of remaining task slots available after the number of queued tasks have been removed.
        /// </summary>
        public int TaskSlotsAvailableNet(int priorityLevel)
        {
            return TaskSlotsAvailable - mTasksQueue.Count;
        }
        #endregion
   
        #region TasksMaxConcurrent
        /// <summary>
        /// This is the maximum number of concurrent tasks that can execure in parallel.
        /// </summary>
        public int TasksMaxConcurrent
        {
            get
            {
                return mTasksMaxConcurrent;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException($"{nameof(TaskManager)}: TasksMaxConcurrent must be a positive integer.");

                mTasksMaxConcurrent = value;
            }
        } 
        #endregion

        #region TasksActive
        /// <summary>
        /// This is a count all all the current active requests.
        /// </summary>
        public int TasksActive { get { return mTaskRequests.Count; } }
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
                    .Where((t) => t.CancelledTime.HasValue && ((DateTime.UtcNow - t.CancelledTime.Value) > mProcessKillOverrunGracePeriod))
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
                LogException("TaskCancel exception", ex);
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
                Interlocked.Increment(ref mTasksKilled);
                Interlocked.Increment(ref mTasksKilledTotal);
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

                if (!current.HasValue && mAutotuneProcessorCurrentMissCount < 5)
                {
                    Interlocked.Increment(ref mAutotuneProcessorCurrentMissCount);
                    return;
                }

                if (!mPolicy.AutotuneEnabled)
                    return;

                mAutotuneProcessorCurrentMissCount = 0;
                float processpercentage = current.HasValue ? (float)current.Value : 0.01F;

                //Do we need to scale down
                if ((processpercentage > mPolicy.ProcessorTargetLevelPercentage)
                    || (mAutotuneTasksMaxConcurrent > mPolicy.ConcurrentRequestsMin))
                {
                    Interlocked.Decrement(ref mAutotuneTasksMaxConcurrent);
                    if (mAutotuneTasksMaxConcurrent < 0)
                        mAutotuneTasksMaxConcurrent = 0;
                }
                //Do we need to scale up
                if ((processpercentage <= mPolicy.ProcessorTargetLevelPercentage)
                    && (mAutotuneTasksMaxConcurrent < mPolicy.ConcurrentRequestsMax))
                {
                    Interlocked.Increment(ref mAutotuneTasksMaxConcurrent);
                    if (mAutotuneTasksMaxConcurrent > mPolicy.ConcurrentRequestsMax)
                        mAutotuneTasksMaxConcurrent = mPolicy.ConcurrentRequestsMax;
                }

                int AutotuneOverloadTasksCurrent = mAutotuneTasksMaxConcurrent / 10;

                if (AutotuneOverloadTasksCurrent > mPolicy.OverloadProcessLimitMax)
                {
                    mAutotuneOverloadTasksConcurrent = mPolicy.OverloadProcessLimitMax;
                }
                else if (AutotuneOverloadTasksCurrent < mPolicy.OverloadProcessLimitMin)
                {
                    mAutotuneOverloadTasksConcurrent = mPolicy.OverloadProcessLimitMin;
                }
                else
                {
                    mAutotuneOverloadTasksConcurrent = AutotuneOverloadTasksCurrent;
                }
            }
            catch (Exception ex)
            {
                //Autotune should not throw an exceptions
                LogException("Autotune threw an exception.", ex);
            }
        }
        #endregion

        protected class ProcessHolder
        {
            public int Priority { get; set; }

            public Action Execute { get; set; }

            public string Name { get; set; }
        }

        /// <summary>
        /// This class is used to hold the priority settings.
        /// </summary>
        [DebuggerDisplay("{Debug}")]
        protected class TaskPrioritySettings
        {
            public TaskPrioritySettings(int level)
            {
                Level = level;
            }

            public readonly int Level;

            public long Count;

            public int Active;

            public int BulkHeadReservation;

            public string Debug
            {
                get
                {
                    return $"Level={Level} Hits={Count} Active={Active} Reserved={BulkHeadReservation}";
                }
            }
        }
    }
}
