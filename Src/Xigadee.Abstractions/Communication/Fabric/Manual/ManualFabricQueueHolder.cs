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
            Queue = new ConcurrentQueue<FabricMessage>();
        }

        public bool Enqueue(FabricMessage message)
        {
            if (!(Filter?.Invoke(message) ?? true))
                return false;

            Queue.Enqueue(message);
            return true;
        }

        private object syncDeadletter = new object();

        public void DeadletterEnqueue(FabricMessage message)
        {
            if (Deadletter == null)
                lock (syncDeadletter)
                {
                    if (Deadletter == null)
                        Deadletter = new ConcurrentQueue<FabricMessage>();
                }

            Deadletter.Enqueue(message);
        }

        public bool TryDequeue(out FabricMessage message)
        {
            return Queue.TryDequeue(out message);
        }


        public bool TryDeadletterDequeue(out FabricMessage message)
        {
            message = null;
            return Deadletter?.TryDequeue(out message) ?? false;
        }


        public Func<FabricMessage, bool> Filter { get; set; }

        public ConcurrentQueue<FabricMessage> Queue { get; }

        public ConcurrentQueue<FabricMessage> Deadletter { get; private set; }
    }
}
