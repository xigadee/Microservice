using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public class SerializationHarnessDependencies: ServiceHarnessDependencies
    {
        /// <summary>
        /// Initializes the service relationships.
        /// </summary>
        protected override void Initialise()
        {
            ResourceTracker.Start();

            Scheduler.Collector = Collector;
            Scheduler.Start();

            ResourceTracker.Collector = Collector;
            ResourceTracker.SharedServices = SharedService;
            ResourceTracker.Start();
        }


        public override ServiceHarnessSerializationContainer PayloadSerializer => null;
    }

    public class SerializationContainerHarness: ServiceHarness<SerializationContainer, SerializationHarnessDependencies>
    {
    }
}
