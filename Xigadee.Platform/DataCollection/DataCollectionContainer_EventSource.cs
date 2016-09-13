using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public partial class DataCollectionContainer
    {
        /// <summary>
        /// This collection holds the event sources for the Microservice.
        /// </summary>
        protected ActionQueueCollection<Action<IEventSource>, IEventSource> mContainerEventSource;

        #region Start/Stop...
        /// <summary>
        /// This method starts the telemetry.
        /// </summary>
        protected virtual void StartEventSource()
        {
            mEventSource.ForEach((c) => ServiceStart(c));
            var items = mCollectors.Where((c) => c.IsSupported(DataCollectionSupport.EventSource)).Cast<IEventSource>().Union(mEventSource).ToList();
            mContainerEventSource = new ActionQueueCollection<Action<IEventSource>, IEventSource>(items, mPolicy.EventSource, EventProcessEventSource);
            mContainerEventSource.Start();
        }
        /// <summary>
        /// This method stops the event source.
        /// </summary>
        protected virtual void StopEventSource()
        {
            mContainerEventSource.Stop();
            mContainerEventSource = null;
            mEventSource.ForEach((c) => ServiceStop(c));
        }
        #endregion

        /// <summary>
        /// This is the external method to submit events to the event source.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="originatorId"></param>
        /// <param name="entry"></param>
        /// <param name="utcTimeStamp"></param>
        /// <param name="sync"></param>
        /// <returns></returns>
        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            mContainerEventSource.EventSubmit((e) => WriteSync(e, originatorId, entry, utcTimeStamp), !sync);
        }

        private void EventProcessEventSource(Action<IEventSource> action, IEventSource evSource)
        {
            action(evSource);
        }


        private void WriteSync<K, E>(IEventSource eventSource, string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp)
        {
            int numberOfRetries = 0;

            while (true)
            {
                try
                {
                    eventSource.Write(originatorId, entry, utcTimeStamp).Wait();

                    return;
                }
                catch (Exception ex)
                {
                    if (numberOfRetries >= mPolicy.EventSource.RetryLimit)
                    {
                        LogException(string.Format("Unable to log to event source {0} for {1}-{2}-{3}", eventSource.GetType().Name, entry.EntityType, entry.Key, entry.EntityVersion), ex);
                        throw;
                    }
                }

                Task.Delay(TimeSpan.FromMilliseconds(numberOfRetries * 100)).Wait();

                numberOfRetries++;
            }
        }

        /// <summary>
        /// This is the name of the logger.
        /// </summary>
        public string Name
        {
            get
            {
                return GetType().Name;
            }
        }
    }
}
