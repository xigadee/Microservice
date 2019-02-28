using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the default interface for a communication client.
    /// </summary>
    public interface IClientHolder
    {
        /// <summary>
        /// This is the client identifier
        /// </summary>
        Guid Id { get; }

        string Name { get; set; }

        string ChannelId { get; set; }

        string MappingChannelId { get; set; }

        bool IsActive { get; set; }

        Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null);

        long? QueueLengthCurrent();

        List<ResourceProfile> ResourceProfiles { get; set; }

        int Priority { get; set; }

        decimal Weighting { get; set; }

        Task Transmit(TransmissionPayload payload, int retry = 0);

        string DebugStatus { get; }

        MessagingServiceStatistics StatisticsInternal { get; }

        MessagingServiceStatistics StatisticsRecalculated { get; }
    }

    public interface IClientHolderV2: IClientHolder, IRequireDataCollector, IRequireServiceHandlers, IService
    {

        /// <summary>
        /// This method is used to close the client.
        /// </summary>
        void ClientClose();
        /// <summary>
        /// This method is used to reset the client.
        /// </summary>
        void ClientReset(Exception ex);

        /// <summary>
        /// This is the collection
        /// </summary>
        ServiceHandlerIdCollection ServiceHandlerIds { get; set; }
    }

    public static class ClientHolderExtensions
    {
        /// <summary>
        /// This method logs the last time a message was enqueued.
        /// </summary>
        /// <param name="h">The client.</param>
        /// <param name="EnqueuedTimeUTC">The current UTC time.</param>
        public static void QueueTimeLog(this IClientHolder h, DateTime? EnqueuedTimeUTC)
        {
            h.StatisticsInternal.QueueTimeLog(EnqueuedTimeUTC);
            h.StatisticsInternal.ActiveIncrement();
        }

        /// <summary>
        /// This method increments the active messages.
        /// </summary>
        /// <param name="h">The client.</param>
        public static int? ActiveIncrement(this IClientHolder h)
        {
            return h.StatisticsInternal.ActiveIncrement();
        }

        /// <summary>
        /// This method decrements the active messages.
        /// </summary>
        /// <param name="h">The client.</param>
        /// <param name="start">The tick count when the process started.</param>
        public static void ActiveDecrement(this IClientHolder h, int start)
        {
            h.StatisticsInternal.ActiveDecrement(start);
        }
        /// <summary>
        /// This method increments the error count.
        /// </summary>
        /// <param name="h">The client.</param>
        public static void ErrorIncrement(this IClientHolder h)
        {
            h.StatisticsInternal.ErrorIncrement();
        }
    }


    public interface IClientHolderP
    {



        bool BoundaryLoggingActive { get; set; }
        bool CanStart { get; set; }
        Action ClientClose { get; set; }
        Action<Exception> ClientReset { get; set; }
        IDataCollection Collector { get; set; }
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

    }
}