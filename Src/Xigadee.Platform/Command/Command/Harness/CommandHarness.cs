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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xigadee
{
    #region CommandHarness<C>
    /// <summary>
    /// This is the shortcut constructor for the command harness.
    /// </summary>
    /// <typeparam name="C">The command type.</typeparam>
    public class CommandHarness<C> : CommandHarness<C, CommandStatistics, CommandPolicy>
        where C : CommandBase<CommandStatistics, CommandPolicy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarness{C}"/> class.
        /// </summary>
        /// <param name="policy">The optional command policy.</param>
        /// <param name="commandCreator">This is the optional creator function to create the command.</param>
        public CommandHarness(CommandPolicy policy = null, Func<C> commandCreator = null) : base(policy, commandCreator)
        {

        }
    }
    #endregion
    #region CommandHarness<C, P>
    /// <summary>
    /// This is the shortcut constructor for the command harness.
    /// </summary>
    /// <typeparam name="C">The command type.</typeparam>
    /// <typeparam name="P">The specific policy type.</typeparam>
    public class CommandHarness<C, P> : CommandHarness<C, CommandStatistics, P>
        where C : CommandBase<CommandStatistics, P>
        where P : CommandPolicy, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHarness{C, P}"/> class.
        /// </summary>
        /// <param name="policy">The optional command policy.</param>
        /// <param name="commandCreator">This is the optional creator function to create the command.</param>
        public CommandHarness(P policy = null, Func<C> commandCreator = null) : base(policy, commandCreator)
        {

        }
    } 
    #endregion

    /// <summary>
    /// This harness is used to test command functionality independent of a Microservice.
    /// </summary>
    /// <typeparam name="C">The command type.</typeparam>
    /// <typeparam name="S">The command statistics class type.</typeparam>
    /// <typeparam name="P">The command policy.</typeparam>
    public partial class CommandHarness<C,S,P> : ServiceHarness<C, CommandHarnessDependencies>, ICommandHarness
        where C : CommandBase<S, P>
        where S : CommandStatistics, new()
        where P : CommandPolicy, new()
    {
        #region Declarations
        private long mCounter = -1;
        #endregion
        #region Events
        /// <summary>
        /// Occurs when a CommandHarnessRequest object is created.
        /// </summary>
        public event EventHandler<CommandHarnessEventArgs> OnEvent;
        /// <summary>
        /// Occurs when a request CommandHarnessRequest object is created.
        /// </summary>
        public event EventHandler<CommandHarnessEventArgs> OnEventRequest;
        /// <summary>
        /// Occurs when a response CommandHarnessRequest object is created.
        /// </summary>
        public event EventHandler<CommandHarnessEventArgs> OnEventResponse;
        /// <summary>
        /// Occurs when an outgoing CommandHarnessRequest object is created.
        /// </summary>
        public event EventHandler<CommandHarnessEventArgs> OnEventOutgoing;
        #endregion
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="policy">The optional command policy.</param>
        /// <param name="commandCreator">This is the optional creator function to create the command.</param>
        public CommandHarness(P policy = null, Func<C> commandCreator = null) 
            : base(new CommandHarnessDependencies(), commandCreator ?? CommandHarnessHelper.DefaultCreator<C,P>(policy ?? new P()))
        {
            Dispatcher = new CommandHarnessDispatchWrapper(Policy, Dependencies.ServiceHandlers, CommandExecute, () => Service?.Status ?? ServiceStatus.Stopped, true);

            //Set the harness scheduler to send the schedule execute requests here so that they can be tracked.
            Dependencies.Scheduler.TaskSubmit = ScheduleExecute;
        }
        #endregion

        #region DefaultCommand()
        /// <summary>
        /// Gets the command as its base interface..
        /// </summary>
        public ICommand DefaultCommand()
        {
            return Service;
        } 
        #endregion

        #region Policy
        /// <summary>
        /// This is the command policy.
        /// </summary>
        public P Policy => Service.Policy;
        /// <summary>
        /// Gets the command root policy.
        /// </summary>
        public CommandPolicy DefaultPolicy()
        {
            return Service.Policy;
        }
        #endregion
        #region Statistics
        /// <summary>
        /// Gets the command statistics without generating a refresh.
        /// </summary>
        public S Statistics => Service.StatisticsInternal;
        /// <summary>
        /// Gets the command root statistics.
        /// </summary>
        public CommandStatistics DefaultStatistics()
        {
            return Service.StatisticsInternal;
        }
        #endregion

        #region Traffic
        /// <summary>
        /// Gets the history of the transactions.
        /// </summary>
        public ConcurrentDictionary<long, CommandHarnessTraffic> Traffic { get; } = new ConcurrentDictionary<long, CommandHarnessTraffic>();
        #endregion
        #region TrafficFailed
        /// <summary>
        /// A list containing the failed traffic.
        /// </summary>
        public List<KeyValuePair<long,CommandHarnessTraffic>> TrafficFailed => Traffic.Where((t) => !t.Value.IsSuccess).ToList();
        #endregion

        #region TrafficPayloadRequests
        /// <summary>
        /// Contains the request messages generated by the command in the order that they were generated.
        /// </summary>
        public List<KeyValuePair<long, CommandHarnessTraffic>> TrafficPayloadRequests => Traffic.Where((t) => t.Value.Direction == CommandHarnessTrafficDirection.Request && t.Value.Tracker.HasTransmissionPayload()).ToList();
        #endregion
        #region TrafficPayloadResponses
        /// <summary>
        /// Contains the response messages generated by the command in the order that they were generated.
        /// </summary>
        public List<KeyValuePair<long, CommandHarnessTraffic>> TrafficPayloadResponses => Traffic.Where((t) => t.Value.Direction == CommandHarnessTrafficDirection.Response && t.Value.Tracker.HasTransmissionPayload()).ToList();
        #endregion
        #region TrafficPayloadOutgoing
        /// <summary>
        /// Contains the outgoing messages generated by the command in the order that they were generated.
        /// </summary>
        public List<KeyValuePair<long, CommandHarnessTraffic>> TrafficPayloadOutgoing => Traffic.Where((t) => t.Value.Direction == CommandHarnessTrafficDirection.Outgoing && t.Value.Tracker.HasTransmissionPayload()).ToList();
        #endregion

        #region RegisteredCommands
        /// <summary>
        /// Contains the set of active registered commands.
        /// </summary>
        public Dictionary<MessageFilterWrapper, bool> RegisteredCommandMethods { get; } = new Dictionary<MessageFilterWrapper, bool>();
        #endregion
        #region RegisteredSchedules
        /// <summary>
        /// Contains the set of active registered schedules.
        /// </summary>
        public Dictionary<CommandJobSchedule, bool> RegisteredSchedules { get; } = new Dictionary<CommandJobSchedule, bool>();
        #endregion

        #region Create()
        /// <summary>
        /// This override creates the command.
        /// </summary>
        /// <returns>Returns the command.</returns>
        protected override C Create()
        {
            var command = mServiceCreator();

            return command;
        }
        #endregion
        #region Configure(C command)
        /// <summary>
        /// Configures the specified command and connects the ancillary services and events.
        /// </summary>
        /// <param name="command">The command.</param>
        protected override void Configure(C command)
        {
            base.Configure(command);

            command.OnCommandChange += Command_OnCommandChange;
            command.OnScheduleChange += Command_OnScheduleChange;

            command.TaskManager = OutgoingCapture;
        }
        #endregion

        #region OutgoingCapture(ICommand command, string id, TransmissionPayload payload)
        /// <summary>
        /// This method captures the outgoing requests from the command and adds them to the Outgoing queue.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="payload">The payload.</param>
        protected virtual void OutgoingCapture(ICommand command, string id, TransmissionPayload payload)
        {
            payload.Message.OriginatorServiceId = Dependencies.OriginatorId.ExternalServiceId;
            var harness = TrafficTrackerCreate(CommandHarnessTrafficDirection.Outgoing, payload, command.GetType().Name);

            //Add delay in here.

            if (id != null)
            {
                harness.Tracker.CallbackId = id;
                harness.Tracker.Callback = command;
            }

            long count;
            TrafficTrackerLog(harness, out count);
        } 
        #endregion

        #region Event helpers
        private void Command_OnScheduleChange(object sender, ScheduleChangeEventArgs e)
        {
            if (e.IsRemoval)
            {
                if (RegisteredSchedules.ContainsKey(e.Schedule))
                    RegisteredSchedules.Remove(e.Schedule);
            }
            else
                RegisteredSchedules.Add(e.Schedule, e.Schedule.IsMasterJob);
        }

        private void Command_OnCommandChange(object sender, CommandChangeEventArgs e)
        {
            if (e.IsRemoval)
            {
                if (RegisteredCommandMethods.ContainsKey(e.Key))
                    RegisteredCommandMethods.Remove(e.Key);
            }
            else
                RegisteredCommandMethods.Add(e.Key, e.IsMasterJob);
        }
        #endregion

        #region TrafficTrackerLog(CommandHarnessTraffic request, out long id)
        /// <summary>
        /// This method logs the command traffic and raises the appropriate event to a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns true if logged successfully.</returns>
        private bool TrafficTrackerLog(CommandHarnessTraffic request, out long id)
        {
            id = Interlocked.Increment(ref mCounter);
            if (!Traffic.TryAdd(id, request))
                return false;

            OnEvent?.Invoke(this, new CommandHarnessEventArgs(id, request));

            switch (request.Direction)
            {
                case CommandHarnessTrafficDirection.Request:
                    OnEventRequest?.Invoke(this, new CommandHarnessEventArgs(id, request));
                    break;
                case CommandHarnessTrafficDirection.Response:
                    OnEventResponse?.Invoke(this, new CommandHarnessEventArgs(id, request));
                    break;
                case CommandHarnessTrafficDirection.Outgoing:
                    OnEventOutgoing?.Invoke(this, new CommandHarnessEventArgs(id, request));
                    break;
            }

            return true;
        }
        #endregion
        #region TrafficTrackerCreate ...

        /// <summary>
        /// Traffics the tracker create.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="originatorId">The originator identifier.</param>
        /// <returns></returns>
        private CommandHarnessTraffic TrafficTrackerCreate(CommandHarnessTrafficDirection direction
            , TransmissionPayload payload
            , string reference
            , long? originatorId = null)
        {
            TaskTracker tracker = TaskManager.TrackerCreateFromPayload(payload, reference);
            return new CommandHarnessTraffic(direction, tracker, reference, originatorId);
        } 
        #endregion

        #region -> CommandExecute(TransmissionPayload payload, string reference)
        /// <summary>
        /// This method processes the command requests generated by the Dispatcher.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="reference">Something.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">payload.Id - The payload has already been processed</exception>
        protected virtual void CommandExecute(TransmissionPayload payload, string reference)
        {
            var harness = TrafficTrackerCreate(CommandHarnessTrafficDirection.Request, payload, reference);

            long id;
            if (!TrafficTrackerLog(harness, out id))
                throw new ArgumentOutOfRangeException("payload.Id", $"The payload {harness.Id} has already been processed: {harness.ReferenceId}");

            try
            {
                Service.ProcessRequest(harness.Tracker.ToTransmissionPayload(), harness.Responses).Wait();
            }
            catch (Exception ex)
            {
                harness.Exception = ex;
            }

            harness.Responses.ForEach((p) =>
            {
                long idIgnore;
                var harnessOut = TrafficTrackerCreate(CommandHarnessTrafficDirection.Response, p, reference, id);
                TrafficTrackerLog(harnessOut, out idIgnore);
            });
        } 
        #endregion
        #region -> ScheduleExecute(TaskTracker tracker)
        /// <summary>
        /// This method is used to manually process the schedule.
        /// </summary>
        /// <param name="tracker">The tracker.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">tracker - The tracker object was not of type schedule.</exception>
        protected virtual void ScheduleExecute(TaskTracker tracker)
        {
            if (tracker.Type != TaskTrackerType.Schedule)
                throw new ArgumentOutOfRangeException("tracker", "The tracker object was not of type schedule.");

            var holder = new CommandHarnessTraffic(CommandHarnessTrafficDirection.Request, tracker, $"Schedule: {tracker.ToTransmissionPayload()}");
            long outId;
            if (!TrafficTrackerLog(holder, out outId))
                throw new ArgumentOutOfRangeException("payload.Id", $"The sch {holder.Id} has already been processed: {holder.ReferenceId}");

            Exception failedEx = null;

            try
            {
                var token = new CancellationToken();
                tracker.Execute(token).Wait(token);

            }
            catch (Exception ex)
            {
                failedEx = ex;
                holder.Exception = ex;
            }
            finally
            {
                tracker.ExecuteComplete(tracker, failedEx != null, failedEx);
            }
        }
        #endregion

        #region Dispatcher
        /// <summary>
        /// Gets the dispatcher used to help set requests to the command.
        /// </summary>
        public ICommandHarnessDispath Dispatcher { get; }
        #endregion

        #region HasCommand ...
        /// <summary>
        /// Determines whether a registered command exists based on a match to the service message header.
        /// </summary>
        /// <param name="header">The header to compare.</param>
        /// <param name="useMatch">if set to true us a match, otherwise compare exactly. The default is false.</param>
        /// <returns>
        ///   <c>true</c> if the specified header has matched or is equal; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCommand(ServiceMessageHeader header, bool useMatch = false)
        {
            if (useMatch)
                return RegisteredCommandMethods.Any((s) => s.Key.Header.IsMatch(header));
            else
                return RegisteredCommandMethods.Any((s) => s.Key.Header == header);
        }

        /// <summary>
        /// Determines whether a registered command exists based on a match to the service message header fragment.
        /// The ChannelId will be set based on the ChannelId set in the command policy.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="useMatch">if set to true us a match, otherwise compare exact. The default is false.</param>
        /// <returns>
        ///   <c>true</c> if the specified fragment has matched or is equal; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCommand(ServiceMessageHeaderFragment fragment, bool useMatch = false)
        {
            return HasCommand((Policy.ChannelId, fragment), useMatch);
        }
        #endregion

        #region HasSchedule...
        /// <summary>
        /// Determines whether the collection has the specified schedule.
        /// </summary>
        /// <param name="name">The schedule name.</param>
        /// <param name="allSchedules">This optional parameter can be used to match against just the registered schedules (false) or all schedules registered with the scheduler (true)</param>
        /// <param name="scheduleType">This optional parameter can be used to match to a specific type of schedule, by matching a string on the Schedule.ScheduleType parameter.</param>
        /// <returns>Returns true if the schedule exists</returns>
        public bool HasSchedule(string name, bool allSchedules = false, string scheduleType = null)
        {
            return HasSchedule(allSchedules, (s) => s.Name == name && fnMatchScheduleType(s, scheduleType));
        }
        /// <summary>
        /// Determines whether the collection has the specified schedule.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="allSchedules">This optional parameter can be used to match against just the registered schedules (false) or all schedules registered with the scheduler (true)</param>
        /// <param name="scheduleType">This optional parameter can be used to match to a specific type of schedule, by matching a string on the Schedule.ScheduleType parameter.</param>
        /// <returns>Returns true if the schedule exists</returns>
        public bool HasSchedule(Guid id, bool allSchedules = false, string scheduleType = null)
        {
            return HasSchedule(allSchedules, (s) => s.Id == id && fnMatchScheduleType(s, scheduleType));
        }
        /// <summary>
        /// Determines whether the collection has the specified schedule.
        /// </summary>
        /// <param name="allSchedules">if set to <c>true</c> [all schedules].</param>
        /// <param name="predicate">The predicate to filter the schedule.</param>
        /// <returns>Returns true if a schedule matched the predicate.</returns>
        public bool HasSchedule(bool allSchedules, Func<Schedule, bool> predicate)
        {
            return ScheduleCollection(allSchedules).Any((s) => predicate(s));
        }
        #endregion
        #region ScheduleExecute...
        /// <summary>
        /// Triggers execution of the schedule.
        /// </summary>
        /// <param name="name">The schedule name.</param>
        /// <param name="allSchedules">This optional parameter can be used to match against just the registered schedules (false) or all schedules registered with the scheduler (true)</param>
        /// <param name="scheduleType">This optional parameter can be used to match to a specific type of schedule, by matching a string on the Schedule.ScheduleType parameter.</param>
        /// <returns>Returns true if the schedule is resolved and submitted for executed.</returns>
        public bool ScheduleExecute(string name, bool allSchedules = false, string scheduleType = null)
        {
            return ScheduleExecute(allSchedules, (s) => s.Name == name && fnMatchScheduleType(s, scheduleType));
        }
        /// <summary>
        /// Triggers execution of a schedule.
        /// </summary>
        /// <param name="id">The unique schedule id.</param>
        /// <param name="allSchedules">This optional parameter can be used to match against just the registered schedules (false) or all schedules registered with the scheduler (true)</param>
        /// <param name="scheduleType">This optional parameter can be used to match to a specific type of schedule, by matching a string on the Schedule.ScheduleType parameter.</param>
        /// <returns>Returns true if the schedule is resolved and submitted for executed.</returns>
        public bool ScheduleExecute(Guid id, bool allSchedules = false, string scheduleType = null)
        {
            return ScheduleExecute(allSchedules, (s) => s.Id == id && fnMatchScheduleType(s, scheduleType));
        }
        /// <summary>
        /// Triggers execution of a schedule.
        /// </summary>
        /// <param name="allSchedules">if set to <c>true</c> [all schedules].</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Returns true if the schedule is matched to the predicate and submitted for executed.</returns>
        public bool ScheduleExecute(bool allSchedules, Func<Schedule, bool> predicate)
        {
            var schedule = ScheduleCollection(allSchedules).FirstOrDefault((s) => predicate(s));

            if (schedule != null)
                return Dependencies.Scheduler.Execute(schedule.Id);

            return false;
        }
        #endregion
        #region ScheduleCollection(bool allSchedules)
        private Func<Schedule, string, bool> fnMatchScheduleType = (s, type) => type == null || s.ScheduleType == type;
        /// <summary>
        /// Returns the specified schedule collection.
        /// </summary>
        /// <param name="allSchedules">if set to <c>true</c> [all schedules].</param>
        /// <returns>Returns the specific schedule collection</returns>
        public IEnumerable<Schedule> ScheduleCollection(bool allSchedules)
        {
            if (allSchedules)
                return Dependencies.Scheduler.Items;
            else
                return RegisteredSchedules.Keys;
        }
        #endregion
    }
}
