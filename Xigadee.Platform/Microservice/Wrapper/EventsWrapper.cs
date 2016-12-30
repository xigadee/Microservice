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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This wrapper provides a single place for all Microservice events.
    /// </summary>
    internal class EventsWrapper: WrapperBase, IMicroserviceEvents
    {
        private IDataCollection mDataCollection = null;
        private IMicroservice mService;

        public EventsWrapper(IMicroservice service, IDataCollection dataCollection, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mService = service;
            mService.StatusChanged += OnStatusChanged;
            mDataCollection = dataCollection;
        }

        /// <summary>
        /// This event can be used to subscribe to status changes.
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        /// <summary>
        /// This event handler can be used to inspect an incoming message before it executes.
        /// </summary>
        public event EventHandler<Microservice.TransmissionPayloadState> ExecuteBegin;
        /// <summary>
        /// This event handler can be used to inspect an incoming message after it has executed.
        /// </summary>
        public event EventHandler<Microservice.TransmissionPayloadState> ExecuteComplete;

        /// <summary>
        /// This event is raised when the service start begins
        /// </summary>
        public event EventHandler<StartEventArgs> StartRequested;
        /// <summary>
        /// This event is raised when the service start completes.
        /// </summary>
        public event EventHandler<StartEventArgs> StartCompleted;
        /// <summary>
        /// This event is raised when a stop request begins.
        /// </summary>
        public event EventHandler<StopEventArgs> StopRequested;
        /// <summary>
        /// This event is raised when the service has stopped.
        /// </summary>
        public event EventHandler<StopEventArgs> StopCompleted;
        /// <summary>
        /// This event is raised when the service has stopped.
        /// </summary>
        public event EventHandler<MicroserviceStatusEventArgs> ComponentStatusChange;
        /// <summary>
        /// This event will be triggered when the Microservice statistics have been calculated.
        /// </summary>
        public event EventHandler<StatisticsEventArgs> StatisticsIssued;
        /// <summary>
        /// This event will be thrown if an incoming message cannot be resolved against a command.
        /// </summary>
        public event EventHandler<DispatcherRequestUnresolvedEventArgs> ProcessRequestUnresolved;
        /// <summary>
        /// This event will be raised when a process request errors.
        /// </summary>
        public event EventHandler<ProcessRequestErrorEventArgs> ProcessRequestError;


        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            try
            {
                StatusChanged?.Invoke(mService, e);
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnStatusChanged / external exception thrown on event", ex);
            }
        }


        #region OnExecuteBegin(Microservice.TransmissionPayloadState state)
        /// <summary>
        /// This method is called when the Microservice receives a start request.
        /// </summary>
        internal virtual void OnExecuteBegin(Microservice.TransmissionPayloadState state)
        {
            try
            {
                ExecuteBegin?.Invoke(mService, state);
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnExecuteBegin / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnExecuteComplete(Microservice.TransmissionPayloadState state)
        /// <summary>
        /// This method is called when the Microservice completes the start request.
        /// </summary>
        internal virtual void OnExecuteComplete(Microservice.TransmissionPayloadState state)
        {
            try
            {
                ExecuteComplete?.Invoke(mService, state);
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnExecuteComplete / external exception thrown on event", ex);
            }
        }
        #endregion


        #region OnProcessRequestUnresolved(TransmissionPayload payload)
        /// <summary>
        /// This method is called when an incoming request cannot be matched to a command within the Microservice.
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="reason">The reason the request was unsesolved</param>
        internal virtual void OnProcessRequestUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {
            try
            {
                ProcessRequestUnresolved?.Invoke(this, new DispatcherRequestUnresolvedEventArgs() { Payload = payload, Reason = reason });
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnProcessRequestError(TransmissionPayload payload, Exception pex)
        /// <summary>
        /// This method is called when a command throws an unhanlded exception when processing the request
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="pex">The unhandled exception.</param>
        internal virtual void OnProcessRequestError(TransmissionPayload payload, Exception pex)
        {
            try
            {
                ProcessRequestError?.Invoke(this, new ProcessRequestErrorEventArgs() { Payload = payload, Ex = pex });
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }
        #endregion

        #region OnStartRequested()
        /// <summary>
        /// This method is called when the Microservice receives a start request.
        /// </summary>
        internal virtual void OnStartRequested()
        {
            try
            {
                StartRequested?.Invoke(mService, new StartEventArgs());
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnStartRequested / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnStartCompleted()
        /// <summary>
        /// This method is called when the Microservice completes the start request.
        /// </summary>
        internal virtual void OnStartCompleted()
        {
            try
            {
                mDataCollection.LogMessage(LoggingLevel.Status, "Microservice has started.");
                StartCompleted?.Invoke(mService, new StartEventArgs());
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnStartCompleted / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnStopRequested()
        /// <summary>
        /// This method is called when the Microservice receives a stop request.
        /// </summary>
        internal virtual void OnStopRequested()
        {
            try
            {
                mDataCollection.LogMessage(LoggingLevel.Status, "Microservice is stopping.");
                StopRequested?.Invoke(mService, new StopEventArgs());
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnStopRequested / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnStopCompleted()
        /// <summary>
        /// This method is called when the Microservice completed a stop request.
        /// </summary>
        internal virtual void OnStopCompleted()
        {
            try
            {
                StopCompleted?.Invoke(mService, new StopEventArgs());
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("OnStopCompleted / external exception thrown on event", ex);
            }
        }
        #endregion

        #region OnStatisticsIssued(MicroserviceStatistics statistics)
        /// <summary>
        /// This method is called on a regular interval when the statistics are updated.
        /// </summary>
        /// <param name="statistics">The statistics collection.</param>
        internal virtual void OnStatisticsIssued(MicroserviceStatistics statistics)
        {
            try
            {
                StatisticsIssued?.Invoke(mService, new StatisticsEventArgs() { Statistics = statistics });
            }
            catch (Exception ex)
            {
                mDataCollection?.LogException("Action_OnStatistics / external exception thrown on event", ex);
            }
        }
        #endregion

        #region Event wrappers...
        /// <summary>
        /// This is the generic exception wrapper.
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        /// <param name="type">The action type, i.e. starting or stopping.</param>
        internal virtual void EventGeneric(Action action, string title, MicroserviceComponentStatusChangeAction type)
        {
            var args = new MicroserviceStatusEventArgs(type, title);

            try
            {
                ComponentStatusChange?.Invoke(mService, args);
                action();
                args.State = MicroserviceComponentStatusChangeState.Completed;
                ComponentStatusChange?.Invoke(mService, args);
            }
            catch (Exception ex)
            {
                args.Ex = new MicroserviceStatusChangeException(title, ex);
                args.State = MicroserviceComponentStatusChangeState.Failed;
                ComponentStatusChange?.Invoke(mService, args);
                throw args.Ex;
            }
        }
        #endregion
    }
}
