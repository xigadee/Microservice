using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface IClientHolder
    {
        Guid Id { get; }

        string Name { get; set; }

        string MappingChannelId { get; set; }

        bool IsActive { get; set; }

        Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null);

        long? QueueLengthCurrent();

        void QueueTimeLog(DateTime? EnqueuedTimeUTC);

        void ActiveDecrement(int start);

        void ErrorIncrement();

        List<ResourceProfile> ResourceProfiles { get; set; }

        int Priority { get; set; }

        decimal Weighting { get; set; }

    }

    public interface IClientHolderP
    {



        bool BoundaryLoggingActive { get; set; }
        bool CanStart { get; set; }
        string ChannelId { get; set; }
        Action ClientClose { get; set; }
        Action<Exception> ClientReset { get; set; }
        IDataCollection Collector { get; set; }
        string DebugStatus { get; }
        Action FabricInitialize { get; set; }
        TimeSpan? FabricMaxMessageLock { get; set; }
        List<string> Filters { get; set; }
        int LastTickCount { get; }
        int MaxRetries { get; set; }
        TimeSpan? MessageMaxProcessingTime { get; set; }
        DateTime? QueueLengthLastPoll { get; set; }
        IServiceHandlers ServiceHandlers { get; set; }
        Action Start { get; set; }
        Action Stop { get; set; }
        bool SupportsQueueLength { get; set; }
        bool SupportsRateLimiting { get; set; }
        string Type { get; set; }

        Task Transmit(TransmissionPayload payload, int retry = 0);
    }
}