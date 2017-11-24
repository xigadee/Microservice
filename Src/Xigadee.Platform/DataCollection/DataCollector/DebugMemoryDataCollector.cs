using System.Collections.Concurrent;

namespace Xigadee
{
    /// <summary>
    /// This is a test collector. It is primarily used for unit testing to ensure the correct logging has occurred.
    /// </summary>
    public class DebugMemoryDataCollector: DataCollectorBase
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="supportMap">The support map can be used to filter the types of events that you wish to filter. Leave this null to support all types.</param>
        public DebugMemoryDataCollector(DataCollectionSupport? supportMap = null):base(supportMap: supportMap)
        {

        }
        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.Boundary, (e) => EventsBoundary.Add(e));
            SupportAdd(DataCollectionSupport.Dispatcher, (e) => EventsDispatcher.Add(e));
            SupportAdd(DataCollectionSupport.EventSource, (e) => EventsEventSource.Add(e));
            SupportAdd(DataCollectionSupport.Logger, (e) => EventsLog.Add(e));
            SupportAdd(DataCollectionSupport.Statistics, (e) => EventsMicroservice.Add(e));
            SupportAdd(DataCollectionSupport.Telemetry, (e) => EventsMetric.Add(e));

            SupportAdd(DataCollectionSupport.Resource, (e) => EventsResource.Add(e));

            SupportAdd(DataCollectionSupport.Custom, (e) => EventsCustom.Add(e));

            SupportAdd(DataCollectionSupport.Security, (e) => EventsSecurity.Add(e));

            SupportAdd(DataCollectionSupport.ApiBoundary, (e) => EventsBoundary.Add(e));
        }
        /// <summary>
        /// Gets or sets the EventSource loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsEventSource { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the boundary events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsBoundary { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the dispatcher events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsDispatcher { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the log events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsLog { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the events metric loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsMetric { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the microservice events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsMicroservice { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the custom events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsCustom { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the security events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsSecurity { get; set; } = new ConcurrentBag<EventHolder>();
        /// <summary>
        /// Gets or sets the resource events loggers.
        /// </summary>
        public ConcurrentBag<EventHolder> EventsResource { get; set; } = new ConcurrentBag<EventHolder>();

    }
}
