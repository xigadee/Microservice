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

        public virtual void Initialise(string parent, MessageFilterWrapper key, Func<TransmissionPayload, List<TransmissionPayload>, Task> action)
        {
            Parent = parent;
            Key = key;
            Action = action;
        }

        #region Declarations
        public string Parent { get; protected set; }
        /// <summary>
        /// This is the key for the specific handler.
        /// </summary>
        public MessageFilterWrapper Key { get; protected set; }
        /// <summary>
        /// This is the action called when an incoming message comes in.
        /// </summary>
        public Func<TransmissionPayload, List<TransmissionPayload>, Task> Action { get; protected set; }
        #endregion

        public ICommandHandlerStatistics HandlerStatistics
        {
            get
            {
                return Statistics;
            }
        }

        protected override void StatisticsRecalculate(S stats)
        {
            stats.Name = $"{Parent} [{Key.Header.ToKey()}]";
        }

        #region Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
        /// <summary>
        /// This method executes the message handler and logs the time statistics.
        /// </summary>
        /// <param name="rq">The incoming requests.</param>
        /// <param name="rs">The outgoing responses.</param>
        public async Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
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
