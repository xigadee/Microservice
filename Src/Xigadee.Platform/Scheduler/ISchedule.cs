using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ISchedule
    {
        bool Active { get; set; }
        object Context { get; set; }
        Schedule.ScheduleException ExecuteException { get; set; }
        long ExecutionCount { get; }
        TimeSpan? Frequency { get; set; }
        Guid Id { get; }
        DateTime? InitialTime { get; set; }
        TimeSpan? InitialWait { get; set; }
        bool IsInternal { get; set; }
        bool IsLongRunning { get; set; }
        DateTime? LastPollTime { get; }
        string Message { get; set; }
        string Name { get; set; }
        DateTime? NextPollTime { get; }
        long PollActiveSkip { get; }
        string ScheduleType { get; }
        bool ShouldPoll { get; set; }
        object State { get; set; }
        string WillRunIn { get; }

        void Complete(bool success, bool recalculate = true, bool isException = false, Exception lastEx = null, DateTime? exceptionTime = default(DateTime?));
        Task Execute(CancellationToken token);
        void PollSkip();
        void Recalculate(bool force = false);
        void Start();
    }
}