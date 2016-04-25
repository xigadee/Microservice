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
            {
                stats.OutgoingRequests = mOutgoingRequests?.Select((h) => h.Value.Debug).ToList();
            }

            stats.MasterJob.Active = mPolicy.MasterJobEnabled;
            if (mPolicy.MasterJobEnabled)
            {
                stats.MasterJob.Server = string.Format("{0} @ {1:o}", mCurrentMasterServiceId, mCurrentMasterReceiveTime);
                stats.MasterJob.Status = string.Format("Status={0} Channel={1}/{2} Type={3}", State.ToString(), NegotiationChannelId, NegotiationChannelPriority, NegotiationMessageType);
                stats.MasterJob.Standbys = mStandbyPartner.Values.ToList();
            }

        }
    }
}
