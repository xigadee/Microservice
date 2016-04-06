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
    public partial class Microservice
    {
        #region Declarations
        /// <summary>
        /// This class contains the running tasks and provides a breakdown of the current availability for new tasks.
        /// </summary>
        private TaskManager mTaskContainer;
        /// <summary>
        /// This is the scheduler container.
        /// </summary>
        private SchedulerContainer mScheduler;
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
        /// <param name="payload">The payload to process.</param>
        /// <param name="callerName">This is the name of the calling party. It is primarily used for debug and trace reasons.</param>
        protected virtual void ExecuteOrEnqueue(TransmissionPayload payload, string callerName)
        {
            TaskTracker tracker = TrackerCreateFromPayload(payload, callerName);
            mTaskContainer.ExecuteOrEnqueue(tracker);
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
        /// <param name="schedule">The schedule to add to a tracker.</param>
        /// <returns>Returns a tracker of type payload.</returns>
        private TaskTracker TrackerCreateFromSchedule(Schedule schedule)
        {
            TaskTracker tracker = new TaskTracker(TaskTrackerType.Schedule, null);
            tracker.IsLongRunning = schedule.IsLongRunning;

            if (schedule.IsInternal)
                tracker.Priority = TaskTracker.PriorityInternal;
            else
                tracker.Priority = 2;

            tracker.Context = schedule;
            tracker.Name = schedule.Name;

            return tracker;
        }
        #endregion
        #region TrackerCreateFromListenerContext(HolderSlotContext context)
        /// <summary>
        /// This private method builds the payload consistently for the incoming payload.
        /// </summary>
        /// <param name="context">The client holder context.</param>
        /// <returns>Returns a tracker of type payload.</returns>
        private TaskTracker TrackerCreateFromListenerContext(HolderSlotContext context, int? priority = null)
        {
            TaskTracker tracker = new TaskTracker(TaskTrackerType.ListenerPoll, TimeSpan.FromSeconds(30))
            {
                Priority = priority ?? TaskTracker.PriorityInternal,
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

        #region TaskManagerInitialise()
        /// <summary>
        /// This method initialises the process loop components.
        /// </summary>
        protected virtual void TaskManagerInitialise()
        {
            mTaskContainer = InitialiseTaskManager();

            mScheduler = InitialiseSchedulerContainer();
        }
        #endregion

        #region TaskManagerStart()
        /// <summary>
        /// This method starts the processing process loop.
        /// </summary>
        protected virtual void TaskManagerStart()
        {
            TaskManagerProcessRegister();

            mTaskContainer.Start();
        }
        #endregion
        #region TaskManagerProcessRegister()
        /// <summary>
        /// 
        /// </summary>
        protected virtual void TaskManagerProcessRegister()
        {
            mTaskContainer.ProcessRegister("SchedulesProcess", mTaskContainer.LevelMax + 1, SchedulesProcess);

            for (int l = mTaskContainer.LevelMin; l <= mTaskContainer.LevelMax; l++)
            {
                mTaskContainer.ProcessRegister($"ListenersProcess: {l}", l, () => ListenersProcess(l));
            }

            mTaskContainer.ProcessRegister("OverloadCheck", mTaskContainer.LevelMin - 1, OverloadCheck);
        } 
        #endregion
        #region TaskManagerStop()
        /// <summary>
        /// This method stops the process loop.
        /// </summary>
        protected virtual void TaskManagerStop()
        {
            mTaskContainer.Stop();
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
            if (!process.Overloaded)// || process.OverloadProcessCount >= mAutotuneOverloadTasksConcurrent)
                return;

            TaskTracker tracker = new TaskTracker(TaskTrackerType.Overload, null);
            tracker.Name = process.GetType().Name;
            tracker.Caller = process.GetType().Name;
            tracker.IsLongRunning = true;
            tracker.Priority = 3;
            tracker.Execute = async (token) => await process.OverloadProcess(ConfigurationOptions.OverloadProcessTimeInMs);

            mTaskContainer.ExecuteOrEnqueue(tracker);
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

                mTaskContainer.ExecuteOrEnqueue(tracker);
            }
        }
        #endregion
        #region ScheduleComplete(Task task, TaskTracker tracker)
        /// <summary>
        /// This method is used to complete a schedule request and to 
        /// recalculate the next schedule.
        /// </summary>
        /// <param name="tracker">The tracker object.</param>
        private void ScheduleComplete(TaskTracker tracker, bool failed, Exception ex)
        {
            var schedule = tracker.Context as Schedule;

            schedule.Complete(!failed, isException:failed, lastEx: ex, exceptionTime: DateTime.UtcNow);
        }
        #endregion

        #region ListenersProcess(int priorityLevel)
        /// <summary>
        /// This method is used to create tasks to dequeue data from the listeners.
        /// </summary>
        /// <param name="priorityLevel">This property specifies the priority level that should be processed..</param>
        private void ListenersProcess(int priorityLevel)
        {
            bool singleHop = false;
            //Ok, we don't receive any messages until the system is running.
            if (Status != ServiceStatus.Running)
                return;

            HolderSlotContext context;

            int listenerTaskSlotsAvailable = mTaskContainer.TaskSlotsAvailableNet(priorityLevel);

            while (listenerTaskSlotsAvailable > 0
                && mCommunication.ListenerClientNext(priorityLevel, listenerTaskSlotsAvailable, out context))
            {
                TaskTracker tracker = TrackerCreateFromListenerContext(context, priorityLevel);

                mTaskContainer.ExecuteOrEnqueue(tracker);

                if (singleHop)
                    break;

                listenerTaskSlotsAvailable = mTaskContainer.TaskSlotsAvailableNet(priorityLevel);
            }
        }
        #endregion
        #region ListenerProcessClientPayloads(ListenerClient container, TransmissionPayload payload)
        /// <summary>
        /// This method processes the individual payload and passes it to be exectued.
        /// </summary>
        /// <param name="clientId">The clientId that has been polled.</param>
        /// <param name="payloads">The payloads to process.</param>
        private void ListenerProcessClientPayloads(Guid clientId, List<TransmissionPayload> payloads)
        {
            if (payloads == null || payloads.Count == 0)
                return;

            foreach (var payload in payloads)
                ListenerProcessClientPayload(clientId, payload);
        }
        #endregion
        #region ListenerProcessClientPayload(ClientHolder client, TransmissionPayload payload)
        /// <summary>
        /// This method processes an individual payload returned from a client.
        /// </summary>
        /// <param name="clientId">The originating client.</param>
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

                mTaskContainer.ExecuteOrEnqueue(tracker);
            }
            catch (Exception ex)
            {
                mLogger.LogException(string.Format("ProcessClientPayload: unhandled error {0}/{1}-{2}", payload.Source, payload.Message.CorrelationKey, payload), ex);
                payload.SignalFail();
            }

        }
        #endregion

    }
}
