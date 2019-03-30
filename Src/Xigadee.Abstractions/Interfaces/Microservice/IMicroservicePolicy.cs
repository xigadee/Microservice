using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface lists the policy options for the Microservice.
    /// </summary>
    public interface IMicroservicePolicy
    {
        /// <summary>
        /// Gets the Microservice default policy.
        /// </summary>
        MicroservicePolicy Microservice { get; }
        /// <summary>
        /// Gets the task manager policy.
        /// </summary>
        TaskManagerPolicy TaskManager { get; }
        /// <summary>
        /// Gets the resource monitor policy.
        /// </summary>
        ResourceContainerPolicy ResourceMonitor { get; }
        /// <summary>
        /// Gets the command container policy.
        /// </summary>
        CommandContainerPolicy CommandContainer { get; }
        /// <summary>
        /// Gets the communication policy.
        /// </summary>
        CommunicationContainerPolicy Communication { get; }
        /// <summary>
        /// Gets the scheduler policy.
        /// </summary>
        SchedulerContainerPolicy Scheduler { get; }
        /// <summary>
        /// Gets the data collection policy.
        /// </summary>
        DataCollectionContainerPolicy DataCollection { get; }
        /// <summary>
        /// Gets the service handler policy.
        /// </summary>
        ServiceHandlerContainerPolicy ServiceHandlers { get; }
    }
}
