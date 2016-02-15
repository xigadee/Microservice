#region using
using System;
using System.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This command is the base implementation that allows multiple commands to be handled within a single container.
    /// </summary>
    public abstract partial class CommandBase<S, P>
    {
        #region StatisticsRecalculate()
        /// <summary>
        /// This override lists the handlers supported for each handler.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            try
            {
                mStatistics.SupportedHandlers = mSupported.Select((h) => string.Format("{0}.{1} {2}", h.Key.Header.ToKey(), h.Key.ClientId, h.Key.IsDeadLetter ? "DL" : "")).ToList();

                mStatistics.MasterJob.Active = mPolicy.RequireMasterJob;
                if (mPolicy.RequireMasterJob)
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
        #endregion
    }
}
