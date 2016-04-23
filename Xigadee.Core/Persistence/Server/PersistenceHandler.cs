using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This handler holds and tracks commands to the persistence agent.
    /// </summary>
    public class PersistenceHandler:CommandHandler<PersistenceHandlerStatistics>
    {
        public override async Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs)
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
                int extent = StatisticsInternal.ActiveDecrement(timerStart);
            }
        }
    }

    public class PersistenceHandlerStatistics: CommandHandlerStatistics
    {

    }
}
