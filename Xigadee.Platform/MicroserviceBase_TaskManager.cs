#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //TaskManager
    public partial class MicroserviceBase
    {
        #region Declarations
        /// <summary>
        /// This is the scheduler container.
        /// </summary>
        SchedulerContainer mScheduler;
        /// <summary>
        /// This queue holds pending jobs.
        /// </summary>
        private QueueTrackerContainer<QueueTracker> mTasksQueue;
        /// <summary>
        /// This dictionary holds active jobs.
        /// </summary>
        private ConcurrentDictionary<Guid, TaskTracker> mTaskRequests;
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
        /// <summary>
        /// This is the current count of the internal running tasks.
        /// </summary>
        private int mTasksInternal = 0;

        private long mProcessSlot = 0;
        /// <summary>
        /// This is the current count of the killed tasks that have been freed up because the task has failed to return.
        /// </summary>
        private int mTasksKilled = 0;

        private long mTasksKilledTotal = 0;

        private long mTasksKilledDidReturn = 0;
        #endregion

        #region Process ...
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <typeparam name="C">The message contract.</typeparam>
        /// <param name="package">The objet package to process.</param>
        /// <param name="ChannelPriority">The prioirty that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// <param name="isDeadLetterMessage">A flag indicating whether the message is a deadletter replay. These messages may be treated differently
        /// by the receiving commands.</param>
        public void Process<C>(object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , bool isDeadLetterMessage = false)
            where C : IMessageContract
        {
            string channelId, messageType, actionType;
            ServiceMessageHelper.ExtractContractInfo<C>(out channelId, out messageType, out actionType);

            Process(channelId, messageType, actionType, package, ChannelPriority, options, release, isDeadLetterMessage);
        }
        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="ChannelId">The incoming channel. This must be supplied.</param>
        /// <param name="MessageType">The message type. This may be null.</param>
        /// <param name="ActionType">The message action. This may be null.</param>
        /// <param name="package">The objet package to process.</param>
        /// <param name="ChannelPriority">The prioirty that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// <param name="isDeadLetterMessage">A flag indicating whether the message is a deadletter replay. These messages may be treated differently
        /// by the receiving commands.</param>
        public void Process(string ChannelId, string MessageType = null, string ActionType = null
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , bool isDeadLetterMessage = false)
        {
            var header = new ServiceMessageHeader(ChannelId, MessageType, ActionType);
            Process(header, package, ChannelPriority, options, release, isDeadLetterMessage);
        }

        /// <summary>
        /// This method creates a service message and injects it in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="header">The message header to identify the recipient.</param>
        /// <param name="package">The objet package to process.</param>
        /// <param name="ChannelPriority">The prioirty that the message should be processed. The default is 1. If this message is not a valid value, it will be matched to the nearest valid value.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// <param name="isDeadLetterMessage">A flag indicating whether the message is a deadletter replay. These messages may be treated differently
        /// by the receiving commands.</param>
        public void Process(ServiceMessageHeader header
            , object package = null
            , int ChannelPriority = 1
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , bool isDeadLetterMessage = false)
        {

            var message = new ServiceMessage(header);
            message.ChannelPriority = ChannelPriority;
            if (package != null)
                message.Blob = mSerializer.PayloadSerialize(package);

            Process(message, options, release, isDeadLetterMessage);
        }

        /// <summary>
        /// This method injects a service message in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <param name="options">The process options.</param>
        /// <param name="release">The release action which is called when the payload has been executed.</param>
        /// <param name="isDeadLetterMessage">A flag indicating whether the message is a deadletter replay. These messages may be treated differently
        /// by the receiving commands.</param>
        public void Process(ServiceMessage message
            , ProcessOptions options = ProcessOptions.RouteExternal | ProcessOptions.RouteInternal
            , Action<bool, Guid> release = null
            , bool isDeadLetterMessage = false)
        {
            var payload = new TransmissionPayload(message, release: release, options: options, isDeadLetterMessage: isDeadLetterMessage);

            Process(payload);
        }

        /// <summary>
        /// This method injects a payload in to the execution path and bypasses the listener infrastructure.
        /// </summary>
        /// <param name="payload">The transmission payload to execute.</param>
        public void Process(TransmissionPayload payload)
        {
            ValidateServiceStarted();

            ExecuteOrEnqueue(payload, "Incoming Process method request");
        } 
        #endregion

        #region -->ExecuteOrEnqueue(ServiceBase service, TransmissionPayload payload)
        /// <summary>
        /// This method takes incoming messages from the initiators.
        /// </summary>
        /// <param name="service">The calling service.</param>
        /// <param name="payload">The payload to process.</param>
        protected virtual void ExecuteOrEnqueue(IService service, TransmissionPayload payload)
        {
            ExecuteOrEnqueue(payload, service.GetType().Name);
        }
        /// <summary>
        /// This method takes incoming messages from the initiators.
        /// </summary>
        /// <param name="service">The calling service.</param>
        /// <param name="payload">The payload to process.</param>
        protected virtual void ExecuteOrEnqueue(TransmissionPayload payload, string callerName)
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
        /// <returns>Returns a tracker of type payload.</returns>
        private TaskTracker TrackerCreateFromPayload(TransmissionPayload payload, string caller)
        {
            if (payload == null || payload.Message == null)
                throw new ArgumentNullException("Payload or Payload message cannot be null.");

            int priority = payload.Message.ChannelPriority;

            return new TaskTracker(TaskTrackerType.Payload, payload.MaxProcessingTime)
            {
                IsLongRunning = false,
                IsInternal = priority == -1,
                Priority = priority,
                Name = payload.Message.ToKey(),
                Context = payload,
                Caller = caller
            };
        }
        #endregion
        #region TrackerCreateFromSchedule(Schedule schedule)
        /// <summary>
        /// This private method builds the payload consistently for the incoming payload.
        /// </summary>
        /// <param name="payload">The payload to add to a tracker.</param>
        /// <returns>Returns a tracker of type payload.</returns>
        private TaskTracker TrackerCreateFromSchedule(Schedule schedule)
        {
            TaskTracker tracker = new TaskTracker(TaskTrackerType.Schedule, null);
            tracker.IsLongRunning = schedule.IsLongRunning;

            if (schedule.IsInternal)
                tracker.IsInternal = true;
            else
                tracker.Priority = 2;

            tracker.Context = schedule;
            tracker.Name = schedule.Name;

            return tracker;
        }
        #endregion

        #region ExecuteOrEnqueue(TaskTracker tracker)
        /// <summary>
        /// This method is used to execute a tracker on enqueue until there are task slots available.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        protected void ExecuteOrEnqueue(TaskTracker tracker)
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

        #region --> ExecuteTask(TaskTracker tracker)
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
                mLogger.LogMessage(LoggingLevel.Error, string.Format("Task could not be enqueued: {0}/{1}", tracker.Id, tracker.Caller), "TaskManager");
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
                tracker.ExecuteTask = Execute(payload);
            }
            else
                tracker.ExecuteTask = tracker.Execute(tracker.Cts.Token);

            return tracker.ExecuteTask;
        }
        #endregion

        #region ExecuteTaskComplete(TaskTracker tracker, bool failed, Exception tex)
        /// <summary>
        /// This method is called after a task has ended.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <param name="task">The task.</param>
        private void ExecuteTaskComplete(TaskTracker tracker, bool failed, Exception tex)
        {
            OverloadCheck();

            try
            {
                TaskTracker outTracker;
                if (mTaskRequests.TryRemove(tracker.Id, out outTracker))
                {
                    //Remove the internal task count.
                    if (outTracker.IsInternal)
                        Interlocked.Decrement(ref mTasksInternal);
                    else
                        DequeueTasksAndExecute(1);

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
                        mLogger.LogException("ExecuteTaskComplete/ExecuteComplete", ex);
                    }

                    if (outTracker.ExecuteTickCount.HasValue)
                        mStatistics.ActiveDecrement(outTracker.ExecuteTickCount.Value);
                }
            }
            catch (Exception ex)
            {
                mLogger.LogException(string.Format("Task {0} has faulted when completing: {1}", tracker.Id, ex.Message), ex);
            }

            try
            {
                //While we have a thread see if there is work to enqueue.
                DequeueTasksAndExecute();

                //This method polls the next priority listener if needed.
                ListenersProcess(true);

                //Signal the loop to proceed in case it is waiting.
                mPauseCheck.Set();
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
                            mLogger.LogException(string.Format("Task exception {0}-{1}", tracker.Id, tracker.Caller), ex);
                }
            }
            catch { }
        }
        #endregion

        #region ProcessLoopInitialise()
        /// <summary>
        /// This method initialises the process loop components.
        /// </summary>
        protected virtual void ProcessLoopInitialise()
        {
            mTaskRequests = new ConcurrentDictionary<Guid, TaskTracker>();
            mScheduler = InitialiseSchedulerContainer();
            mTasksQueue = InitialiseQueueTracker();
        }
        #endregion

        #region ProcessLoopStart()
        /// <summary>
        /// This method starts the processing process loop.
        /// </summary>
        protected virtual void ProcessLoopStart()
        {
            mMessagePump = new Thread(new ParameterizedThreadStart(ProcessLoop));
            mPauseCheck.Reset();
            mMessagePump.Start();
        }
        #endregion
        #region ProcessLoopStop()
        /// <summary>
        /// This method stops the process loop.
        /// </summary>
        protected virtual void ProcessLoopStop()
        {
            MessagePumpActive = false;
            if (mPauseCheck != null)
                mPauseCheck.Set();

            if (mMessagePump != null)
                mMessagePump.Join();
        }
        #endregion

        #region TaskSlotsAvailable
        /// <summary>
        /// This figure is the number of remaining task slots available. Internal tasks are removed from the running tasks.
        /// </summary>
        public int TaskSlotsAvailable
        {
            get { return mAutotuneTasksMaxConcurrent - (mTaskRequests.Count - mTasksInternal - mTasksKilled); }
        }
        #endregion

        #region OverloadCheck...

        private void OverloadCheck()
        {
            try
            {
                //If the eventsource or logger queues are overloaded, 
                //then assign tasks to it to slow down processing of incoming or queued requests.
                OverloadCheck(mEventSource);
                OverloadCheck(mLogger);
            }
            catch (Exception ex)
            {
                mLogger.LogException("ExecuteTaskComplete/ OverloadCheck has faulted", ex);
            }
        }

        /// <summary>
        /// This method checks whether the process is overloaded and schedules a long running task to reduce the overload.
        /// </summary>
        /// <param name="process">The process to check.</param>
        protected virtual void OverloadCheck(IActionQueue process)
        {
            if (!process.Overloaded || process.OverloadProcessCount >= mAutotuneOverloadTasksConcurrent)
                return;

            TaskTracker tracker = new TaskTracker(TaskTrackerType.Overload, null);
            tracker.Name = process.GetType().Name;
            tracker.Caller = process.GetType().Name;
            tracker.IsLongRunning = true;
            tracker.Priority = 3;
            tracker.IsInternal = false;
            tracker.Execute = async (token) => await process.OverloadProcess(ConfigurationOptions.OverloadProcessTimeInMs);

            ExecuteOrEnqueue(tracker);
        }
        #endregion

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
                    mPauseCheck.Wait(50);

                    try
                    {
                        //Timeout any overdue tasks.
                        TaskTimedoutCancel();

                        //Run any pending schedules.
                        SchedulesProcess();

                        //Process waiting messages.
                        DequeueTasksAndExecute();

                        //Get new pending messages.
                        ListenersProcess();

                        //If the eventsource or logger queues are overloaded, 
                        //then assign tasks to it to slow down incoming requests.
                        OverloadCheck(mEventSource);
                        OverloadCheck(mLogger);
                    }
                    catch (Exception tex)
                    {
                        mLogger.LogException("ProcessLoop unhandled exception", tex);
                    }
                    //Reset the reset event to stop the thread.
                    mPauseCheck.Reset();
                }
            }
            catch (ThreadAbortException)
            {
                //OK, we're shutting down.
                MessagePumpActive = false;
                mLogger.LogMessage(LoggingLevel.Info, "Thread aborting, shutting down", "Messaging");
            }
            catch (Exception ex)
            {
                mLogger.LogException("TaskManager (Unhandled)", ex);
            }
            finally
            {
                //Cancel any remaining tasks.
                TasksCancel();
            }
        }
        #endregion

        #region SchedulesProcess()
        /// <summary>
        /// This method processes any outstanding schedules and created a new task.
        /// </summary>
        private void SchedulesProcess()
        {
            var items = mScheduler.Items.Where((c) => c.ShouldPoll);

            foreach (Schedule schedule in items)
            {
                if (schedule.Active)
                {
                    schedule.PollSkip();
                    continue;
                }

                schedule.Start();

                TaskTracker tracker = TrackerCreateFromSchedule(schedule);

                tracker.Execute = async (token) =>
                {
                    try
                    {
                        await schedule.Execute(token);
                    }
                    catch (Exception ex)
                    {
                        mLogger.LogException(string.Format("Schedule failed: {0}", schedule.Name), ex);
                    }
                };

                tracker.ExecuteComplete = ScheduleComplete;

                ExecuteOrEnqueue(tracker);
            }
        }
        #endregion
        #region ScheduleComplete(Task task, TaskTracker tracker)
        /// <summary>
        /// This method is used to complete a schedule request and to 
        /// recalculate the next schedule.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="tracker">The tracker object.</param>
        private void ScheduleComplete(TaskTracker tracker, bool failed, Exception ex)
        {
            var schedule = tracker.Context as Schedule;

            schedule.Complete(!failed, isException:failed, lastEx: ex, exceptionTime: DateTime.UtcNow);
        }
        #endregion

        #region DequeueTasksAndExecute(int? slots = null)
        /// <summary>
        /// This method processes the tasks that resides in the queue.
        /// </summary>
        private void DequeueTasksAndExecute(int? slots = null)
        {
            try
            {               
                slots = slots ?? TaskSlotsAvailable;
                if (slots > 0)
                    foreach(var dequeueTask in mTasksQueue.Dequeue(slots.Value))
                        ExecuteTask(dequeueTask);
            }
            catch (Exception ex)
            {
                mLogger.LogException("DequeueTasksAndExecute", ex);
            }
        }
        #endregion

        #region TaskTimedoutKill()
        /// <summary>
        /// This method is used to kill the overrun tasks when it has exceeded the configured cancel time.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task TaskTimedoutKill()
        {
            if (mTaskRequests.IsEmpty)
                return;

            var cancelled = mTaskRequests.Values
                .Where((t) => t.IsCancelled)
                .ToList();

            if (cancelled != null && cancelled.Count > 0)
            {
                cancelled
                    .Where ((t) => t.CancelledTime.HasValue && ((DateTime.UtcNow - t.CancelledTime.Value) > ConfigurationOptions.ProcessKillOverrunGracePeriod))
                    .ForEach((t) => TaskKill(t));
            }
        } 
        #endregion
        #region TaskTimedoutCancel()
        /// <summary>
        /// This method stops all timedout tasks.
        /// </summary>
        private void TaskTimedoutCancel()
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
        #region TasksCancel()
        /// <summary>
        /// This method stops all current jobs.
        /// </summary>
        private void TasksCancel()
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
                mLogger.LogException("TaskCancel exception", ex);
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

        #region ListenersProcess(bool singleHop = false)
        /// <summary>
        /// This method is used to create tasks to dequeue data from the listeners.
        /// </summary>
        /// <param name="hops">The number of tasks to run.</param>
        private void ListenersProcess(bool singleHop = false)
        {
            //Ok, we don't receive any messages until the system is running.
            if (Status != ServiceStatus.Running)
                return;

            HolderSlotContext context;

            int oldTaskSlotsAvailable = TaskSlotsAvailable - mTasksQueue.Count;

            while (mCommunication.ListenerClientNext(oldTaskSlotsAvailable, out context))
            {
                TaskTracker tracker = TrackerCreateFromListenerContext(context);

                ExecuteOrEnqueue(tracker);

                if (singleHop)
                    break;
            }
        } 
        #endregion
        #region ListenerProcessClientPayloads(ListenerClient container, TransmissionPayload payload)
        /// <summary>
        /// This method processes the individual payload and passes it to be exectued.
        /// </summary>
        /// <param name="payload">The payload to process.</param>
        private void ListenerProcessClientPayloads(Guid clientId, List<TransmissionPayload> payloads)
        {
            if (payloads == null)
                return;

            foreach (var payload in payloads)
                ListenerProcessClientPayload(clientId, payload);
        }
        #endregion
        #region ListenerProcessClientPayload(ClientHolder client, TransmissionPayload payload)
        /// <summary>
        /// This method processes an individual payload returned from a client.
        /// </summary>
        /// <param name="client">The originating client.</param>
        /// <param name="payload">The payload.</param>
        private void ListenerProcessClientPayload(Guid clientId, TransmissionPayload payload)
        {
            try
            {
                if (payload.Message.ChannelPriority < 0)
                    payload.Message.ChannelPriority = 0;

                mCommunication.QueueTimeLog(clientId, payload.Message.EnqueuedTimeUTC);
                mCommunication.ActiveIncrement(clientId);

                TaskTracker tracker = TrackerCreateFromPayload(payload, payload.Source);

                tracker.ExecuteComplete = (tr, failed, ex) =>
                {
                    try
                    {
                        var contextPayload = tr.Context as TransmissionPayload;

                        mCommunication.ActiveDecrement(clientId, tr.TickCount);

                        if (failed)
                            mCommunication.ErrorIncrement(clientId);

                        contextPayload.Signal(!failed);
                    }
                    catch (Exception exin)
                    {
                        mLogger.LogException(string.Format("Payload completion error-{0}", payload), exin);
                    }
                };

                ExecuteOrEnqueue(tracker);
            }
            catch (Exception ex)
            {
                mLogger.LogException(string.Format("ProcessClientPayload: unhandled error {0}/{1}-{2}", payload.Source, payload.Message.CorrelationKey, payload), ex);
                payload.SignalFail();
            }

        }
        #endregion

        #region TrackerCreateFromListenerContext(HolderSlotContext context)
        /// <summary>
        /// This private method builds the payload consistently for the incoming payload.
        /// </summary>
        /// <param name="payload">The payload to add to a tracker.</param>
        /// <returns>Returns a tracker of type payload.</returns>
        private TaskTracker TrackerCreateFromListenerContext(HolderSlotContext context)
        {
            TaskTracker tracker = new TaskTracker(TaskTrackerType.ListenerPoll, TimeSpan.FromSeconds(30))
            {
                IsInternal = true,
                Context = context,
                Name = context.Name
            };

            tracker.Execute = async t =>
            {
                var currentContext = ((HolderSlotContext)tracker.Context);

                var payloads = await currentContext.Poll();

                ListenerProcessClientPayloads(currentContext.Id, payloads);
            };

            tracker.ExecuteComplete = (tr, failed, ex) =>
            {
                ((HolderSlotContext)tracker.Context).Release(failed);
            };

            return tracker;
        }
        #endregion
    }
}
