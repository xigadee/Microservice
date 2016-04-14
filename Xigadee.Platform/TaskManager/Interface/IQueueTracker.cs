namespace Xigadee
{
    public interface IQueueTracker
    {
        bool IsEmpty { get; }
        void Enqueue(TaskTracker item);
        bool TryDequeue(out TaskTracker item);

        MessagingStatistics Statistics{get;}

        int Count { get; }
    }
}