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
            Queue = new ConcurrentQueue<ManaualFabricMessage>();
        }

        public bool Enqueue(ManaualFabricMessage message)
        {
            if (!(Filter?.Invoke(message) ?? true))
                return false;

            Queue.Enqueue(message);
            return true;
        }

        private object syncDeadletter = new object();

        public void DeadletterEnqueue(ManaualFabricMessage message)
        {
            if (Deadletter == null)
                lock (syncDeadletter)
                {
                    if (Deadletter == null)
                        Deadletter = new ConcurrentQueue<ManaualFabricMessage>();
                }

            Deadletter.Enqueue(message);
        }

        public bool TryDequeue(out ManaualFabricMessage message)
        {
            return Queue.TryDequeue(out message);
        }


        public bool TryDeadletterDequeue(out ManaualFabricMessage message)
        {
            message = null;
            return Deadletter?.TryDequeue(out message) ?? false;
        }


        public Func<ManaualFabricMessage, bool> Filter { get; set; }

        public ConcurrentQueue<ManaualFabricMessage> Queue { get; }

        public ConcurrentQueue<ManaualFabricMessage> Deadletter { get; private set; }
    }
}
