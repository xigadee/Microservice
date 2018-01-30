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
        Microservice.Policy Microservice { get; }
        /// <summary>
        /// Gets the task manager policy.
        /// </summary>
        /// <value>
        TaskManager.Policy TaskManager { get; }
        /// <summary>
        /// Gets the resource monitor policy.
        /// </summary>
        ResourceContainer.Policy ResourceMonitor { get; }
        /// <summary>
        /// Gets the command container policy.
        /// </summary>
        CommandContainer.Policy CommandContainer { get; }
        /// <summary>
        /// Gets the communication policy.
        /// </summary>
        CommunicationContainer.Policy Communication { get; }
        /// <summary>
        /// Gets the scheduler policy.
        /// </summary>
        SchedulerContainer.Policy Scheduler { get; }
        /// <summary>
        /// Gets the data collection policy.
        /// </summary>
        DataCollectionContainer.Policy DataCollection { get; }
        /// <summary>
        /// Gets the service handler policy.
        /// </summary>
        ServiceHandlerContainer.Policy ServiceHandlers { get; }
    }
}
