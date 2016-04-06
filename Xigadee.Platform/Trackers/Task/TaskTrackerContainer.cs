using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ProcessHolder
    {
        public int Priority { get; set; }

        public Action Execute { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// This container holds the current tasks being processed on the system and calculates the availabile slots for the supported priority levels.
    /// </summary>
    public class TaskTrackerContainer: ServiceBase<TaskTrackerStatistics>
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
        private ManualResetEventSlim mPauseCheck = new ManualResetEventSlim();

        private TaskTrackerPolicy mPolicy;

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

        private Func<TransmissionPayload, Task> Dispatcher;

        private int mLevels;

        private int mTasksMaxConcurrent;
        /// <summary>
        /// This is the current count of the internal running tasks.
        /// </summary>
        private int mTasksInternal = 0;

        private int[] mTasksActive;

        private int[] mTasksBulkheadReservation;

        private int mLoopPauseTimeInMs = 200;

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
        public TaskTrackerContainer(int levels
            , Func<TransmissionPayload, Task> dispatcher, int tasksMaxConcurrent
            , TimeSpan? processKillOverrunGracePeriod = null
            , TaskTrackerPolicy policy = null
            ) : base(nameof(TaskTrackerContainer))
        {
            if (dispatcher == null)
                throw new ArgumentNullException($"{nameof(TaskTrackerContainer)}: dispatcher can not be null");

            mPolicy = policy ?? new TaskTrackerPolicy();

            mLevels = levels;
            mTasksActive = new int[levels];
            mTasksBulkheadReservation = new int[levels];
            mTasksQueue = new QueueTrackerContainer<QueueTracker>(levels);

            mProcesses = new ConcurrentDictionary<string, ProcessHolder>();

            TasksMaxConcurrent = tasksMaxConcurrent;

            Dispatcher = dispatcher;

            mProcessKillOverrunGracePeriod = processKillOverrunGracePeriod ?? TimeSpan.FromSeconds(15);

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

        public void ProcessUnregister(string name)
        {
            ProcessHolder value;
            mProcesses.TryRemove(name, out value);
        }

        public void ProcessRegister(string name, int priority, Action execute)
        {
            var process = new ProcessHolder() { Priority = priority, Name = name, Execute = execute };

            mProcesses.AddOrUpdate(name, process, (n,o) => process);
        }

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

        #region -->ProcessLoop(object state)
        /// <summary>
        /// This is the message loop that receives messages.
        /// </summary>
        /// <param name="state">This is not used.</param>
        protected void ProcessLoop(object state)
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
                        //Timeout any overdue tasks.
                        TaskTimedoutCancel();

                        //Execute any registered processes
                        mProcesses.Values
                            .OrderByDescending((p) => p.Priority)
                            .ForEach((p) => ProcessExecute(p));

                        //Process waiting messages.
                        DequeueTasksAndExecute();

                    }
                    catch (Exception tex)
                    {
                        LogException("ProcessLoop unhandled exception", tex);
                    }

                    //Reset the reset event to pause the thread.
                    LoopReset();
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
        public void LoopPause()
        {
            mPauseCheck.Wait(mLoopPauseTimeInMs);
        }

        public void LoopReset()
        {
            mPauseCheck.Reset();
        }

        public void LoopSet()
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

                mStatistics.AutotuneActive = mPolicy.Supported;
                mStatistics.TasksMaxConcurrent = mAutotuneTasksMaxConcurrent;
                mStatistics.OverloadTasksConcurrent = mAutotuneOverloadTasksConcurrent;

                if (mTaskRequests != null)
                {
                    mStatistics.Active = mTaskRequests.Count;
                    mStatistics.SlotsAvailable = TaskSlotsAvailable;
                }

                mStatistics.Internal = mTasksInternal;

                mStatistics.Killed = mTasksKilled;
                mStatistics.KilledTotal = mTasksKilledTotal;
                mStatistics.KilledDidReturn = mTasksKilledDidReturn;

                if (mTaskRequests != null) mStatistics.Running =
                    mTaskRequests.Values
                    .Where((t) => t.ProcessSlot.HasValue)
                    .OrderByDescending((t) => t.ProcessSlot.Value)
                    .Select((t) => t.Debug)
                    .ToList();

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
            if (level < 0 || level > mLevels - 1)
                return false;

            if (slotCount < 0)
                return false;

            mTasksBulkheadReservation[level] = slotCount;

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
                    Interlocked.Increment(ref mTasksInternal);
                else
                    Interlocked.Increment(ref mTasksActive[tracker.Priority.Value]);

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
                        Interlocked.Decrement(ref mTasksInternal);
                    else
                        Interlocked.Decrement(ref mTasksActive[tracker.Priority.Value]);


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
                //While we have a thread see if there is work to enqueue.
                DequeueTasksAndExecute();

                //Signal the poll loop to proceed in case it is waiting.
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
            get { return mTasksMaxConcurrent - (mTaskRequests.Count - mTasksInternal - mTasksKilled); }
        }
        #endregion
        #region TaskSlotsAvailableNet
        /// <summary>
        /// This figure is the number of remaining task slots available after the number of queued tasks have been removed.
        /// </summary>
        public int TaskSlotsAvailableNet
        {
            get { return TaskSlotsAvailable - mTasksQueue.Count; }
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
                    throw new ArgumentOutOfRangeException($"{nameof(TaskTrackerContainer)}: TasksMaxConcurrent must be a positive integer.");

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

                if (!mPolicy.Supported)
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

    }
}
