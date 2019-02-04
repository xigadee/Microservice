using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class ResourceTrackerUnitTest1
    {
        private ResourceContainer CreateResourceContainer()
        {
            var policy = new ResourceContainerPolicy();
            SharedServiceContainer sharedServices = new SharedServiceContainer();

            var resourceContainer = new ResourceContainer(policy);
            resourceContainer.SharedServices = sharedServices;

            return resourceContainer;
        }

        [TestMethod]
        public void StartTraceSetup()
        {
            var resourceContainer = CreateResourceContainer();

            var tracker = resourceContainer.SharedServices.GetService<IResourceTracker>();

            Assert.IsNotNull(tracker);

            resourceContainer.Start();

            //Create new profiles.
            var profile1 = new ResourceProfile("v1");
            var profile2 = (ResourceProfile)"v2";

            var consumer1 = tracker.RegisterConsumer("fredo1", profile1);
            var consumer2 = tracker.RegisterConsumer("fredo2", profile2);

            
            var id = consumer1.Start("", Guid.NewGuid());

            consumer1.End(id, Environment.TickCount - 100, ResourceRequestResult.Success);

            var tracker1 = tracker.RegisterRequestRateLimiter("freda", profile1, profile2 );
   
        }
    }
}
