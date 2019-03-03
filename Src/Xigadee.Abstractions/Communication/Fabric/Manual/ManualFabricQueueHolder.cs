using System;
using System.Collections.Concurrent;
namespace Xigadee
{
    /// <summary>
    /// This class holds the messages transmitted between services.
    /// </summary>
    public class ManualFabricQueueHolder
    {
        public ManualFabricQueueHolder()
        {
            Queue = new ConcurrentQueue<CommunicationFabricMessage>();
        }

        public bool Enqueue(CommunicationFabricMessage message)
        {
            if (!(Filter?.Invoke(message) ?? true))
                return false;

            Queue.Enqueue(message);
            return true;
        }

        private object syncDeadletter = new object();

        public void DeadletterEnqueue(CommunicationFabricMessage message)
        {
            if (Deadletter == null)
                lock (syncDeadletter)
                {
                    if (Deadletter == null)
                        Deadletter = new ConcurrentQueue<CommunicationFabricMessage>();
                }

            Deadletter.Enqueue(message);
        }

        public bool TryDequeue(out CommunicationFabricMessage message)
        {
            return Queue.TryDequeue(out message);
        }


        public bool TryDeadletterDequeue(out CommunicationFabricMessage message)
        {
            message = null;
            return Deadletter?.TryDequeue(out message) ?? false;
        }


        public Func<CommunicationFabricMessage, bool> Filter { get; set; }

        public ConcurrentQueue<CommunicationFabricMessage> Queue { get; }

        public ConcurrentQueue<CommunicationFabricMessage> Deadletter { get; private set; }
    }
}
