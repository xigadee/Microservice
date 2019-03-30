using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the default configuration.
    /// </summary>
    public class ConfigMicroservice : IMicroservicePolicy
    {
        public MicroservicePolicy Microservice { get; set; }

        public TaskManagerPolicy TaskManager { get; set; }

        public ResourceContainerPolicy ResourceMonitor { get; set; }

        public CommandContainerPolicy CommandContainer { get; set; }

        public CommunicationContainerPolicy Communication { get; set; }

        public SchedulerContainerPolicy Scheduler { get; set; }

        public DataCollectionContainerPolicy DataCollection { get; set; }

        public ServiceHandlerContainerPolicy ServiceHandlers { get; set; }
    }
}
