#region using
using System;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P>
    {
        /// <summary>
        /// This override lists the handlers supported for each handler.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            try
            {
                mStatistics.SupportedHandlers = mSupported.Select((h) => string.Format("{0}.{1} {2}", h.Key.Header.ToKey(), h.Key.ClientId, h.Key.IsDeadLetter ? "DL" : "")).ToList();

                mStatistics.MasterJob.Active = mPolicy.MasterJobEnabled;
                if (mPolicy.MasterJobEnabled)
                {
                    mStatistics.MasterJob.Server = string.Format("{0} @ {1:o}", mCurrentMasterServiceId, mCurrentMasterReceiveTime);
                    mStatistics.MasterJob.Status = string.Format("Status={0} Channel={1}/{2} Type={3}", State.ToString(), NegotiationChannelId, NegotiationChannelPriority, NegotiationMessageType);
                    mStatistics.MasterJob.Standbys = mStandbyPartner.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                //We don't want to throw an exception here.
                mStatistics.Ex = ex;
            }
        }
    }
}
