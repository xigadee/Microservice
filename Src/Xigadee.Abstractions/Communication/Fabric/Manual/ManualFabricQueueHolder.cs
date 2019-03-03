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
            Queue = new ConcurrentQueue<ManualFabricMessage>();
        }

        public bool Enqueue(ManualFabricMessage message)
        {
            if (!(Filter?.Invoke(message) ?? true))
                return false;

            Queue.Enqueue(message);
            return true;
        }

        private object syncDeadletter = new object();

        public void DeadletterEnqueue(ManualFabricMessage message)
        {
            if (Deadletter == null)
                lock (syncDeadletter)
                {
                    if (Deadletter == null)
                        Deadletter = new ConcurrentQueue<ManualFabricMessage>();
                }

            Deadletter.Enqueue(message);
        }

        public bool TryDequeue(out ManualFabricMessage message)
        {
            return Queue.TryDequeue(out message);
        }


        public bool TryDeadletterDequeue(out ManualFabricMessage message)
        {
            message = null;
            return Deadletter?.TryDequeue(out message) ?? false;
        }


        public Func<ManualFabricMessage, bool> Filter { get; set; }

        public ConcurrentQueue<ManualFabricMessage> Queue { get; }

        public ConcurrentQueue<ManualFabricMessage> Deadletter { get; private set; }
    }
}
