#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This internal collection contains the Event Source container.
    /// The Event Source is used to track all changes of state for a Microservice so that the system can be 
    /// reconstructed if there is a failure to any primary systems.
    /// </summary>
    public class EventSourceContainer : ActionQueueCollectionBase<Action<IEventSource>, IEventSource, EventSourceContainerStatistics, EventSourcePolicy>
        , IEventSource, IServiceLogger
    {
        private ILoggerExtended mLogger;

        public EventSourceContainer(EventSourcePolicy policy, IEnumerable<IEventSource> eventSources)
            : base(eventSources, policy)
        {
        }

        #region Logger
        /// <summary>
        /// This is the logger used to record event source errors.
        /// </summary>
        public ILoggerExtended Logger
        {
            get { return mLogger; }
            set
            {
                mLogger = value;
                ContainerInternal.OfType<IServiceLogger>().ForEach(sl => sl.Logger = mLogger);
            }
        }
        #endregion

        /// <summary>
        /// This is the name of the container.
        /// </summary>
        public string Name
        {
            get
            {
                return nameof(EventSourceContainer);
            }
        }


        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            if (sync)
            {
                await Task.WhenAll(Items.Select(i => WriteSync(i, originatorId, entry, utcTimeStamp)));
                return;
            }

            Enqueue(i =>
            {
                try
                {
                    i.Write(originatorId, entry, utcTimeStamp);
                }
                catch (Exception ex)
                {
                    Logger.LogException("Unhandled EventSource Exception", ex);
                }
            });
        }

        protected override void Process(Action<IEventSource> data, IEventSource item)
        {
            data(item);
        }

        private async Task WriteSync<K, E>(IEventSource eventSource, string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp)
        {
            int numberOfRetries = 0;
            while (true)
            {
                try
                {
                    await eventSource.Write(originatorId, entry, utcTimeStamp);
                    return;
                }
                catch (Exception ex)
                {
                    if (numberOfRetries > 10)
                    {
                        Logger.LogException(string.Format("Unable to log to event source {0} for {1}-{2}-{3}", eventSource.GetType().Name, entry.EntityType, entry.Key, entry.EntityVersion), ex);
                        throw;
                    }
                }

                await Task.Delay(TimeSpan.FromMilliseconds(numberOfRetries * 100));

                numberOfRetries++;
            }
        }
    }
}
