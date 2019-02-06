using System;
using System.Collections.Generic;

namespace Xigadee
{
    internal class ResourceWrapper: WrapperBase, IMicroserviceResourceMonitor
    {
        ResourceContainer mResourceContainer;

        internal ResourceWrapper(ResourceContainer resourceMonitor, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mResourceContainer = resourceMonitor;
        }

        public IResourceConsumer RegisterConsumer(string name, ResourceProfile profile)
        {
            return mResourceContainer.RegisterConsumer(name, profile);
        }

        public IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, IEnumerable<ResourceProfile> profiles)
        {
            return mResourceContainer.RegisterRequestRateLimiter(name, profiles);
        }

        public IResourceRequestRateLimiter RegisterRequestRateLimiter(string name, params ResourceProfile[] profiles)
        {
            return mResourceContainer.RegisterRequestRateLimiter(name, profiles);
        }

        public ResourceStatus ResourceStatusGet(string name)
        {
            return mResourceContainer.ResourceStatusGet(name);
        }
    }
}
