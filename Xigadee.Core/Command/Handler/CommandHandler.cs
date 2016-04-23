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
    public class CommandHandler<S>: StatisticsBase<S>, ICommandHandler where S: CommandHandlerStatistics, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandHandler()
        {
        }
        #endregion

        #region Initialise(MessageFilterWrapper key, Func<TransmissionPayload, List<TransmissionPayload>, Task> action)
        /// <summary>
        /// This method initialises the holder.
        /// </summary>
        /// <param name="parent">The name of the parent container.</param>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public virtual void Initialise(MessageFilterWrapper key, Func<TransmissionPayload, List<TransmissionPayload>, Task> action)
        {
            Key = key;
            Action = action;
        }
        #endregion

        #region Key
        /// <summary>
        /// This is the key for the specific handler.
        /// </summary>
        public MessageFilterWrapper Key { get; protected set; }
        #endregion
        #region Action
        /// <summary>
        /// This is the action called when an incoming message comes in.
        /// </summary>
        public Func<TransmissionPayload, List<TransmissionPayload>, Task> Action { get; protected set; }
        #endregion

        /// <summary>
        /// This is the HandlerStatistics
        /// </summary>
        public ICommandHandlerStatistics HandlerStatistics
        {
            get
            {
                return Statistics;
            }
        }

        protected override void StatisticsRecalculate(S stats)
        {
            stats.Name = Key.Header.ToKey();
        }

        #region Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
        /// <summary>
        /// This method executes the message handler and logs the time statistics.
        /// </summary>
        /// <param name="rq">The incoming requests.</param>
        /// <param name="rs">The outgoing responses.</param>
        public async virtual Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            int timerStart = StatisticsInternal.ActiveIncrement();
            StatisticsInternal.LastAccessed = DateTime.UtcNow;

            try
            {
                await Action(rq, rs);
            }
            catch (Exception ex)
            {
                StatisticsInternal.ErrorIncrement();
                StatisticsInternal.Ex = ex;
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(timerStart);
            }
        } 
        #endregion
    }
}
