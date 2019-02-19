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

            if (Policy.OutgoingRequestsEnabled)
                stats.OutgoingRequests = mOutgoingRequests?.Select((h) => h.Value.Debug).ToList();

            stats.MasterJob.Enabled = Policy.MasterJobEnabled;

            if (Policy.MasterJobEnabled)
            {
                //stats.MasterJob.Server = string.Format("{0} @ {1:o}", mCurrentMasterServiceId, mCurrentMasterReceiveTime);
                stats.MasterJob.Status = string.Format("Status={0} Channel={1}/{2} Type={3}"
                    , mMasterJobContext.State.ToString()
                    , Policy.MasterJobNegotiationChannelIdOutgoing
                    , Policy.MasterJobNegotiationChannelPriority
                    , Policy.MasterJobNegotiationChannelMessageType
                    );

                stats.MasterJob.Master = mMasterJobContext.PartnerMaster;
                stats.MasterJob.Standbys = mMasterJobContext.Partners.Values.ToList();
            }
        }
    }
}
