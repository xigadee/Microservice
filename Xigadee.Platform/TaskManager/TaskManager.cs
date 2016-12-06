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
    public class TaskManager: ServiceContainerBase<TaskManagerStatistics, TaskManagerPolicy>, IRequireDataCollector
    {
        #region Declarations
        /// <summary>
        /// This collection holds the internal request jobs.
        /// </summary>
        private ConcurrentQueue<TaskTracker> mProcessInternalQueue;
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
        /// This is the CpuStats holder. It is used to report statistics and to hold trigger autotune events.
        /// </summary>
        private CpuStats mCpuStats = new CpuStats();
        /// <summary>
        /// This is the availability holder.
        /// </summary>
        protected TaskAvailability mAvailability;
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
        /// This event can be attached to verify a task before it is executed.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskBefore;
        /// <summary>
        /// This event can be attached to identify a failure to enqueue a task.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskBeforeEnqueueFailed;
        /// <summary>
        /// This event can be attached to identify a failure to enqueue a task.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskBeforeException;
        /// <summary>
        /// This event can be attached to record an event after is has executed successfully.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskCompleteSuccess;
        /// <summary>
        /// This event can be attached to diagnose an event after is has failed during execution.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskCompleteFailure;
        /// <summary>
        /// This event can be attached to diagnose an orphan event that has already left the collection after being returned.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskCompleteOrphan;
        /// <summary>
        /// This event is called after a task has been cancelled.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskKilled;
        /// <summary>
        /// This event is called after a task has been killed.
        /// </summary>
        public event EventHandler<TaskTracker> DiagnosticOnExecuteTaskCancelled;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the constructor for the task manager.
        /// </summary>
        /// <param name="dispatcher">The dispatcher function that is used to process the specific tasks.</param>
        /// <param name="policy">The task manager policy.</param>
        public TaskManager(Func<TransmissionPayload, Task> dispatcher, TaskManagerPolicy policy)
            : base(policy, nameof(TaskManager))
        {
            if (policy == null)
                throw new ArgumentNullException($"{nameof(TaskManager)}: policy can not be null");

            if (dispatcher == null)
                throw new ArgumentNullException($"{nameof(TaskManager)}: dispatcher can not be null");

            mPauseCheck = new ManualResetEventSlim();

            mAvailability = new TaskAvailability(policy.PriorityLevels, policy.ConcurrentRequestsMax);

            mTasksQueue = new QueueTrackerContainer<QueueTracker>(policy.PriorityLevels);

            mProcessInternalQueue = new ConcurrentQueue<TaskTracker>();

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
        protected override void StatisticsRecalculate(TaskManagerStatistics stats)
        {
            base.StatisticsRecalculate(stats);

            stats.Cpu = mCpuStats;

            stats.InternalQueueLength = mProcessInternalQueue?.Count;

            stats.AutotuneActive = mPolicy.AutotuneEnabled;

            stats.Availability = mAvailability.Statistics;

            stats.TaskCount = mTaskRequests?.Count ?? 0;

            if (mTaskRequests != null)
                stats.Running = mTaskRequests.Values
                    .Where((t) => t.ProcessSlot.HasValue)
                    .OrderBy((t) => t.ProcessSlot.Value)
                    .Select((t) => t.Debug)
                    .ToArray();

            if (mTasksQueue != null) stats.Queues = mTasksQueue.Statistics;
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
            TaskTracker tracker = TrackerCreateFromPayload(payload, service.GetType().Name);

            ICommand command = service as ICommand;
            if (command != null && command.TaskManagerTimeoutSupported)
            {
                tracker.Callback = command;
                tracker.CallbackId = payload?.Message?.OriginatorKey;
            }

            ExecuteOrEnqueue(tracker);
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
        /// <param name="process">The execute action.</param>
        public void ProcessRegister(string name, int ordinal, ITaskManagerProcess process)
        {
            ProcessRegister<object>(name, ordinal, process);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="name"></param>
        /// <param name="ordinal"></param>
        /// <param name="process"></param>
        /// <param name="context"></param>
        public void ProcessRegister<C>(string name, int ordinal, ITaskManagerProcess process, C context = default(C))
        {
            var holder = new TaskManagerProcessContext<C>(name) { Ordinal = ordinal, Process = process, Context = context };
            process.TaskSubmit = ExecuteOrEnqueue;
            process.TaskAvailability = mAvailability;
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
            value.Process.TaskSubmit = null;
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
                    context.Process.Process();
            }
            catch (Exception ex)
            {
                Collector?.LogException($"ProcessExecute failed: {context.Name}", ex);
            }
        }
        #endregion

        #region --> ProcessLoop(object state)
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
                    //Pause if set or until another process signals continue.
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

                        //Execute any additional registered processes
                        mProcesses.Values
                            .OrderByDescending((p) => p.Ordinal)
                            .ForEach((p) => ProcessExecute(p));
                    }
                    catch (Exception tex)
                    {
                        Collector?.LogException("ProcessLoop unhandled exception", tex);
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
                Collector?.LogException("TaskManager (Unhandled)", ex);
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
        public bool BulkheadReserve(int level, int slotCount, int overage = 0)
        {
            return mAvailability.BulkheadReserve(level, slotCount, overage);
        }
        #endregion

        #region --> ExecuteOrEnqueue(TaskTracker tracker)
        /// <summary>
        /// This method is used to execute a tracker on enqueue until there are task slots available.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        public void ExecuteOrEnqueue(TaskTracker tracker)
        {
            //This is where the security validation should be added.

            if (tracker.IsInternal)
            {
                if (mPolicy.ExecuteInternalDirect)
                    ExecuteTask(tracker);
                else
                    mProcessInternalQueue.Enqueue(tracker);
            }
            else
                mTasksQueue.Enqueue(tracker);

            LoopSet();
        }
        #endregion
        #region --> DequeueTasksAndExecute()
        /// <summary>
        /// This method processes the tasks that resides in the queue, dequeuing the highest priority first.
        /// </summary>
        private void DequeueTasksAndExecute()
        {
            TaskTracker tracker;

            while (mProcessInternalQueue.TryDequeue(out tracker))
                ExecuteTask(tracker);

            foreach (var dequeueTask in mTasksQueue.Dequeue(mAvailability.Count))
                ExecuteTask(dequeueTask);
        }
        #endregion

        #region TaskTrackerEvent(EventHandler<TaskTracker> taskEvent, TaskTracker tracker)
        /// <summary>
        /// This helper is used to trigger diagnotic events within the Task Manager at key points in the 
        /// processing pipeline.
        /// </summary>
        /// <param name="taskEvent">The event.</param>
        /// <param name="tracker">The tracker</param>
        private void TaskTrackerEvent(EventHandler<TaskTracker> taskEvent, TaskTracker tracker)
        {
            try
            {
                taskEvent?.Invoke(this, tracker);
            }
            catch { }
        } 
        #endregion

        #region ExecuteTask(TaskTracker tracker)
        /// <summary>
        /// This method sets the task for execution and calls the end method on completion.
        /// </summary>
        /// <param name="tracker">The tracker to enqueue.</param>
        private void ExecuteTask(TaskTracker tracker)
        {
            TaskTrackerEvent(DiagnosticOnExecuteTaskBefore, tracker);

            try
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
                        )
                        .Unwrap();

                        task.ContinueWith((t) => ExecuteTaskComplete(tracker, task.IsCanceled || task.IsFaulted, t.Exception));
                    }
                    catch (Exception ex)
                    {
                        ExecuteTaskComplete(tracker, true, ex);
                        Collector?.LogException("Task creation failed.", ex);
                    }

                    return;
                }
                else
                {
                    TaskTrackerEvent(DiagnosticOnExecuteTaskBeforeEnqueueFailed, tracker);
                    Collector?.LogMessage(LoggingLevel.Error, $"Task could not be enqueued: {tracker.Id}-{tracker.Caller}");
                }
            }
            catch (Exception ex)
            {
                tracker.IsFailure = true;
                tracker.FailureException = ex;
                TaskTrackerEvent(DiagnosticOnExecuteTaskBeforeException, tracker);
                Collector?.LogException("ExecuteTask execute exception", ex);
            }

            try
            {
                tracker.Cancel();
            }
            catch (Exception ex)
            {
                Collector?.LogException("ExecuteTask unhandled exception.", ex);
            }
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
            tracker.ExecuteTickCount = StatisticsInternal.ActiveIncrement();

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
            tracker.IsFailure = failed;
            tracker.FailureException = tex;

            try
            {
                TaskTracker outTracker;
                if (mTaskRequests.TryRemove(tracker.Id, out outTracker))
                {
                    mAvailability.Decrement(outTracker);

                    try
                    {
                        outTracker.ExecuteComplete?.Invoke(outTracker, failed, tex);
                    }
                    catch (Exception ex)
                    {
                        //We shouldn't throw an exception here, but let's check just in case.
                        Collector?.LogException("ExecuteTaskComplete/ExecuteComplete", ex);
                    }

                    if (outTracker.ExecuteTickCount.HasValue)
                        StatisticsInternal.ActiveDecrement(outTracker.ExecuteTickCount.Value);
                }
                else
                {
                    tracker.IsFailure = true;
                    TaskTrackerEvent(DiagnosticOnExecuteTaskCompleteOrphan, tracker);
                }

            }
            catch (Exception ex)
            {
                Collector?.LogException($"Task {tracker.Id} has faulted when completing: {ex.Message}", ex);
            }

            try
            {
                //Signal the poll loop to proceed in case it is waiting, this will check for any pending tasks that require processing.
                LoopSet();

                if (failed)
                {
                    TaskTrackerEvent(DiagnosticOnExecuteTaskCompleteFailure, tracker);

                    StatisticsInternal.ErrorIncrement();

                    if (tex != null && tex is AggregateException)
                        foreach (Exception ex in ((AggregateException)tex).InnerExceptions)
                            Collector?.LogException(string.Format("Task exception {0}-{1}", tracker.Id, tracker.Caller), ex);
                }
                else
                    TaskTrackerEvent(DiagnosticOnExecuteTaskCompleteSuccess, tracker);
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
                StatisticsInternal.TimeoutRegister(1);
                tracker.Cancel();
                LoopSet();
            }
            catch (Exception ex)
            {
                Collector?.LogException("TaskCancel exception", ex);
            }

            TaskTrackerEvent(DiagnosticOnExecuteTaskCancelled, tracker);
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
                LoopSet();
            }

            TaskTrackerEvent(DiagnosticOnExecuteTaskKilled, tracker);
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
                Collector?.LogException("Autotune threw an exception.", ex);
            }
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the system wide data collection reference.
        /// </summary>
        public IDataCollection Collector { get; set; }
        #endregion
    }
}
