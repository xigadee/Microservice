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

        #region OnProcessRequestUnresolved(TransmissionPayload payload)
        /// <summary>
        /// This method is called when an incoming request cannot be matched to a command within the Microservice.
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="reason">The reason the request was unsesolved</param>
        protected virtual void OnProcessRequestUnresolved(TransmissionPayload payload, DispatcherRequestUnresolvedReason reason)
        {
            try
            {
                ProcessRequestUnresolved?.Invoke(this, new DispatcherRequestUnresolvedEventArgs() { Payload = payload, Reason = reason });
            }
            catch (Exception ex)
            {
                Logger?.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnProcessRequestError(TransmissionPayload payload, Exception pex)
        /// <summary>
        /// This method is called when a command throws an unhanlded exception when processing the request
        /// </summary>
        /// <param name="payload">The request payload.</param>
        /// <param name="pex">The unhandled exception.</param>
        protected virtual void OnProcessRequestError(TransmissionPayload payload, Exception pex)
        {
            try
            {
                ProcessRequestError?.Invoke(this, new ProcessRequestErrorEventArgs() { Payload = payload, Ex = pex });
            }
            catch (Exception ex)
            {
                Logger?.LogException("OnUnhandledRequest / external exception thrown on event", ex);
            }
        }
        #endregion

        #region OnStartRequested()
        /// <summary>
        /// This method is called when the Microservice receives a start request.
        /// </summary>
        protected virtual void OnStartRequested()
        {
            try
            {
                StartRequested?.Invoke(this, new StartEventArgs() { ConfigurationOptions = ConfigurationOptions });
            }
            catch (Exception ex)
            {
                Logger?.LogException("OnStartRequested / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnStartCompleted()
        /// <summary>
        /// This method is called when the Microservice completes the start request.
        /// </summary>
        protected virtual void OnStartCompleted()
        {
            try
            {
                StartCompleted?.Invoke(this, new StartEventArgs());
            }
            catch (Exception ex)
            {
                Logger?.LogException("OnStartCompleted / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnStopRequested()
        /// <summary>
        /// This method is called when the Microservice receives a stop request.
        /// </summary>
        protected virtual void OnStopRequested()
        {
            try
            {
                StopRequested?.Invoke(this, new StopEventArgs());
            }
            catch (Exception ex)
            {
                Logger?.LogException("OnStopRequested / external exception thrown on event", ex);
            }
        }
        #endregion
        #region OnStopCompleted()
        /// <summary>
        /// This method is called when the Microservice completed a stop request.
        /// </summary>
        protected virtual void OnStopCompleted()
        {
            try
            {
                StopCompleted?.Invoke(this, new StopEventArgs());
            }
            catch (Exception ex)
            {
                Logger?.LogException("OnStopCompleted / external exception thrown on event", ex);
            }
        }
        #endregion

        #region OnStatisticsIssued(MicroserviceStatistics statistics)
        /// <summary>
        /// This method is called on a regular interval when the statistics are updated.
        /// </summary>
        /// <param name="statistics">The statistics collection.</param>
        protected virtual void OnStatisticsIssued(MicroserviceStatistics statistics)
        {
            try
            {
                mDataCollection?.MicroserviceStatisticsIssued(statistics);
                StatisticsIssued?.Invoke(this, new StatisticsEventArgs() { Statistics = statistics });
            }
            catch (Exception ex)
            {
                Logger?.LogException("Action_OnStatistics / external exception thrown on event", ex);
            }
        } 
        #endregion

        #region Event wrappers...
        /// <summary>
        /// This wrapper is used for starting
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        protected virtual void EventStart(Action action, string title)
        {
            EventGeneric(action, title, MicroserviceComponentStatusChangeAction.Starting);
        }
        /// <summary>
        /// This wrapper is used for stopping
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        protected virtual void EventStop(Action action, string title)
        {
            EventGeneric(action, title, MicroserviceComponentStatusChangeAction.Stopping);
        }
        /// <summary>
        /// This is the generic exception wrapper.
        /// </summary>
        /// <param name="action">The action to wrap.</param>
        /// <param name="title">The section title.</param>
        /// <param name="type">The action type, i.e. starting or stopping.</param>
        protected virtual void EventGeneric(Action action, string title, MicroserviceComponentStatusChangeAction type)
        {
            var args = new MicroserviceStatusEventArgs(type, title);

            try
            {
                ComponentStatusChange?.Invoke(this, args);
                action();
                args.State = MicroserviceComponentStatusChangeState.Completed;
                ComponentStatusChange?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                args.Ex = new MicroserviceStatusChangeException(title, ex);
                args.State = MicroserviceComponentStatusChangeState.Failed;
                ComponentStatusChange?.Invoke(this, args);
                throw args.Ex;
            }
        }
        #endregion

    }
}
