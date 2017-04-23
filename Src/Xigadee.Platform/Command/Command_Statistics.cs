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
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        /// <summary>
        /// This override lists the handlers supported for each handler.
        /// </summary>
        protected override void StatisticsRecalculate(S stats)
        {
            base.StatisticsRecalculate(stats);

            stats.Name = FriendlyName;
            stats.StartupPriority = StartupPriority;

            stats.SupportedHandlers = mSupported.Select((h) => h.Value.HandlerStatistics).ToList();

            if (mPolicy.OutgoingRequestsEnabled)
                stats.OutgoingRequests = mOutgoingRequests?.Select((h) => h.Value.Debug).ToList();

            stats.MasterJob.Active = mPolicy.MasterJobEnabled;
            if (mPolicy.MasterJobEnabled)
            {
                stats.MasterJob.Server = string.Format("{0} @ {1:o}", mCurrentMasterServiceId, mCurrentMasterReceiveTime);
                stats.MasterJob.Status = string.Format("Status={0} Channel={1}/{2} Type={3}"
                    , State.ToString()
                    , mPolicy.MasterJobNegotiationChannelIdOutgoing
                    , mPolicy.MasterJobNegotiationChannelPriority
                    , mPolicy.MasterJobNegotiationChannelType
                    );
                stats.MasterJob.Standbys = mStandbyPartner.Values.ToList();
            }
        }
    }
}
