#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the Handler and the action.
    /// </summary>
    public class CommandHandler : StatisticsBase<CommandHandlerStatistics>
    {
        #region Declarations
        /// <summary>
        /// This is the key for the specific handler.
        /// </summary>
        private readonly MessageFilterWrapper Key;
        /// <summary>
        /// This is the action called when an incoming message comes in.
        /// </summary>
        private readonly Func<TransmissionPayload, List<TransmissionPayload>, Task> Action;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="parent">The handler parent.</param>
        /// <param name="key">The key.</param>
        /// <param name="action">The action.</param>
        public CommandHandler(string parent, MessageFilterWrapper key
            , Func<TransmissionPayload, List<TransmissionPayload>, Task> action)
        {
            Key = key;
            Action = action;
            mStatistics.Name = string.Format("{0} [{1}]", parent, key.Header.ToKey());
        } 
        #endregion

        #region Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
        /// <summary>
        /// This method executes the message handler and logs the time statistics.
        /// </summary>
        /// <param name="rq">The incoming requests.</param>
        /// <param name="rs">The outgoing responses.</param>
        public async Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            int timerStart = mStatistics.ActiveIncrement();
            mStatistics.LastAccessed = DateTime.UtcNow;

            try
            {
                await Action(rq, rs);
            }
            catch (Exception ex)
            {
                mStatistics.ErrorIncrement();
                mStatistics.Ex = ex;
                throw;
            }
            finally
            {
                mStatistics.ActiveDecrement(timerStart);
            }
        } 
        #endregion
    }
}
