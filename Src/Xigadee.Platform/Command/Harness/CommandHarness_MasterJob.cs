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
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class CommandHarness<C, S, P>
    {
        #region MasterJobNegotiationEnable ...
        /// <summary>
        /// This method is used to set the command ready to run the master job.
        /// </summary>
        /// <param name="negotiationChannelIn">The negotiation channel in.</param>
        /// <param name="negotiationChannelOut">The negotiation channel out.</param>
        /// <param name="negotiationMessageType">The Message Type for the negotiation message.</param>
        /// <param name="strategy">The negotiation strategy. If this is not set, MasterJobNegotiationStrategyDebug will be used.</param>
        /// <exception cref="CommandHarnessException">MasterJobEnable should be called before the command is started.</exception>
        public void MasterJobNegotiationEnable(string negotiationChannelIn = null
            , string negotiationChannelOut = null
            , string negotiationMessageType = null
            , MasterJobNegotiationStrategyBase strategy = null)
        {
            if (Service.Status == ServiceStatus.Running)
                throw new CommandHarnessException("MasterJobEnable should be called before the command is started.");

            Policy.MasterJobNegotiationStrategy = strategy ?? new MasterJobNegotiationStrategyDebug();

            if (string.IsNullOrEmpty(Policy.MasterJobNegotiationChannelIdIncoming))
                Policy.MasterJobNegotiationChannelIdIncoming = negotiationChannelIn ?? "masterjob";
            if (string.IsNullOrEmpty(Policy.MasterJobNegotiationChannelIdOutgoing))
                Policy.MasterJobNegotiationChannelIdOutgoing = negotiationChannelOut ?? "masterjob";
            if (string.IsNullOrEmpty(Policy.MasterJobNegotiationChannelMessageType))
                Policy.MasterJobNegotiationChannelMessageType = negotiationMessageType ?? Service.FriendlyName.ToLowerInvariant();

            //Trigger the schedule to run when the command starts.
            Service.StatusChanged += (object sender, StatusChangedEventArgs e) =>
            {
                if (e.StatusNew == ServiceStatus.Running)
                {
                    //Inject the master job message.
                    MasterJobScheduleExecute();
                }
            };

            //Intercept outgoing MasterJob Messages and loop back to pass the comms check.
            this.InterceptOutgoing(MasterJobLoopback, header: (Policy.MasterJobNegotiationChannelIdOutgoing, Policy.MasterJobNegotiationChannelMessageType, null));

            //Set the policy to enable the MasterJob
            Policy.MasterJobEnabled = true;
        } 
        #endregion

        protected virtual void MasterJobLoopback(CommandHarnessRequestContext ctx)
        {
            ctx.Outgoing.Process(
                (Policy.MasterJobNegotiationChannelIdIncoming, Policy.MasterJobNegotiationChannelMessageType, ctx.Message.ActionType)
                , originatorServiceId: Dependencies.OriginatorId.ExternalServiceId
                );
        }

        #region MasterJobScheduleExecute()
        /// <summary>
        /// This job runs the MasterJob poll schedule.
        /// </summary>
        public void MasterJobScheduleExecute()
        {
            ScheduleExecute(Service.FriendlyName, true, "MasterJobNegotiationPollSchedule");
        }
        #endregion

        #region MasterJobStart
        /// <summary>
        /// Starts the master job and waits for it to go active.
        /// </summary>
        public void MasterJobStart()
        {
            ManualResetEventSlim mre;
            CancellationToken token;
            MasterJobStart(out mre, out token);

            //Wait for it to complete.
            mre.Wait();
        }
        /// <summary>
        /// Starts the master job and waits for it to go active.
        /// </summary>
        /// <param name="mre">The master reset event used to hold the thread.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <exception cref="CommandHarnessException">
        /// MasterJobs have not been enabled for this command.
        /// or
        /// MasterJobStart should be called after the command has started.
        /// </exception>
        public void MasterJobStart(out ManualResetEventSlim mre, out CancellationToken cancelToken)
        {
            if (!Policy.MasterJobEnabled)
                throw new CommandHarnessException("MasterJobs have not been enabled for this command.");

            if (Service.Status != ServiceStatus.Running)
                throw new CommandHarnessException("MasterJobStart should be called after the command has started.");

            var signal = new ManualResetEventSlim();
            cancelToken = new CancellationToken();
            bool hasGoneActive = false;

            Service.OnMasterJobChange += (object sender, MasterJobStatusChangeEventArgs e) =>
            {
                if (e.State == MasterJobState.Active)
                {
                    hasGoneActive = true;
                    signal.Set();
                }
            };

            mre = signal;

            Task.Run(() =>
            {
                while (!hasGoneActive)
                {
                    MasterJobScheduleExecute();

                    Dispatcher.Process(
                        (Policy.MasterJobNegotiationChannelIdIncoming, Policy.MasterJobNegotiationChannelMessageType, MasterJobStates.WhoIsMaster)
                        , originatorServiceId: Dependencies.OriginatorIdExternal.ExternalServiceId
                        );

                    Task.Delay(10);
                }
            }, cancelToken);

            MasterJobScheduleExecute();
        }

        #endregion

        /// <summary>
        /// Stops the master job.
        /// </summary>
        public void MasterJobStop()
        {
            ManualResetEventSlim mre;
            CancellationToken token;
            MasterJobStop(out mre, out token);

            //Wait for it to complete.
            mre.Wait();
        }


        /// <summary>
        /// Stops the master job.
        /// </summary>
        public void MasterJobStop(out ManualResetEventSlim mre, out CancellationToken cancelToken)
        {
            var signal = new ManualResetEventSlim();
            cancelToken = new CancellationToken();
            bool hasStopped = false;

            mre = signal;
        }
    }
}
