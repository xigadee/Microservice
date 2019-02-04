namespace Xigadee
{
    /// <summary>
    /// This interface is used for TaskTracker queueing.
    /// </summary>
    public interface IQueueTracker
    {
        /// <summary>
        /// Gets the queue level.
        /// </summary>
        int Level { get; }
        /// <summary>
        /// Gets or sets the queue level.
        /// </summary>
        void Configure(int level);
        /// <summary>
        /// Returns true if the queue is empty.
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Enqueues the TaskTracker.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        void Enqueue(TaskTracker item);
        /// <summary>
        /// Tries to dequeue the first item in the queue.
        /// </summary>
        /// <param name="item">As out parameter with the item dequeued.</param>
        /// <returns>Returns true if an item is dequeued.</returns>
        bool TryDequeue(out TaskTracker item);
        /// <summary>
        /// Returns the statistics on the queue.
        /// </summary>
        QueueTrackerStatistics StatisticsRecalculated {get;}
        /// <summary>
        /// Returns the number of items in the queue.
        /// </summary>
        int Count { get; }
    }
}