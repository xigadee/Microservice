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

namespace Xigadee
{
    /// <summary>
    /// This class is used to provide helper methods to the harness.
    /// </summary>
    public  static partial class CommandHarnessHelper
    {

        public static H MasterJobNegotiationEnable<H>(this H harness
            , string negotiationChannelIn = null
            , string negotiationChannelOut = null
            , string negotiationMessageType = null
            )
            where H : ICommandHarness
        {
            var dCommand = harness.DefaultCommand();
            var dPolicy = harness.DefaultPolicy();

            dPolicy.MasterJobEnabled = true;
            dPolicy.MasterJobNegotiationChannelIdAutoSet = true;

            if (string.IsNullOrEmpty(dPolicy.MasterJobNegotiationChannelIdIncoming))
                dPolicy.MasterJobNegotiationChannelIdIncoming = negotiationChannelIn ?? "masterjob";
            if (string.IsNullOrEmpty(dPolicy.MasterJobNegotiationChannelIdOutgoing))
                dPolicy.MasterJobNegotiationChannelIdOutgoing = negotiationChannelOut ?? "masterjob";
            if (string.IsNullOrEmpty(dPolicy.MasterJobNegotiationChannelMessageType))
                dPolicy.MasterJobNegotiationChannelMessageType = negotiationMessageType ?? dCommand.FriendlyName.ToLowerInvariant();

            dCommand.StatusChanged += (object sender, StatusChangedEventArgs e) =>
            {
                if (e.StatusNew == ServiceStatus.Running)
                {
                    //Inject the master job message.
                    harness.ScheduleExecute(dCommand.FriendlyName, true, "MasterJobNegotiationPollSchedule");
                }
            };

            harness.Intercept((ctx) => 
            {
                if (!ctx.Message.ActionType.Equals(MasterJobStates.IAmMaster, StringComparison.OrdinalIgnoreCase))
                    ctx.Outgoing.Process((dPolicy.MasterJobNegotiationChannelIdIncoming, dPolicy.MasterJobNegotiationChannelMessageType, MasterJobStates.IAmStandby));
            }
            ,header:(dPolicy.MasterJobNegotiationChannelIdOutgoing, dPolicy.MasterJobNegotiationChannelMessageType, null));

            return harness;
        }


        public static H MasterJobNegotiateOnStart<H>(this H harness)
            where H : ICommandHarness
        {
            return harness;
        }
    }
}
