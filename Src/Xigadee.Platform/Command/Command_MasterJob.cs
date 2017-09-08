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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Xigadee;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region Events
        /// <summary>
        /// This event can is fired when the state of the master job is changed.
        /// </summary>
        public event EventHandler<MasterJobStateChangeEventArgs> OnMasterJobStateChange;
        /// <summary>
        /// This event is fired when the masterjob receives or issues communication requests to other masterjobs.
        /// </summary>
        public event EventHandler<MasterJobCommunicationEventArgs> OnMasterJobCommunication;
        #endregion
        #region Declarations
        /// <summary>
        /// This is the context that 
        /// </summary>
        protected MasterJobContext mMasterJobContext;
        #endregion

        #region *--> MasterJobTearUp()
        /// <summary>
        /// This method sets up the master job.
        /// </summary>
        protected virtual void MasterJobTearUp()
        {
            if (mPolicy.MasterJobNegotiationChannelIdIncoming == null)
                throw new CommandStartupException("Masterjobs are enabled, but the NegotiationChannelIdIncoming has not been set");
            if (mPolicy.MasterJobNegotiationChannelType == null)
                throw new CommandStartupException("Masterjobs are enabled, but the NegotiationChannelType has not been set");

            mMasterJobContext = new MasterJobContext(mPolicy.MasterJobName ?? FriendlyName, mPolicy.MasterJobNegotiationStrategy);

            mMasterJobContext.OnMasterJobStateChange += (object o, MasterJobStateChangeEventArgs s) => FireAndDecorateEventArgs(OnMasterJobStateChange, () => s);

            //Initialise the context.
            mMasterJobContext.Start();
            //Add the schedule to generate poll negotiation requests.
            Scheduler.Register(mMasterJobContext.NegotiationPollScheduleInitialise(MasterJobStateNotificationOutgoing));
            //Add the incoming poll channel.
            CommandRegister((MasterJobNegotiationChannelIdIncoming, mPolicy.MasterJobNegotiationChannelType, null), MasterJobStateNotificationIncoming);
        }
        #endregion
        #region *--> MasterJobTearDown()
        /// <summary>
        /// This method stops and running master job processes.
        /// </summary>
        public virtual void MasterJobTearDown()
        {
            if (mMasterJobContext.State == MasterJobState.Active)
                MasterJobStop();

            //Remove the negotiation poll schedule
            Scheduler.Unregister(mMasterJobContext.NegotiationPollSchedule);
            //Remove the incoming poll command.
            CommandUnregister((MasterJobNegotiationChannelIdIncoming, mPolicy.MasterJobNegotiationChannelType, null));

            mMasterJobContext.Stop();
        }
        #endregion

        #region MasterJobNegotiationChannelPriority
        /// <summary>
        /// This is the default channel priority, which is 2 unless otherwise set
        /// </summary>
        public virtual int MasterJobNegotiationChannelPriority
        {
            get
            {
                return mPolicy.MasterJobNegotiationChannelPriority;
            }
            set
            {
                mPolicy.MasterJobNegotiationChannelPriority = value;
            }
        }
        #endregion
        #region MasterJobNegotiationChannelType
        /// <summary>
        /// This command is used to negotiate the channel type.
        /// </summary>
        public virtual string MasterJobNegotiationChannelType
        {
            get
            {
                return mPolicy.MasterJobNegotiationChannelType;
            }
            set
            {
                mPolicy.MasterJobNegotiationChannelType = value;
            }
        }
        #endregion
        #region MasterJobNegotiationChannelIdOutgoing
        /// <summary>
        /// This is the channel used to negotiate control for a master job.
        /// </summary>
        public virtual string MasterJobNegotiationChannelIdOutgoing
        {
            get
            {
                return mPolicy.MasterJobNegotiationChannelIdOutgoing;
            }
            set
            {
                mPolicy.MasterJobNegotiationChannelIdOutgoing = value;
            }
        }
        #endregion
        #region MasterJobNegotiationChannelIdIncoming
        /// <summary>
        /// This is the channel used to negotiate control for a master job.
        /// </summary>
        public virtual string MasterJobNegotiationChannelIdIncoming
        {
            get
            {
                return mPolicy.MasterJobNegotiationChannelIdIncoming;
            }
            set
            {
                mPolicy.MasterJobNegotiationChannelIdIncoming = value;
            }
        }
        #endregion
        #region MasterJobNegotiationChannelIdAutoSet
        /// <summary>
        /// Specifies whether the master job negotiation channel can be set during configuration.
        /// </summary>
        public virtual bool MasterJobNegotiationChannelIdAutoSet
        {
            get { return mPolicy.MasterJobNegotiationChannelIdAutoSet; }
        } 
        #endregion

        #region NegotiationTransmit(string action)
        /// <summary>
        /// This method transmits the notification message to the other instances.
        /// The message is specified to be processed externally so to ensure that we can determine whether 
        /// the comms are active.
        /// </summary>
        /// <param name="action">The master job action to transmit.</param>
        protected virtual Task NegotiationTransmit(string action)
        {
            var payload = TransmissionPayload.Create(mPolicy.TransmissionPayloadTraceEnabled);
            payload.TraceWrite("Create", "Command/NegotiationTransmit");

            payload.Options = ProcessOptions.RouteExternal;

            var message = payload.Message;
            //Note: historically there was only one channel, so we use the incoming channel if the outgoing has
            //not been specified. These should all be lower case so that Service Bus can match accurately.
            message.ChannelId = (mPolicy.MasterJobNegotiationChannelIdOutgoing ?? mPolicy.MasterJobNegotiationChannelIdIncoming).ToLowerInvariant();
            message.MessageType = mPolicy.MasterJobNegotiationChannelType.ToLowerInvariant();
            message.ActionType = action.ToLowerInvariant();

            message.ChannelPriority = mPolicy.MasterJobNegotiationChannelPriority;

            //Go straight to the dispatcher as we don't want to use the tracker for this job
            //as it is transmit only. Only send messages if the service is in a running state.
            switch (Status)
            {
                case ServiceStatus.Running:
                    Outgoing.Process(payload);
                    FireAndDecorateEventArgs(OnMasterJobCommunication
                        , () => new MasterJobCommunicationEventArgs(
                              MasterJobCommunicationDirection.Outgoing
                            , mMasterJobContext.State
                            , action
                            , mMasterJobContext.StateChangeCounter)
                        , (args,ex) => payload.TraceWrite($"Error: {ex.Message}", "Command/NegotiationTransmit/OnMasterJobCommunication"));
                    break;
            }

            return Task.FromResult(0);
        }
        #endregion

        #region ProcessRequestIfSelfGenerated(TransmissionPayload rq)
        /// <summary>
        /// Processes the request if self generated. This is used to accommodate state change.
        /// </summary>
        /// <param name="rq">The incoming request.</param>
        /// <returns>Returns true if the request is from this command.</returns>
        protected virtual bool ProcessRequestIfSelfGenerated(TransmissionPayload rq)
        {
            if (!string.Equals(rq.Message.OriginatorServiceId, OriginatorId.ExternalServiceId, StringComparison.InvariantCultureIgnoreCase))
                return false;

            rq.TraceWrite("Processing", "Command/ProcessSelfSentMessage");
            switch (mMasterJobContext.State)
            {
                case MasterJobState.VerifyingComms:
                    //We can now say that the masterjob channel is working, so we can now enable the job for negotiation.
                    mMasterJobContext.State = MasterJobState.Starting;
                    break;
                case MasterJobState.Inactive:
                    if (mMasterJobContext.MasterPollAttemptsExceeded())
                        mMasterJobContext.State = MasterJobState.Starting;
                    break;
                case MasterJobState.Starting:
                    if (IsMatch(rq, MasterJobStates.WhoIsMaster) && mMasterJobContext.MasterPollAttemptsExceeded())
                        mMasterJobContext.State = MasterJobState.Requesting1;
                    break;
                case MasterJobState.Requesting1:
                    if (IsMatch(rq, MasterJobStates.RequestingControl1) && mMasterJobContext.MasterPollAttemptsExceeded())
                        mMasterJobContext.State = MasterJobState.Requesting2;
                    break;
                case MasterJobState.Requesting2:
                    if (IsMatch(rq, MasterJobStates.RequestingControl2) && mMasterJobContext.MasterPollAttemptsExceeded())
                        mMasterJobContext.State = MasterJobState.TakingControl;
                    break;
                case MasterJobState.TakingControl:
                    if (IsMatch(rq, MasterJobStates.TakingControl))
                        MasterJobStart();
                    break;
            }

            return true;
        }
        #endregion

        #region IsMatch(TransmissionPayload rq, string state)
        /// <summary>
        /// Determines whether there is a case insensitive match with the comparison transmission message type.
        /// </summary>
        /// <param name="rq">The rq.</param>
        /// <param name="state">The message type</param>
        /// <returns>Returns true if correct.</returns>
        protected bool IsMatch(TransmissionPayload rq, string state)
        {
            return state.Equals(rq.Message.ActionType, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion
        #region InformOrResetState(MasterJobState state)
        /// <summary>
        /// This method sends a notification message if the service is active, otherwise it sets the service to inactive.
        /// </summary>
        /// <param name="state">The state.</param>
        protected async Task InformOrResetState(MasterJobState state)
        {
            if (mMasterJobContext.State == MasterJobState.Active)
                await MasterJobSyncIAmMaster();
            else if (mMasterJobContext.State <= state)
            {
                mMasterJobContext.State = MasterJobState.Inactive;
            }
        } 
        #endregion
        #region Negotiation(string message, Schedule schedule, bool increment = true)
        /// <summary>
        /// Transmits the negotiation messages, and then sets the next poll time using the context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="increment">If set to <c>true</c> increment the poll attemps counter.</param>
        protected async Task Negotiation(string message, bool increment = true)
        {
            await NegotiationTransmit(message);
            mMasterJobContext.NegotiationPollSetNextTime();
            if (increment)
                mMasterJobContext.MasterPollAttemptsIncrement();
        } 
        #endregion

        #region --> MasterJobStateNotificationIncoming(TransmissionPayload rq, List<TransmissionPayload> rs)
        /// <summary>
        /// This method processes state notifications from other instances of the MasterJob.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The responses - this is not used.</param>
        protected virtual async Task MasterJobStateNotificationIncoming(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            mMasterJobContext.NegotiationPollLastIn = DateTime.UtcNow;
            rq.TraceWrite("Received", "Command/MasterJobStateNotificationIncoming");

            //If we are not active then do nothing.
            if (mMasterJobContext.State == MasterJobState.Disabled)
                return;

            //Filter out the messages sent from this service. 
            //We will use these messages to signal a transition to the next state.
            if (ProcessRequestIfSelfGenerated(rq))
                return;

            rq.TraceWrite("Processing", "Command/MasterJobStateNotificationIncoming");

            //Raise an event for the incoming communication.
            FireAndDecorateEventArgs(OnMasterJobCommunication, () => new MasterJobCommunicationEventArgs(
                  MasterJobCommunicationDirection.Incoming
                , mMasterJobContext.State
                , rq.Message.ActionType
                , mMasterJobContext.StateChangeCounter
                , rq.Message.OriginatorServiceId));

            if (IsMatch(rq,MasterJobStates.IAmStandby))
            {
                mMasterJobContext.PartnerSet(rq.Message.OriginatorServiceId, true);
            }
            else if (IsMatch(rq, MasterJobStates.IAmMaster))
            {
                if (mMasterJobContext.State == MasterJobState.Active)
                    MasterJobStop();

                mMasterJobContext.PartnerSet(rq.Message.OriginatorServiceId, false);

                mMasterJobContext.State = MasterJobState.Inactive;

                await NegotiationTransmit(MasterJobStates.IAmStandby);
            }
            else if (IsMatch(rq, MasterJobStates.ResyncMaster))
            {
                mMasterJobContext.PartnerMasterClear();

                if (mMasterJobContext.State == MasterJobState.Inactive)
                    mMasterJobContext.State = MasterJobState.Starting;
            }
            else if (IsMatch(rq, MasterJobStates.WhoIsMaster))
            {
                mMasterJobContext.PartnerSet(rq.Message.OriginatorServiceId, true);
                if (mMasterJobContext.State == MasterJobState.Active)
                    await MasterJobSyncIAmMaster();
            }
            else if (IsMatch(rq, MasterJobStates.RequestingControl1))
            {
                await InformOrResetState(MasterJobState.Requesting1);
            }
            else if (IsMatch(rq, MasterJobStates.RequestingControl2))
            {
                await InformOrResetState(MasterJobState.Requesting2);
            }
            else if (IsMatch(rq, MasterJobStates.TakingControl))
            {
                await InformOrResetState(MasterJobState.TakingControl);
            }
            else
            {
                Collector?.LogMessage(LoggingLevel.Warning, $"{rq?.Message?.ActionType??"NULL"} is not a valid negotiating action type for master job {FriendlyName}", "MasterJob");
                rq.TraceWrite("Unhandled", "Command/MasterJobStateNotificationIncoming");
                return;
            }
            rq.TraceWrite("Complete", "Command/MasterJobStateNotificationIncoming");

        }
        #endregion
        #region POLL: MasterJobStateNotificationOutgoing(Schedule state, CancellationToken token) -->
        /// <summary>
        /// This is the master job outgoing negotiation logic.
        /// </summary>
        /// <param name="schedule">The current schedule object.</param>
        /// <param name="token">The cancellation token</param>
        protected virtual async Task MasterJobStateNotificationOutgoing(Schedule schedule, CancellationToken token)
        {
            //We set a random poll time to reduce the potential for deadlocking
            //and to make the negotiation messaging more efficient.
            switch (mMasterJobContext.State)
            {
                case MasterJobState.VerifyingComms:
                    await Negotiation(MasterJobStates.WhoIsMaster, false);
                    break;
                case MasterJobState.Inactive:
                    await Negotiation(MasterJobStates.WhoIsMaster);
                    break;
                case MasterJobState.Starting:
                    await Negotiation(MasterJobStates.WhoIsMaster);
                    break;
                case MasterJobState.Requesting1:
                    await Negotiation(MasterJobStates.RequestingControl1);
                    break;
                case MasterJobState.Requesting2:
                    await Negotiation(MasterJobStates.RequestingControl2);
                    break;
                case MasterJobState.TakingControl:
                    await Negotiation(MasterJobStates.TakingControl);
                    break;
                case MasterJobState.Active:
                    await MasterJobSyncIAmMaster();
                    mMasterJobContext.NegotiationPollSetNextTime();
                    break;
                default:
                    mMasterJobContext.NegotiationPollSetNextTime();
                    return;
            }

            mMasterJobContext.NegotiationPollLastOut = DateTime.UtcNow;
        }
        #endregion

        #region MasterJobSyncIAmMaster()
        /// <summary>
        /// This method is used to send a sync message when the master job becomes active.
        /// </summary>
        /// <returns></returns>
        private async Task MasterJobSyncIAmMaster()
        {
            await NegotiationTransmit(MasterJobStates.IAmMaster);
            mMasterJobContext.PartnerMasterClear();
        }
        #endregion

        //MasterJob Start
        #region MasterJobStart()
        /// <summary>
        /// This method registers each job with the scheduler.
        /// </summary>
        protected virtual void MasterJobStart()
        {
            if (mMasterJobContext.State == MasterJobState.Active)
                return;

            mMasterJobContext.State = MasterJobState.Active;
            MasterJobSyncIAmMaster().Wait();

            MasterJobCommandsStart();

            MasterJobSchedulesStart();
        }
        #endregion
        #region MasterJobCommandsStart()
        /// <summary>
        /// Controls the master job commands start.
        /// </summary>
        protected virtual void MasterJobCommandsStart()
        {
            MasterJobCommandsManualRegister();

            foreach (var holder in this.CommandMethodSignatures<MasterJobCommandContractAttribute, CommandMethodSignature>(true, mPolicy.MasterJobCommandContractAttributeInherit))
                CommandRegister(CommandChannelIdAdjust(holder.Attribute)
                    , (rq, rs) => holder.Signature.Action(rq, rs, PayloadSerializer)
                    , referenceId: holder.Reference
                    , isMasterJob: true
                    );
        } 
        #endregion
        #region MasterJobCommandsManualRegister()
        /// <summary>
        /// You should override this command to register incoming requests when the master job becomes active.
        /// </summary>
        protected virtual void MasterJobCommandsManualRegister()
        {

        }
        #endregion
        #region MasterJobSchedulesStart()
        /// <summary>
        /// This method starts the master job schedules.
        /// </summary>
        protected virtual void MasterJobSchedulesStart()
        {
            MasterJobSchedulesManualRegister();

            if (mPolicy.MasterJobScheduleReflectionSupported)
                JobSchedulesReflectionInitialise<MasterJobScheduleAttribute>(mPolicy.MasterJobScheduleAttributeInherit);
        }
        #endregion
        #region MasterJobSchedulesManualRegister()
        /// <summary>
        /// Override this method to set your schedule registration code.
        /// </summary>
        protected virtual void MasterJobSchedulesManualRegister()
        {

        }
        #endregion

        #region MasterJobScheduleRegister(...)        
        /// <summary>
        /// Creates and registers a schedule.
        /// </summary>
        /// <param name="execute">The execution function.</param>
        /// <param name="timerConfig">The timer poll configuration. If this is set to null, the schedule will fire immediately after it is registered.</param>
        /// <param name="context">The optional schedule context</param>
        /// <param name="name">The name of the schedule.</param>
        /// <param name="isLongRunning">A boolean flag that specifies whether the process is long running.</param>
        /// <param name="tearUp">The set up action.</param>
        /// <param name="tearDown">The clear down action.</param>
        /// <returns>Returns the new master job schedule.</returns>
        protected virtual CommandJobSchedule MasterJobScheduleRegister(Func<Schedule, CancellationToken, Task> execute
            , ScheduleTimerConfig timerConfig = null
            , object context = null
            , string name = null
            , bool isLongRunning = false
            , Action<Schedule> tearUp = null
            , Action<Schedule> tearDown = null)
        {
            var schedule = new CommandJobSchedule();

            schedule.Initialise(execute, timerConfig, context, name, isLongRunning, tearUp, tearDown, true);

            return JobScheduleRegister(schedule);
        }
        #endregion

        //MasterJob Stop
        #region MasterJobStop()
        /// <summary>
        /// This method removes each job from the scheduler.
        /// </summary>
        protected virtual void MasterJobStop()
        {
            if (mMasterJobContext.State != MasterJobState.Active)
                return;

            mMasterJobContext.State = MasterJobState.Disabled;

            NegotiationTransmit(MasterJobStates.ResyncMaster);

            MasterJobSchedulesStop();

            MasterJobCommandsStop();

            //Reset the state to inactive so that it could restart at some point.
            mMasterJobContext.State = MasterJobState.Inactive;
        }
        #endregion
        #region MasterJobCommandsStop()
        /// <summary>
        /// Controls the master job commands stop.
        /// </summary>
        protected virtual void MasterJobCommandsStop()
        {
            MasterJobCommandsManualUnregister();

            var list = mSupported.Where((s) => s.Value.IsMasterJob).ToList();

            list.ForEach((s) => CommandUnregister(s.Key, true));
        } 
        #endregion
        #region MasterJobCommandsManualUnregister()
        /// <summary>
        /// You should override this method to unregister active commands when the job is shutting down or moves to an inactive state.
        /// </summary>
        protected virtual void MasterJobCommandsManualUnregister()
        {

        }
        #endregion
        #region MasterJobSchedulesStop()
        /// <summary>
        /// This method removes the master job schedules.
        /// </summary>
        protected virtual void MasterJobSchedulesStop()
        {
            var schedule = mSchedules.Where((s) => s.IsMasterJob).ToArray();

            schedule.ForEach((s) => JobScheduleUnregister(s));
        }
        #endregion
    }
}
