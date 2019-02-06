using System;

namespace Xigadee
{
    /// <summary>
    /// This is the service harness dependencies.
    /// </summary>
    public class ServiceHarnessDependencies
    {
        /// <summary>
        /// This is the default constructor that connects the services.
        /// </summary>
        public ServiceHarnessDependencies()
        {
            Initialise();
        }

        /// <summary>
        /// Initializes the service relationships.
        /// </summary>
        protected virtual void Initialise()
        {
            ServiceHandlers?.Start();
            ResourceTracker.Start();

            Scheduler.Collector = Collector;
            Scheduler.Start();

            ResourceTracker.Collector = Collector;
            ResourceTracker.SharedServices = SharedService;
            ResourceTracker.Start();
        }

        #region Configure(object service)
        /// <summary>
        /// This method sets the object required services.
        /// </summary>
        /// <param name="service">The service.</param>
        public virtual void Configure(object service)
        {
            if (service is IRequireDataCollector)
                ((IRequireDataCollector)service).Collector = Collector;

            if (service is IRequireScheduler)
                ((IRequireScheduler)service).Scheduler = Scheduler;

            if (service is IRequireSharedServices)
                ((IRequireSharedServices)service).SharedServices = SharedService;

            if (service is IRequireServiceOriginator)
                ((IRequireServiceOriginator)service).OriginatorId = OriginatorId;

            if (service is IRequireServiceHandlers)
                ((IRequireServiceHandlers)service).ServiceHandlers = ServiceHandlers;
        } 
        #endregion

        /// <summary>
        /// This is the stub Payload serializer.
        /// </summary>
        public virtual ServiceHarnessServiceHandlerContainer ServiceHandlers { get; }  = new ServiceHarnessServiceHandlerContainer();
        /// <summary>
        /// This is the example originator id.
        /// </summary>
        public virtual MicroserviceId OriginatorId { get; } =  new MicroserviceId(nameof(ServiceHarnessDependencies), Guid.NewGuid().ToString("N").ToUpperInvariant());
        /// <summary>
        /// This is the stub data collector.
        /// </summary>
        public virtual ServiceHarnessDataCollection Collector { get; } = new ServiceHarnessDataCollection();
        /// <summary>
        /// This is the stub scheduler
        /// </summary>
        public virtual ServiceHarnessScheduler Scheduler { get; } = new ServiceHarnessScheduler();
        /// <summary>
        /// This is the stub shared service container.
        /// </summary>
        public virtual ServiceHarnessSharedService SharedService { get; } = new ServiceHarnessSharedService();
        /// <summary>
        /// This is the resource tracker for the harness.
        /// </summary>
        public virtual ServiceHarnessResourceContainer ResourceTracker { get; } = new ServiceHarnessResourceContainer();
    }
}
